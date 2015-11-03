//#define FAST_PROGRESS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrimeRing : MonoBehaviour {

	public List<Criminal> criminals = new List<Criminal>();
	public Sprite[] criminalIcons;
	public GameObject crime;
	private List<Crime> pendingCrimes = new List<Crime>();
	private List<Crime> currentCrimes = new List<Crime>();
	private List<Crime> crimeHistory = new List<Crime>();
	private List<Crime> crimesCompleted = new List<Crime>();
	private City theCity;
	private int crimeIndex = 0;
	private int currentCrimeLevel = 0;

#if FAST_PROGRESS

	private int[] crimeCountLevelUp = { 5, 10, 15, 20 };	// for every n crimes, crimes are up one level

#else

	private int[] crimeCountLevelUp = { 8, 16, 24, 32, 40, 48 };	// for every n crimes, crimes are up one level

#endif

	void Start()
	{
		theCity = GameObject.Find("City").GetComponent<City>();
		Clock.StartGameClock();
	}

	void Update()
	{
		// if no crimes are pending, plan the next one
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

		// HACK: unlock a new neighborhood periodically
		if (Clock.GetCurrentDay() == 10)
		{
			theCity.EnableNextNeighborhood();
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

#if FAST_PROGRESS
		int chooseTime = Random.Range(2, 4);
		int chooseDuration = Random.Range(10, 15);
#else
		int chooseTime = Random.Range(5, 15);
		int chooseDuration = Random.Range(30, 45);
#endif
		if (crimeIndex == 0)  // let the first crime start after 5 seconds
			chooseTime = 5;

		// determine crime level
		if (currentCrimeLevel < crimeCountLevelUp.Length)
		{
			if (crimeIndex == crimeCountLevelUp[currentCrimeLevel])
			{
				currentCrimeLevel++;
			}
		}

		types.CrimeType thisCrimeType = types.CrimeType.Robbery;
		if (chooseCrime == 1)
			thisCrimeType = types.CrimeType.Vandalism;
		else if (chooseCrime == 2)
			thisCrimeType = types.CrimeType.Violence;

		GameObject crime_go = Instantiate(crime);
		crime_go.name = "crime " + crimeIndex;
		crime_go.transform.SetParent(gameObject.transform);
		crime_go.GetComponent<Crime>().Initialize(thisCrimeType, currentCrimeLevel, new TimeOnClock(true, chooseTime), chooseDuration, crimeIndex);
		crimeIndex++;
		return crime_go.GetComponent<Crime>();
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
		crimesCompleted.Add (_crime);
	}

	public int GetNumCrimesCompleted()
	{
		return crimesCompleted.Count;
	}

	public void ReceiveResolvedCrime(Crime _crime)
	{
		// remove from current list
		currentCrimes.Remove(_crime);
		crimeHistory.Add(_crime);
	}
}
