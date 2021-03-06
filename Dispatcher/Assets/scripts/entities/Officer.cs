//#define FAST_PROGRESS

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Officer : MonoBehaviour {
	
	public int m_index;
	private List<Crime> crimesSolved = new List<Crime>();
	private Crime currentCrime;
	private Depot theDepot;
	private Structure currentLocation;
	public GameObject headPin;
	public Sprite[] headImages;
	private Sprite m_sprite;

	private types.OfficerState m_officerState;

	private types.DestinationType m_destinationType;

	private Structure destination;
	private Route currentRoute;
	private float progressAlongTrip;
	private Vector3 tripStart;
	private Vector3 tripEnd;
	private bool tripCanBeInterrupted = false;
	private int lastRouteNodeReached;
	private List<OfficerTask> taskQueue = new List<OfficerTask>();

	// officer attributes
	public class OfficerLevel
	{
		public float xp;
		public int level;

		public OfficerLevel(float _xp, int _level)
		{
			xp = _xp;
			level = _level;
		}
	}
	private OfficerLevel m_level;
	private Dictionary<Neighborhood, OfficerLevel> m_level_neighborhoods = new Dictionary<Neighborhood, OfficerLevel>();
	private Dictionary<types.CrimeType, OfficerLevel> m_level_crimeTypes = new Dictionary<types.CrimeType, OfficerLevel>();

#if (FAST_PROGRESS)

	private float m_baseCrimeResolvingSpeed = 2.0f;
	private float m_baseTravelingSpeed = 100.0f;
	private float[] levelTravelingSpeedMultiplier = { 1.0f, 1.2f, 1.4f, 1.8f };
	private float[] levelResolvingMultiplier = { 1.0f, 1.2f, 1.4f, 1.8f };
	private float[] levelResolvingMultiplier_crimeType = { 1.0f, 2.0f, 4.0f, 8.0f };
	private float[] levelResolvingMultiplier_neighborhood = { 1.0f, 1.2f, 1.4f, 1.8f };
	private int[] xpRequirements_level = { 5, 10, 15, 20 };
	private int[] xpRequirements_crime = { 5, 10, 15, 20 };
	private int[] xpRequirements_neighborhood = { 5, 10, 15, 20 };
	private float incarcerationTime = 2.0f;

#else

	private float m_baseCrimeResolvingSpeed = 2.0f;
	private float m_baseTravelingSpeed = 20.0f;
	private float[] levelTravelingSpeedMultiplier = { 1.0f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f };
	private float[] levelResolvingMultiplier = { 1.0f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f };
	private float[] levelResolvingMultiplier_crimeType = { 1.0f, 1.1f, 1.2f, 1.3f };
	private float[] levelResolvingMultiplier_neighborhood = { 1.0f, 1.1f, 1.2f, 1.3f };
	private int[] xpRequirements_level = { 10, 20, 30, 70, 120, 180 };
	private int[] xpRequirements_crime = { 30, 60, 90, 120 };
	private int[] xpRequirements_neighborhood = { 30, 60, 90, 120 };
	private float incarcerationTime = 5.0f;

#endif


	public void Initialize(int _index, int _level)
	{
		m_index = _index;
		if (_level > 0)
		{
			m_level = new OfficerLevel(xpRequirements_level[_level - 1], _level);
		}
		else
		{
			m_level = new OfficerLevel(0.0f, 0);
		}

		// add all possible keys to dictionaries
		foreach (Neighborhood neighborhood in GameObject.Find("City").GetComponent<City>().GetNeighborhoods())
		{
			m_level_neighborhoods.Add(neighborhood, new OfficerLevel(0.0f, 0));
		}
		foreach (types.CrimeType type in System.Enum.GetValues(typeof(types.CrimeType)))
		{
			m_level_crimeTypes.Add(type, new OfficerLevel(0.0f, 0));
		}

		// head sprite
		m_sprite = headImages[m_index % (headImages.Length)];
	}

	void Start()
	{
		//InputManager.Instance.OnClick += OnClick;
		theDepot = GameObject.Find ("Depot").GetComponent<Depot>();
		currentLocation = theDepot;
		m_destinationType = types.DestinationType.Depot;
	}

	void Update()
	{
		// Activate next in queue if available
		if (GetIsAvailableForAssignment())
		{
			StartCoroutine("DequeueTask");
		}

		if (m_officerState == types.OfficerState.isAtDepot)
		{
			// do nothing
		}
		else if (m_officerState == types.OfficerState.isAtBuilding)
		{
			// do nothing
		}
		else if (m_officerState == types.OfficerState.isTravelling_interruptible)
		{
			if (m_destinationType == types.DestinationType.Crime)
			{
				if (!GetCurrentCrimeIsActive())
				{
					CancelTripToCrime();
				}
				else if (MoveTowardDestination())
				{
					ArriveAtCrime();
				}
			}
			else if (m_destinationType == types.DestinationType.Depot)
			{
				if (MoveTowardDestination())
				{
					EnterDepot(false);
				}
			}
			else if (m_destinationType == types.DestinationType.Building)
			{
				if (MoveTowardDestination())
				{
					ArriveAtBuildingNoActivity();
				}
			}
		}
		else if (m_officerState == types.OfficerState.isTravelling_uninterruptible)
		{
			if (MoveTowardDestination())
			{
				EnterDepot(true);
			}
		}
		else if (m_officerState == types.OfficerState.isAtCrime)
		{
			if (GetCurrentCrimeIsActive())
			{
				WorkCrime();
			}
			else
			{
				LeaveCompletedCrimeScene();
			}
		}
	}

	public void SubmitTask(OfficerTask _task)
	{
		taskQueue.Add (_task);
	}

	public void SubmitTask(Crime _crime)
	{
		OfficerTask task = new OfficerTask((Activity)_crime);
		SubmitTask(task);
	}

	public void SubmitTask(Structure _structure)
	{
		OfficerTask task = new OfficerTask(_structure);
		SubmitTask(task);
	}

	IEnumerator DequeueTask()
	{
		if (taskQueue.Count < 1)
			yield break;

		OfficerTask task = taskQueue[0];
		taskQueue.RemoveAt(0);

		// check to see if officer can be interrpted, wait if not
		if ((m_officerState == types.OfficerState.isTravelling_interruptible))
		{
			while (!tripCanBeInterrupted)
			{
				yield return null;
			}
		}
		// interpret the task, then initiate
		int type = task.GetTaskType();
		if (type == 0)  // crime
		{
			AssignCrime((Crime)task.GetActivity());
		}
		else if (type == 1 || type == 2)  // depot or building
		{
			GoToLocation ((Structure)task.GetLocation());
		}
	}

	void AssignCrime(Crime _crime)
	{
		currentCrime = _crime;
		m_destinationType = types.DestinationType.Crime;
		m_officerState = types.OfficerState.isTravelling_interruptible;

		InitiateTrip(_crime.GetBuilding());
	}

	void GoToLocation(Structure _s)
	{
		Depot structure_depot = _s as Depot;
		if (structure_depot != null)
		{
			m_destinationType = types.DestinationType.Depot;
		}
		else
		{
			m_destinationType = types.DestinationType.Building;
		}

		currentCrime = null;
		m_officerState = types.OfficerState.isTravelling_interruptible;
		InitiateTrip (_s);
	}

	void InitiateTrip(Structure _s)	// this is the final stop that actually initiates the trip
	{
		destination = _s;
		PathFind (_s);
		currentLocation = null;
	}
	
	void PathFind(Structure _destination)
	{
		// find shortest path
		progressAlongTrip = 0.0f;
		currentRoute = GameObject.Find("City").GetComponent<City>().pathfinder.FindPath(currentLocation, _destination);
		//print ("RESULT: " + currentRoute.ToString());
		currentRoute.DrawRouteDebugLine();
		lastRouteNodeReached = 0;
	}
	
	void LeaveCompletedCrimeScene()
	{
		m_officerState = types.OfficerState.isTravelling_interruptible;
		currentCrime = null;
		
		InitiateTrip((Structure)theDepot);
	}
	
	void ResolveCurrentCrime()
	{
		// record crime as a resolved crime
		theDepot.ReportResolvedCrime(currentCrime);
		m_officerState = types.OfficerState.isTravelling_uninterruptible;

		// adjust officer experience as necessary
		
		currentCrime.ResolveCrime();
		currentCrime = null;

		InitiateTrip((Structure)theDepot);
	}
	
	void CancelTripToCrime()
	{
		currentCrime = null;
		m_destinationType = types.DestinationType.Building;
		m_officerState = types.OfficerState.isTravelling_interruptible;
		SubmitTask((Structure)theDepot);
	}
	
	bool MoveTowardDestination()
	{
		float currentSpeed = m_baseTravelingSpeed * levelTravelingSpeedMultiplier[m_level.level];

		// move toward destination along path
		progressAlongTrip += (currentSpeed * Time.deltaTime) / currentRoute.GetDistance();
		float distAlongTrip = currentRoute.GetDistance() * progressAlongTrip;

		bool retVal = false;
		if (progressAlongTrip >= 1.0f)
		{
			retVal = true;
		}

		for (int i = 0; i < currentRoute.segmentLengths.Count; i++)
		{
			if (i > lastRouteNodeReached || retVal == true)
			{
				lastRouteNodeReached = i;
				tripCanBeInterrupted = true;
				currentLocation = (Structure)currentRoute.nodes[i].structure;
			}
			else
			{
				tripCanBeInterrupted = false;
			}

			distAlongTrip -= currentRoute.segmentLengths[i];
			if (distAlongTrip < 0.0f)
			{
				Vector3 _o = currentRoute.nodes[i].GetPosition();
				Vector3 _d = currentRoute.nodes[i + 1].GetPosition();
				transform.position = ((_d - _o) * ((distAlongTrip + currentRoute.segmentLengths[i])/currentRoute.segmentLengths[i])) + _o;
				break;
			}
		}

		return retVal;
	}
	
	void EnterDepot(bool _hasCriminal)
	{
		ArriveAtLocation();
		if (_hasCriminal)
		{
			m_officerState = types.OfficerState.isIncarcerating;
			StartCoroutine("Incarcerate");
		}
		else
		{
			m_officerState = types.OfficerState.isAtDepot;
		}
	}

	IEnumerator Incarcerate()
	{
		while (incarcerationTime > 0.0f)
		{
			incarcerationTime -= Clock.GetDeltaTime();
			yield return null;
		}
		print ("end");
		m_officerState = types.OfficerState.isAtDepot;
	}

	
	void ArriveAtCrime()
	{
		m_officerState = types.OfficerState.isAtCrime;
		ArriveAtLocation();
	}

	void ArriveAtBuildingNoActivity()
	{
		m_officerState = types.OfficerState.isAtBuilding;
		ArriveAtLocation();
	}
	
	void ArriveAtLocation()
	{
		currentLocation = destination;
	}

	void WorkCrime()
	{
		float currentSpeed = m_baseCrimeResolvingSpeed
			* levelResolvingMultiplier[m_level.level]
			* levelResolvingMultiplier_crimeType[m_level_crimeTypes[GetCurrentCrime().GetCrimeType()].level]
			* levelResolvingMultiplier_neighborhood[m_level_neighborhoods[GetCurrentCrime().GetNeighborhood()].level]
			* ((float)(m_level.level + 1) / (float)(GetCurrentCrime().GetCrimeLevel() + 1));

		GetCurrentCrime().UpdateDuration(Clock.GetDeltaTime() * -currentSpeed);
		GainExperience(Clock.GetDeltaTime());

		// if the crime is resolved
		if (GetCurrentCrime().CheckIfDurationEmptied())
		{
			ResolveCurrentCrime();
		}
	}

	public void OnHeadClick ()
	{
		if (GetIsAvailableForAssignment())
		{
			GameObject.Find ("GameManager").GetComponent<GameManager>().GetGameEvents(this);
			GameObject.Find ("FloatingUIManager").GetComponent<FloatingUI>().ClearCurrentHeads();
		}
	}

	void GainExperience(float _experience)
	{
		// add experience points
		m_level.xp += Clock.GetDeltaTime();
		// add experience points toward neighborhood
		if (m_level_neighborhoods.ContainsKey(GetCurrentCrime().GetNeighborhood()))
		{
			m_level_neighborhoods[GetCurrentCrime().GetNeighborhood()].xp += Clock.GetDeltaTime();
		}
		else
		{
			m_level_neighborhoods.Add(GetCurrentCrime().GetNeighborhood(), new OfficerLevel(Clock.GetDeltaTime(), 0));
		}
		// add experience points toward crime type
		if (m_level_crimeTypes.ContainsKey(GetCurrentCrime().GetCrimeType()))
		{
			m_level_crimeTypes[GetCurrentCrime().GetCrimeType()].xp += Clock.GetDeltaTime();
		}
		else
		{
			m_level_crimeTypes.Add(GetCurrentCrime().GetCrimeType(), new OfficerLevel(Clock.GetDeltaTime(), 0));
		}

		// check if level up
		for (int i = 0; i < xpRequirements_level.Length; i++)
		{
			// level up
			if (i > m_level.level)
			{
				if (m_level.xp >= xpRequirements_level[i])
				{
					LevelUp(m_level);
				}
			}
			// level up crime type
			if (i < xpRequirements_crime.Length)
			{
				OfficerLevel ol = m_level_crimeTypes[GetCurrentCrime().GetCrimeType()];
				if (i > ol.level)
				{
					if (ol.xp >= xpRequirements_crime[i])
					{
						LevelUpCrime(ol);
					}
				}
			}
			// level up neighborhood
			if (i < xpRequirements_neighborhood.Length)
			{
				OfficerLevel ol = m_level_neighborhoods[GetCurrentCrime().GetNeighborhood()];
				if (i > ol.level)
				{
					if (ol.xp >= xpRequirements_neighborhood[i])
					{
						LevelUpNeighborhood(ol);
					}
				}
			}
		}
	}

	void LevelUpGeneral(OfficerLevel _level)
	{
		_level.level++;
	}

	void LevelUp(OfficerLevel _level)
	{
		LevelUpGeneral(_level);
		MonoBehaviour.print("Officer " + m_index + " levels up to level " + (_level.level + 1));
	}

	void LevelUpCrime(OfficerLevel _level)
	{
		LevelUpGeneral(_level);
		MonoBehaviour.print("Officer " + m_index + " levels up a crime to level " + (_level.level + 1));
	}

	void LevelUpNeighborhood(OfficerLevel _level)
	{
		LevelUpGeneral(_level);
		MonoBehaviour.print("Officer " + m_index + " levels up a neighborhood to level " + (_level.level + 1));
	}
	
	public GameObject GetMyHead()
	{
		return headPin;
	}

	public Sprite GetMySprite()
	{
		return m_sprite;
	}

	// =========== PUBLIC GETTERS ===========

	public bool GetIsAtCrime()
	{
		return (m_officerState == types.OfficerState.isAtCrime);
	}

	public Crime GetCurrentCrime()
	{
		return currentCrime;
	}

	public bool GetCurrentCrimeIsActive()	// this returns true if the crime is active to prevent having to null check
	{
		if (currentCrime != null)
		{
			return currentCrime.GetIsActive();
		}
		return false;
	}

	public bool GetIsAtDepot()
	{
		return (m_officerState == types.OfficerState.isAtDepot);
	}

	public bool GetIsAvailableForAssignment()
	{
		bool isAvailable = true;
		if (m_officerState == types.OfficerState.isTravelling_uninterruptible)
			isAvailable = false;
		else if (m_officerState == types.OfficerState.isIncarcerating)
			isAvailable = false;
		return isAvailable;
	}

	public int GetCrimeLevel(types.CrimeType _type)
	{
		return m_level_crimeTypes[_type].level;
	}

	public int GetOfficerLevel()
	{
		return m_level.level;
	}
}
