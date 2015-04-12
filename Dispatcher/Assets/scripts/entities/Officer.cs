using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Officer : MonoBehaviour {

	private int officerLevel;
	public int officerIndex;
	private List<Crime> crimesSolved = new List<Crime>();
	private Crime currentCrime;
	private Depot theDepot;

	public enum OfficerState
	{
		isAtDepot,
		isTravelling_interruptible,
		isTravelling_uninterruptible,
		isAtCrime,
	}
	private OfficerState m_officerState;
	public enum DestinationType
	{
		Depot,
		Crime,
		Tip,
		Raid
	}
	private DestinationType m_destinationType;

	private Structure destination;
	private float progressAlongTrip;
	private Vector3 tripStart;
	private Vector3 tripEnd;

	// officer attributes
	private float m_crimeResolvingSpeed = 5.0f;
	private float m_speed = 0.4f;
	private int m_level = 0;

	void Start()
	{
		InputManager.Instance.OnClick += OnClick;
		theDepot = GameObject.Find ("Depot").GetComponent<Depot>();
		m_destinationType = DestinationType.Depot;
	}

	void Update()
	{
		if (m_officerState == OfficerState.isAtDepot)
		{
			// do nothing
		}
		else if (m_officerState == OfficerState.isTravelling_interruptible)
		{
			if (m_destinationType == DestinationType.Crime)
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
			else if (m_destinationType == DestinationType.Depot)
			{
				if (MoveTowardDestination())
				{
					EnterDepot();
				}
			}
		}
		else if (m_officerState == OfficerState.isTravelling_uninterruptible)
		{
			if (MoveTowardDestination())
			{
				EnterDepot();
			}
		}
		else if (m_officerState == OfficerState.isAtCrime)
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

	public void AssignCrime(Crime _crime)
	{
		currentCrime = _crime;
		PathFind (_crime.GetBuilding());
		m_destinationType = DestinationType.Crime;
		if (m_officerState == OfficerState.isAtDepot)
		{
			LeaveDepot(true);
		}
	}
	
	void PathFind(Structure _destination)
	{
		// find shortest path
		progressAlongTrip = 0.0f;
		tripStart = transform.position;
		tripEnd = _destination.gameObject.transform.position;
	}
	
	void LeaveDepot(bool _isHeadingToCrime)
	{
		m_officerState = OfficerState.isTravelling_interruptible;
		if (_isHeadingToCrime)
		{
			PathFind(currentCrime.GetBuilding());
		}
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
		m_destinationType = DestinationType.Depot;
	}
	
	void EnterDepot()
	{
		m_officerState = OfficerState.isAtDepot;
	}
	
	void LeaveCompletedCrimeScene()
	{
		m_officerState = OfficerState.isTravelling_uninterruptible;
		PathFind ((Structure)theDepot);
	}

	void WorkCrime()
	{
		GetCurrentCrime().UpdateDuration(-Clock.GetDeltaTime() * m_crimeResolvingSpeed);
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
		m_officerState = OfficerState.isAtCrime;
	}

	public void Initialize(int _index)
	{
		officerIndex = _index;
	}

	void OnClick (GameObject go)
	{
		if (go == gameObject)
		{
			if (GetIsAvailableForAssignment())
			{
				GameObject.Find ("GameManager").GetComponent<GameManager>().GetGameEvents(this);
			}
		}
	}

	void GainExperience(int _experience)
	{

	}

	void LevelUp()
	{

	}

	public bool GetIsAtCrime()
	{
		return (m_officerState == OfficerState.isAtCrime);
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
		return (m_officerState == OfficerState.isAtDepot);
	}

	bool GetCanBeRerouted()
	{
		return (m_officerState != OfficerState.isTravelling_uninterruptible);
	}

	public bool GetIsAvailableForAssignment()
	{
		return (GetIsAtDepot() || GetCanBeRerouted());
	}
}
