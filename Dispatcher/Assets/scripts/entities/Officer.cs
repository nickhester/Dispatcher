using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Officer : MonoBehaviour {
	
	public int officerIndex;
	private List<Crime> crimesSolved = new List<Crime>();
	private Crime currentCrime;
	private Depot theDepot;
	public List<GameObject> headImages = new List<GameObject>();

	private types.OfficerState m_officerState;

	private types.DestinationType m_destinationType;

	private Structure destination;
	private float progressAlongTrip;
	private Vector3 tripStart;
	private Vector3 tripEnd;

	// officer attributes
	public float m_xp = 0.0f;
	private Dictionary<Neighborhood, float> m_xpFromNeighborhoods = new Dictionary<Neighborhood, float>();
	private Dictionary<types.CrimeType, float> m_xpFromCrimeTypes = new Dictionary<types.CrimeType, float>();
	private int m_level = 0;
	private float m_crimeResolvingSpeed = 5.0f;
	private float m_speed = 0.4f;

	public void Initialize(int _index, int _level)
	{
		officerIndex = _index;
		m_level = _level;
	}

	void Start()
	{
		//InputManager.Instance.OnClick += OnClick;
		theDepot = GameObject.Find ("Depot").GetComponent<Depot>();
		m_destinationType = types.DestinationType.Depot;
	}

	void Update()
	{
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

	public GameObject GetMyHead()
	{
		return headImages[0];
	}

	public void AssignCrime(Crime _crime)
	{
		currentCrime = _crime;
		PathFind (_crime.GetBuilding());
		m_destinationType = types.DestinationType.Crime;

		if (m_officerState == types.OfficerState.isAtDepot)
		{
			LeaveDepot(true);
		}
		else
			m_officerState = types.OfficerState.isTravelling_interruptible;
	}
	
	void PathFind(Structure _destination)
	{
		// find shortest path
		progressAlongTrip = 0.0f;
		tripStart = transform.position;
		tripEnd = _destination.gameObject.transform.position;

		// just for fun, use the pathfinder
		print ("RESULT: " + GameObject.Find("City").GetComponent<City>().pathfinder.FindPath((Structure)theDepot, _destination).ToString());
	}
	
	void LeaveDepot(bool _isHeadingToCrime)
	{
		m_officerState = types.OfficerState.isTravelling_interruptible;
	}
	
	bool MoveTowardDestination()
	{
		// move toward destination along path
		progressAlongTrip += m_speed * Time.deltaTime;
		transform.position = ((tripEnd - tripStart) * progressAlongTrip) + tripStart;
		if (progressAlongTrip >= 1.0f)
		{
			return true;
		}
		return false;
	}
	
	void CancelTripToCrime()
	{
		currentCrime = null;
		PathFind ((Structure)theDepot);
		m_destinationType = types.DestinationType.Depot;
	}
	
	void EnterDepot()
	{
		m_officerState = types.OfficerState.isAtDepot;
	}
	
	void LeaveCompletedCrimeScene()
	{
		m_officerState = types.OfficerState.isTravelling_uninterruptible;
		PathFind ((Structure)theDepot);
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
	
	void ResolveCurrentCrime()
	{
		// record crime as a solved crime
		theDepot.ReportResolvedCrime(currentCrime);
		// adjust officer experience as necessary
		
		currentCrime.ResolveCrime();
		currentCrime = null;
		LeaveCompletedCrimeScene();
	}
	
	void ArriveAtCrime()
	{
		m_officerState = types.OfficerState.isAtCrime;
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

	bool GetCanBeRerouted()
	{
		return (m_officerState != types.OfficerState.isTravelling_uninterruptible);
	}

	public bool GetIsAvailableForAssignment()
	{
		return (GetIsAtDepot() || GetCanBeRerouted());
	}
}
