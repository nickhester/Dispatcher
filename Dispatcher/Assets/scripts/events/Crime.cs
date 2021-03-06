﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Crime : Activity {

	public Criminal criminal;
	public Sprite crime_robbery;
	public Sprite crime_violence;
	public Sprite crime_vandalism;

	private types.CrimeType m_crimeType;
	private int m_crimeLevel;
	private float[] crimeLevelSpeedMultiplier = { 1.0f, 1.25f, 1.5f, 1.75f, 2.0f };
	private int crimeIndex;

	public void Initialize(types.CrimeType _crimeType, int _crimeLevel, TimeOnClock _plannedStartTime, int _duration, int _index)
	{
		m_crimeType = _crimeType;
		m_crimeLevel = _crimeLevel;
		plannedStartTime = _plannedStartTime;
		duration = (float)_duration;
		crimeIndex = _index;

		speedMultiplier = Random.Range(0.8f, 1.2f);
	}
	
	public void Update()
	{
		BaseUpdate();
	}

	public types.CrimeType GetCrimeType()
	{
		return m_crimeType;
	}

	public override void Activate(Building _building)
	{
		Sprite _sprite = null;
		if (m_crimeType == types.CrimeType.Robbery)
			_sprite = crime_robbery;
		else if (m_crimeType == types.CrimeType.Violence)
			_sprite = crime_violence;
		else if (m_crimeType == types.CrimeType.Vandalism)
			_sprite = crime_vandalism;

		building = _building;
		// spawn pin
		pin = floatingUIManager.SpawnPin(GetBuilding().transform.position, _sprite);
		floatingUIManager.SubscribeToOnClick(this, pin.transform.GetChild(0).gameObject);

		progressToDuration = theDepot.GetCrimeDiscoveryValue() * duration;

		isActive = true;
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
