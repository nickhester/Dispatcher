//#define FAST_PROGRESS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Depot : Structure {

	private List<Officer> officers = new List<Officer>();
	private City theCity;
	private List<Crime> listOfKnownCrimes = new List<Crime>();
	public GameObject officerPrefab;
	private int officerIndex = 0;
	private List<Crime> listOfCrimesResolved = new List<Crime>();

	// resource stuff
	private int m_cash = 0;
#if FAST_PROGRESS
	private int[] cost_officer = { 0, 0, 20, 30, 40, 50, 60, 70 };
	private int earned_resolvedCrimeCashPerLevel = 10;
#else
	private int[] cost_officer = { 0, 0, 50, 100, 200, 400, 800, 1600 };
	private int earned_resolvedCrimeCashPerLevel = 2;
#endif
	private int earned_buildingGood = 2;
	private int earned_buildingMedium = 1;
	private int earned_buildingBad = 0;

	public delegate void OnGenerateOfficerEvent();
	public event OnGenerateOfficerEvent OnGenerateOfficer;

	private float[] crimeDiscoveryRange = { 0.1f, 0.5f };

	void Start()
	{
		theCity = GameObject.Find ("City").GetComponent<City>();

		// start game with 2 officers
		GenerateOfficer(2);
		GenerateOfficer(0);

		InputManager.Instance.OnClick += OnClick;
	}

	void Update()
	{
		RemoveCompletedCrimes();
		DetectNewCrimes();
	}

	void OnClick (GameObject go)
	{
		if (go == gameObject)
		{
			// if depot is clicked, try to buy a new officer
			if (m_cash >= cost_officer[officers.Count])
			{
				m_cash -= cost_officer[officers.Count];
				GenerateOfficer(0);
			}
		}
	}

	// this is the progress that the crime will have before it was reported
	public float GetCrimeDiscoveryValue()
	{
		return Random.Range(crimeDiscoveryRange[0], crimeDiscoveryRange[1]);
	}

	void RemoveCompletedCrimes()
	{
		int len = listOfKnownCrimes.Count;
		for (int i = 0; i < len; i++)
		{
			if (listOfKnownCrimes[i].GetIsActive() == false)
			{
				listOfKnownCrimes.RemoveAt(i);
				i--; len--;
			}
		}
	}

	void DetectNewCrimes()
	{
		List<Structure> allStructures = theCity.GetAllStructures();
		foreach (Structure structure in allStructures)
		{
			Building building = structure as Building;
			if (building == null) { continue; }
			if (building.IsCrimeOccuring())
			{
				Crime c = building.GetCurrentCrime();
				if (!GetListOfCrimes().Contains(c))
				{
					listOfKnownCrimes.Add(c);
				}
			}
		}
	}

	void ReportCrimeToPlayer(Crime _crime)
	{
		// send crime to GUI
	}

	void SendOfficer(Officer _officer, Crime _crime)
	{
		// send an existing officer out for an action
		_officer.SubmitTask(_crime);
		//print ("depot sends officer (#" + _officer.officerIndex + ")");
	}

	public void SendOfficerRequest(Officer _officer, Crime _crime)
	{
		SendOfficer (_officer, _crime);
	}

	void GenerateOfficer(int _level)
	{
		// create a new officer when earned
		GameObject o = Instantiate(officerPrefab) as GameObject;
		o.transform.SetParent(gameObject.transform);
		Officer o_comp = o.GetComponent<Officer>();
		o_comp.Initialize(officerIndex, _level);
		officers.Add(o_comp);
		officerIndex++;

		// send event
		OnGenerateOfficer();
	}

	public List<Crime> GetListOfCrimes()
	{
		return listOfKnownCrimes;
	}

	List<Officer> GetOfficersAtDepot()
	{
		List<Officer> retVal = new List<Officer>();
		foreach (Officer officer in officers)
		{
			if (officer.GetIsAtDepot())
			{
				retVal.Add(officer);
			}
		}
		return retVal;
	}

	public List<Officer> GetAllOfficers()
	{
		return officers;
	}

	public List<Officer> GetAllAvailableOfficers()
	{
		List<Officer> retVal = new List<Officer>();
		foreach (Officer officer in officers)
		{
			if (officer.GetIsAvailableForAssignment())
			{
				retVal.Add(officer);
			}
		}
		return retVal;
	}

	public void ReportResolvedCrime(Crime _crime)
	{
		listOfCrimesResolved.Add(_crime);
		// add appropriate amount of cash
		m_cash += (_crime.GetCrimeLevel() + 1) * earned_resolvedCrimeCashPerLevel;
	}

	public void CollectIncomeFromBuildings()
	{
		int total = 0;
		foreach (Structure structure in theCity.GetAllStructures())
		{
			Building building = structure as Building;
			if (building == null) { continue; }
			if (building.GetCondition() == Building.BuildingCondition.Good)
				total += earned_buildingGood;
			else if (building.GetCondition() == Building.BuildingCondition.Medium)
				total += earned_buildingMedium;
			else if (building.GetCondition() == Building.BuildingCondition.Bad)
				total += earned_buildingBad;
		}
		m_cash += total;
	}

	public int GetCash()
	{
		return m_cash;
	}


}
