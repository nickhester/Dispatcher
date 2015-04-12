using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Intersection : MonoBehaviour {

	public List<Path> paths = new List<Path>();

	public List<Path> GetPaths()
	{
		return paths;
	}

	public List<Building> FindAllBuildings()
	{
		List<Building> retVal = new List<Building>();
		foreach (Path path in paths)
		{
			retVal.AddRange(path.GetBuildings());
		}
		return retVal;
	}
}
