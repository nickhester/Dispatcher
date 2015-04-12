using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Crime : Activity {

	public Criminal criminal;

	public enum CrimeType
	{
		Robbery,
		Violence,
		Vandalism
	}
	private CrimeType m_crimeType;
	private int m_crimeLevel;
	private int crimeIndex;

	public void Initialize(CrimeType _crimeType, int _crimeLevel, TimeOnClock _plannedStartTime, int _duration, int _index)
	{
		m_crimeType = _crimeType;
		m_crimeLevel = _crimeLevel;
		plannedStartTime = _plannedStartTime;
		duration = (float)_duration;
		crimeIndex = _index;
	}

	public void ResolveCrime()	// a crime is successfully resolved by officers
	{
		Deactivate();
		building.ResolveCurrentCrime();
		GameObject.Find ("CrimeRing").GetComponent<CrimeRing>().ReceiveResolvedCrime(this);
		RemoveNotification();
	}

	public void CompleteCrime()	// a crime is successfully completed by the crimeRing
	{
		Deactivate();
		building.CompleteCurrentCrime();
		RemoveNotification();
	}

	public void SetNeighborhood(Neighborhood _neighborhood)
	{
		neighborhood = _neighborhood;
	}

	public Neighborhood GetNeighborhood()
	{
		return neighborhood;
	}

	public TimeOnClock GetPlannedStartTime()
	{
		return plannedStartTime;
	}

	public int GetCrimeLevel()
	{
		return m_crimeLevel;
	}
}
