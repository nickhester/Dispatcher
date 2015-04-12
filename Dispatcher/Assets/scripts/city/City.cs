using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class City : MonoBehaviour {

	public List<Neighborhood> neighborhoods = new List<Neighborhood>();
	public Depot depot;
	public CrimeRing crimeRing;

	public Neighborhood CalculateNextCrimeLocation()
	{
		// where will the next crime be?

		// currently random; later weight by crime level
		int randomlyChoosenNeighborhood = Random.Range(0, neighborhoods.Count);
		if (neighborhoods[randomlyChoosenNeighborhood].CheckIfBuildingsAvailable())
		{
			return neighborhoods[randomlyChoosenNeighborhood];
		}

		// if the random neighborhood has no buildings, then just find any building
		foreach (Neighborhood neighborhood in neighborhoods)
		{
			if (neighborhood.CheckIfBuildingsAvailable())
			{
				return neighborhood;
			}
		}
		return null;
	}

	public void ActivateCrime(Crime _crime)
	{
		// pass crime on to neighborhood
		_crime.GetNeighborhood().ActivateCrime(_crime);
	}

	public List<Neighborhood> GetNeighborhoods()
	{
		return neighborhoods;
	}

	public List<Building> GetAllBuildings()
	{
		List<Building> retVal = new List<Building>();
		foreach (Neighborhood neighborhood in neighborhoods)
		{
			retVal.AddRange(neighborhood.GetBuildings());
		}
		return retVal;
	}
}
