using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class types {

	public enum DestinationType
	{
		Depot,
		Crime,
		Tip,
		Raid,
		Building
	}

	public enum OfficerState
	{
		isAtDepot,
		isTravelling_interruptible,
		isTravelling_uninterruptible,
		isAtCrime,
	}

	public enum CrimeType
	{
		Robbery,
		Violence,
		Vandalism
	}

	public struct RouteNode
	{
		public Structure structure;

		public RouteNode(Structure _structure)
		{
			structure = _structure;
		}

		public Vector3 GetPosition()
		{
			return GetObject().transform.position;
		}

		public GameObject GetObject()
		{
			return structure.gameObject;
		}

		public bool IsSame(Structure _s)
		{
			if (_s == structure)
				return true;
			return false;
		}

		public override string ToString()
		{
			return GetObject().name;
		}
	}
}

public class OfficerTask
{
	private Structure location;
	private Activity activity;
	private int type = -1;

	public OfficerTask(Activity _a)
	{
		activity = _a;

		Crime c = _a as Crime;
		//Tip t = _a as Tip;
		//Raid r = _a as Raid;

		if (c != null)
		{
			type = 0;
			location = c.GetBuilding();
		}
	}

	public OfficerTask(Structure _l)
	{
		location = _l;

		Depot d = _l as Depot;
		Building b = _l as Building;

		if (d != null)
		{
			type = 1;
		}
		else if (b != null)
		{
			type = 2;
		}
	}

	public int GetTaskType()
	{
		return type;
	}

	public Structure GetLocation()
	{
		return location;
	}

	public Activity GetActivity()
	{
		return activity;
	}
}

public class Route
{
	public List<types.RouteNode> nodes;
	public List<float> segmentLengths = new List<float>();
	
	public Route(types.RouteNode _r1, types.RouteNode _r2)
	{
		nodes = new List<types.RouteNode>();
		nodes.Add(_r1);
		nodes.Add(_r2);
		segmentLengths.Add(Vector3.Distance(_r1.GetPosition(), _r2.GetPosition()));
	}

	public Route(types.RouteNode _r1)
	{
		nodes = new List<types.RouteNode>();
		nodes.Add(_r1);
	}

	public void DrawRouteDebugLine()
	{
		for (int i = 0; i < nodes.Count - 1; i++)
		{
			Debug.DrawLine(nodes[i].GetPosition(), nodes[i + 1].GetPosition(), Color.red, 5.0f);
		}
	}
	
	public Route(Route _r)	// copy constructor
	{
		nodes = new List<types.RouteNode>();
		nodes.AddRange(_r.nodes);
		segmentLengths = new List<float>();
		segmentLengths.AddRange(_r.segmentLengths);
	}
	
	public void AddNode(types.RouteNode _r)
	{
		float dist = Vector3.Distance(GetLastNode().GetPosition(), _r.GetPosition());
		segmentLengths.Add(dist);
		nodes.Add(_r);
	}

	public float GetDistance()
	{
		float retVal = 0.0f;
		foreach (var dist in segmentLengths)
		{
			retVal += dist;
		}
		return retVal;
	}
	
	public types.RouteNode GetLastNode()
	{
		return nodes[nodes.Count - 1];
	}
	
	public override string ToString()
	{
		string retVal = "Route: ";
		foreach (types.RouteNode node in nodes)
		{
			retVal += node.GetObject().name + ", ";
		}
		return retVal;
	}
}
