using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path : MonoBehaviour {

	public List<Building> buildings = new List<Building>();

	public List<Building> GetBuildings()
	{
		return buildings;
	}
}
