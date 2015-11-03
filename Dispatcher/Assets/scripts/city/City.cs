using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class City : MonoBehaviour {

	public List<Neighborhood> neighborhoods = new List<Neighborhood>();
	private int neighborhoodsEnabled = 0;
	public Depot depot;
	public CrimeRing crimeRing;
	public Pathfinder pathfinder;

	void Awake()
	{
		int neighborhoodCount = 0;
		foreach (GameObject neighborhood in GameObject.FindGameObjectsWithTag("Neighborhood"))
		{
			neighborhoodCount++;
		}
		if (neighborhoodCount != neighborhoods.Count)
			Debug.LogError("===CITY NOT AWARE OF ALL NEIGHBORHOODS===");

		// build navgraph of city
		foreach (Neighborhood neighborhood in neighborhoods)
		{
			foreach (Intersection intersection in neighborhood.GetIntersections())
			{
				intersection.Initialize(neighborhood);

				foreach (Path path in intersection.GetPaths())
				{
					path.Initialize(intersection);

					foreach (Structure structure in path.GetStructures())
					{
						structure.Initialize(path);
					}
				}
			}
		}
		pathfinder = new Pathfinder();

		EnableNextNeighborhood();
	}

	public void EnableNextNeighborhood()
	{
		neighborhoodsEnabled++;
		if (neighborhoods.Count > neighborhoodsEnabled)
			neighborhoods[neighborhoodsEnabled].Activate();
	}

	public Neighborhood CalculateNextCrimeLocation()
	{
		// where will the next crime be?

		// currently random; later weight by crime level
		bool hasFoundNeighborhood = false;
		while (hasFoundNeighborhood)
		{
			int randomlyChoosenNeighborhood = Random.Range(0, neighborhoods.Count);
			if (neighborhoods[randomlyChoosenNeighborhood].GetIsActive()
			    && neighborhoods[randomlyChoosenNeighborhood].CheckIfBuildingsAvailable())
			{
				return neighborhoods[randomlyChoosenNeighborhood];
			}
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

	public List<Structure> GetAllStructures()
	{
		List<Structure> retVal = new List<Structure>();
		foreach (Neighborhood neighborhood in neighborhoods)
		{
			retVal.AddRange(neighborhood.GetStructures());
		}
		return retVal;
	}


}
