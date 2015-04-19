using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Intersection : MonoBehaviour {

	public List<Path> paths = new List<Path>();
	private Neighborhood m_neighborhood;

	public void Initialize(Neighborhood _n)
	{
		m_neighborhood = _n;
	}

	public List<Path> GetPaths()
	{
		return paths;
	}

	public List<Structure> FindAllStructures()
	{
		List<Structure> retVal = new List<Structure>();
		foreach (Path path in paths)
		{
			retVal.AddRange(path.GetStructures());
		}
		return retVal;
	}

	public Neighborhood GetNeighborhood()
	{
		return m_neighborhood;
	}

	public Intersection GetOppositeIntersection(Path _p)
	{
		foreach (Intersection intersection in _p.GetIntersections())
		{
			if (intersection != this)
			{
				return intersection;
			}
		}
		Debug.LogError("couldn't get opposite intersection");
		return null;
	}
}
