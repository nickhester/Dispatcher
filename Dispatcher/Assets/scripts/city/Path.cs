using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path : MonoBehaviour {

	public List<Structure> structures = new List<Structure>();
	private List<Intersection> intersections = new List<Intersection>();

	public void Initialize(Intersection _i)
	{
		if (!intersections.Contains(_i))
			intersections.Add(_i);
	}

	public List<Structure> GetStructures()
	{
		return structures;
	}

	public List<Intersection> GetIntersections()
	{
		return intersections;
	}
}
