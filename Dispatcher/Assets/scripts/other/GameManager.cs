using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	private CrimeRing crimeRing;
	private Depot depot;

	private Crime currentCrimeSelected;
	private int currentDay = 0;

	public enum GameState
	{
		Map,
		Map_crimeSelected_waitingForOfficer,
		Map_officerSelected_waitingForCrime,
	}
	private GameState gameState = GameState.Map;

	void Start()
	{
		crimeRing = GameObject.Find ("CrimeRing").GetComponent<CrimeRing>();
		depot = GameObject.Find ("Depot").GetComponent<Depot>();
	}

	void Update()
	{
		if (gameState == GameState.Map_crimeSelected_waitingForOfficer)
		{
			if (!currentCrimeSelected.GetIsActive())	// if the crime ends before an officer is selected...
			{
				gameState = GameState.Map;
			}
		}

		// pay out building value at each new day
		if (Clock.GetCurrentDay() != currentDay)
		{
			currentDay = Clock.GetCurrentDay();
			depot.CollectIncomeFromBuildings();
		}
	}

	public void GetGameEvents(Object obj)
	{
		// cast to get incoming type
		Officer sender_officer = obj as Officer;
		Crime sender_crime = obj as Crime;

		if (gameState == GameState.Map)
		{
			if (sender_crime != null)
			{
				SelectCrime(sender_crime);
			}
		}
		else if (gameState == GameState.Map_crimeSelected_waitingForOfficer)
		{
			if (sender_officer != null)
			{
				depot.SendOfficerRequest(sender_officer, currentCrimeSelected);
				currentCrimeSelected = null;
				gameState = GameState.Map;
			}
			else if (sender_crime != null)
			{
				SelectCrime(sender_crime);
			}
		}
		else if (gameState == GameState.Map_officerSelected_waitingForCrime)
		{
			// this workflow (selecting officer then crime) is not yet created
		}
	}

	void SelectCrime(Object _sender_crime)
	{
		if (((Crime)_sender_crime).GetIsActive())
		{
			currentCrimeSelected = ((Crime)_sender_crime);
			gameState = GameState.Map_crimeSelected_waitingForOfficer;
		}
	}

	void OnGUI()
	{
		GUI.Box (new Rect (10,10,100,90), "timer: " + (int)Clock.GetTotalSecondsFromStart()
		         + "\nday: " + Clock.GetCurrentDay()
		         + "\ncash: " + depot.GetCash());
	}
}
