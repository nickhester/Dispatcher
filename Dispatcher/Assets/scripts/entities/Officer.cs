using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Officer : MonoBehaviour {
	
	public int officerIndex;
	private List<Crime> crimesSolved = new List<Crime>();
	private Crime currentCrime;
	private Depot theDepot;
	private Structure currentLocation;
	public List<GameObject> headImages = new List<GameObject>();

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
	public float m_xp = 0.0f;
	private Dictionary<Neighborhood, float> m_xpFromNeighborhoods = new Dictionary<Neighborhood, float>();
	private Dictionary<types.CrimeType, float> m_xpFromCrimeTypes = new Dictionary<types.CrimeType, float>();
	private int m_level = 0;
	private float m_crimeResolvingSpeed = 5.0f;
	private float m_speed = 2.0f;

	public void Initialize(int _index, int _level)
	{
		officerIndex = _index;
		m_level = _level;
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
					EnterDepot();
				}
			}
		}
		else if (m_officerState == types.OfficerState.isTravelling_uninterruptible)
		{
			if (MoveTowardDestination())
			{
				EnterDepot();
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
		print ("RESULT: " + currentRoute.ToString());
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
	
	void ArriveAtLocation(Structure _destination)
	{
		currentLocation = _destination;
	}
	
	void CancelTripToCrime()
	{
		currentCrime = null;
		m_destinationType = types.DestinationType.Depot;
		SubmitTask((Structure)theDepot);
	}
	
	bool MoveTowardDestination()
	{
		// move toward destination along path
		progressAlongTrip += (m_speed * Time.deltaTime) / currentRoute.GetDistance();
		float distAlongTrip = currentRoute.GetDistance() * progressAlongTrip;
		for (int i = 0; i < currentRoute.segmentLengths.Count; i++)
		{
			if (i > lastRouteNodeReached)
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

		if (progressAlongTrip >= 1.0f)
		{
			return true;
		}
		return false;
	}
	
	void EnterDepot()
	{
		m_officerState = types.OfficerState.isAtDepot;
		ArriveAtLocation(destination);
	}

	void WorkCrime()
	{
		GetCurrentCrime().UpdateDuration(-Clock.GetDeltaTime() * m_crimeResolvingSpeed);
		GainExperience(Clock.GetDeltaTime());

		// if the crime is resolved
		if (GetCurrentCrime().CheckIfDurationEmptied())
		{
			ResolveCurrentCrime();
		}
	}
	
	void ArriveAtCrime()
	{
		m_officerState = types.OfficerState.isAtCrime;
		ArriveAtLocation(destination);
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
		m_xp += Clock.GetDeltaTime();
		// add experience points toward neighborhood
		if (m_xpFromNeighborhoods.ContainsKey(GetCurrentCrime().GetNeighborhood()))
		{
			m_xpFromNeighborhoods[GetCurrentCrime().GetNeighborhood()] += Clock.GetDeltaTime();
		}
		else
		{
			m_xpFromNeighborhoods.Add(GetCurrentCrime().GetNeighborhood(), Clock.GetDeltaTime());
		}
		// add experience points toward crime type
		if (m_xpFromCrimeTypes.ContainsKey(GetCurrentCrime().GetCrimeType()))
		{
			m_xpFromCrimeTypes[GetCurrentCrime().GetCrimeType()] += Clock.GetDeltaTime();
		}
		else
		{
			m_xpFromCrimeTypes.Add(GetCurrentCrime().GetCrimeType(), Clock.GetDeltaTime());
		}

		// TODO: check if level up
	}

	void LevelUp()
	{

	}
	
	public GameObject GetMyHead()
	{
		return headImages[0];
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
		return (GetIsAtDepot() || m_officerState != types.OfficerState.isTravelling_uninterruptible);
	}
}
