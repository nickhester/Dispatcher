using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Neighborhood : MonoBehaviour {

	public List<Intersection> intersections;
	private List<Structure> allStructures = new List<Structure>();
	private int m_crimeLevel = 0;

	void Start()
	{
		// find all buildings (and set neighborhood)
		foreach (Intersection intersection in intersections)
		{
			foreach (Structure structure in intersection.FindAllStructures())
			{
				if (!allStructures.Contains(structure))
				{
					structure.SetNeighborhood(this);
					allStructures.Add(structure);
				}
			}
		}
	}

	public void UpdateCrimeLevel(int _change)		// a crime has ended, update this neighborhood's crime level for better or worse
	{
		m_crimeLevel += _change;
		if (m_crimeLevel < 0)
			m_crimeLevel = 0;
	}

	void ShutDown()		// this neighborhood's crime has gotten so bad that it needs to be under lockdown for a little while
	{

	}

	void Reopen()		// this neighborhood has been shut down, and is now reopening
	{

	}

	void DiscoverAllBuildings()
	{
		// build list of all buildings from intersection, and then from path
	}

	public List<Structure> GetStructures()
	{
		return allStructures;
	}

	Building FindAvailableBuilding()
	{
		List<int> randomSearchList = new List<int>();
		for (int i = 0; i < allStructures.Count; i++)
		{
			randomSearchList.Add(Random.Range(0, allStructures.Count - 1));
		}
		for (int i = 0; i < allStructures.Count; i++)
		{
			randomSearchList.Add(i);
		}
		foreach (int i in randomSearchList)
		{
			Building building = allStructures[i] as Building;
			if (building == null) { continue; }
			if (!building.IsCrimeOccuring())
			{
				return building;
			}
		}
		//Debug.LogError("Neighborhood could not find Bldg w/o crime");
		return null;
	}

	public bool CheckIfBuildingsAvailable()
	{
		if (FindAvailableBuilding() != null)
		{
			return true;
		}
		return false;
	}

	public void ActivateCrime(Crime _crime)
	{
		// activate the crime by passing it to the building
		Building building = FindAvailableBuilding();
		_crime.Activate(building);
		building.SetCurrentCrime(_crime);
	}

	public List<Intersection> GetIntersections()
	{
		return intersections;
	}
}
