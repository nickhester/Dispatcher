using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrimeRing : MonoBehaviour {

	public List<Criminal> criminals = new List<Criminal>();
	private List<Crime> pendingCrimes = new List<Crime>();
	private List<Crime> currentCrimes = new List<Crime>();
	private List<Crime> crimeHistory = new List<Crime>();
	private City theCity;
	private int crimeIndex = 0;


	void Start()
	{
		theCity = GameObject.Find("City").GetComponent<City>();
		Clock.StartGameClock();
	}

	void Update()
	{
		// if no crimes are pending, plan the next one
		//if (currentCrimes.Count == 0 && pendingCrimes.Count == 0)	// use this line for one crime at a time
		if (pendingCrimes.Count < 1)
		{
			PlanNextCrime();
		}

		// check all pending crime to see if it's time to activate
		foreach (Crime pendingCrime in pendingCrimes)
		{
			if (pendingCrime.GetPlannedStartTime().secondsFromStart <= Clock.GetTotalSecondsFromStart())
			{
				ActivateCrime(pendingCrime);
				break;
			}
		}

		// check all current crimes to update time left, and see if they're ready to be completed
		int listLen = currentCrimes.Count;
		for (int i = 0; i < listLen; i++)
		{
			if (currentCrimes[i].CheckIfDurationReached())
			{
				CompleteCrime (currentCrimes[i]);
				i--; listLen--;		// Completing the crime removes it from the list, so step the loop index back
			}
		}
	}

	void PlanNextCrime()
	{
		// query city for appropriate neighborhood to spawn crime
		Neighborhood neighborhood = theCity.CalculateNextCrimeLocation();

		if (neighborhood == null)   // if no buildings are avialable, return
		{
			return;
		}

		// choose crime
		Crime crime = GenerateCrime();
		crime.SetNeighborhood(neighborhood);
		// queue it up
		pendingCrimes.Add(crime);
	}

	Crime GenerateCrime()
	{
		// create a crime
		int chooseCrime = Random.Range(0, 3);
		int chooseTime = Random.Range(1, 6);
		int chooseDuration = Random.Range(10, 20);

		Crime.CrimeType thisCrimeType = Crime.CrimeType.Robbery;
		if (chooseCrime == 1)
			thisCrimeType = Crime.CrimeType.Vandalism;
		else if (chooseCrime == 2)
			thisCrimeType = Crime.CrimeType.Violence;

		GameObject crime_go = new GameObject("crime " + crimeIndex);
		crime_go.transform.SetParent(gameObject.transform);
		Crime crime = crime_go.AddComponent<Crime>() as Crime;
		crime.Initialize(thisCrimeType, 1, new TimeOnClock(true, chooseTime), chooseDuration, crimeIndex);
		crimeIndex++;
		return crime;
	}
	
	void ActivateCrime(Crime _crime)
	{
		// pass crime on to the city
		theCity.ActivateCrime(_crime);
		// remove from pending list
		pendingCrimes.Remove(_crime);
		currentCrimes.Add(_crime);
	}

	public void CompleteCrime(Crime _crime)
	{
		_crime.CompleteCrime();
		// remove from current list
		currentCrimes.Remove(_crime);
		crimeHistory.Add(_crime);
	}

	public void ReceiveResolvedCrime(Crime _crime)
	{
		// remove from current list
		currentCrimes.Remove(_crime);
		crimeHistory.Add(_crime);
	}
}
