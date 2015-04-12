using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : Structure {

	private Crime currentCrime;
	public enum BuildingCondition 
	{
		Good,
		Medium,
		Bad
	}
	private BuildingCondition m_buildingCondition;
	private int buildingConditionValue = 0;
	private int buildingConditionValue_Medium = 1;
	private int buildingConditionValue_Bad = 2;

	void Start()
	{
		DisplayNormalState();
	}

	void DisplayCrime()
	{
		// activate visual crime state
		Renderer r = gameObject.GetComponent<Renderer>();
		r.material.color = Color.red;
	}

	void DisplayNormalState()
	{
		// de-activate visual crime state, and set new display condition state
		Renderer r = gameObject.GetComponent<Renderer>();
		if (m_buildingCondition == BuildingCondition.Good)
			r.material.color = Color.green;
		else if (m_buildingCondition == BuildingCondition.Medium)
			r.material.color = Color.blue;
		else if (m_buildingCondition == BuildingCondition.Bad)
			r.material.color = Color.black;
	}

	void DisplayConditionState(BuildingCondition _condition)
	{

	}

	public BuildingCondition GetCondition()
	{
		return m_buildingCondition;
	}

	void AdjustCondition(int adjustBy)	// +1 is worse, -1 is an improvement
	{
		// update condition level
		buildingConditionValue += adjustBy;
		if (buildingConditionValue < 0)
			buildingConditionValue = 0;
		else if (buildingConditionValue > buildingConditionValue_Bad)
			buildingConditionValue = buildingConditionValue_Bad;

		if (buildingConditionValue >= buildingConditionValue_Bad)
			m_buildingCondition = BuildingCondition.Bad;
		else if (buildingConditionValue >= buildingConditionValue_Medium)
			m_buildingCondition = BuildingCondition.Medium;
		else
			m_buildingCondition = BuildingCondition.Good;
	}

	public bool IsCrimeOccuring()
	{
		if (currentCrime == null)
		{
			return false;
		}
		return true;
	}

	public Crime GetCurrentCrime()
	{
		return currentCrime;
	}

	public void SetCurrentCrime(Crime _crime)
	{
		currentCrime = _crime;
		DisplayCrime();
	}

	public void CompleteCurrentCrime()
	{
		currentCrime = null;
		AdjustCondition(1);
		DisplayNormalState();
		GetNeighborhood().UpdateCrimeLevel(1);
	}

	public void ResolveCurrentCrime()
	{
		currentCrime = null;
		AdjustCondition(-1);
		DisplayNormalState();
		GetNeighborhood().UpdateCrimeLevel(-1);
	}
}
