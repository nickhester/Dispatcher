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
		Raid
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
		public int type;
		public Intersection intersection;
		public Structure structure;

		public RouteNode(Intersection _intersection)
		{
			type = 0;
			intersection = _intersection;
			structure = null;
		}

		public RouteNode(Structure _structure)
		{
			type = 1;
			intersection = null;
			structure = _structure;
		}

		public Vector3 GetPosition()
		{
			return GetObject().transform.position;
		}

		public GameObject GetObject()
		{
			if (type == 0)
				return intersection.gameObject;
			else if (type == 1)
				return structure.gameObject;
			Debug.LogError("Request to get object of bad RouteNode");
			return null;
		}

		public bool IsSame(Intersection _i)
		{
			if (_i == intersection)
				return true;
			return false;
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

	public struct Route
	{
		public List<RouteNode> nodes;
		public float distance;

		public Route(RouteNode _r1, RouteNode _r2)
		{
			nodes = new List<RouteNode>();
			nodes.Add(_r1);
			nodes.Add(_r2);
			distance = Vector3.Distance(_r1.GetPosition(), _r2.GetPosition());
			Debug.DrawLine(_r1.GetPosition(), _r2.GetPosition(), Color.red, 10.0f);
		}

		public Route(Route _r)	// copy constructor
		{
			nodes = new List<RouteNode>();
			nodes.AddRange(_r.nodes);
			distance = _r.distance;
		}

		public void AddNode(RouteNode _r)
		{
			Debug.DrawLine(GetLastNode().GetObject().gameObject.transform.position, _r.GetObject().gameObject.transform.position, Color.blue, 10.0f);
			distance += Vector3.Distance(GetLastNode().GetPosition(), _r.GetPosition());
			nodes.Add(_r);
		}

		public RouteNode GetLastNode()
		{
			return nodes[nodes.Count - 1];
		}

		public override string ToString()
		{
			string retVal = "Route: ";
			foreach (RouteNode node in nodes)
			{
				retVal += node.GetObject().name + ", ";
			}
			return retVal;
		}
	}
}

public class Route
{
	public List<types.RouteNode> nodes;
	public float distance;
	
	public Route(types.RouteNode _r1, types.RouteNode _r2)
	{
		nodes = new List<types.RouteNode>();
		nodes.Add(_r1);
		nodes.Add(_r2);
		distance = Vector3.Distance(_r1.GetPosition(), _r2.GetPosition());
		Debug.DrawLine(_r1.GetPosition(), _r2.GetPosition(), Color.red, 10.0f);
	}
	
	public Route(Route _r)	// copy constructor
	{
		nodes = new List<types.RouteNode>();
		nodes.AddRange(_r.nodes);
		distance = _r.distance;
	}
	
	public void AddNode(types.RouteNode _r)
	{
		Debug.DrawLine(GetLastNode().GetObject().gameObject.transform.position, _r.GetObject().gameObject.transform.position, Color.blue, 10.0f);
		distance += Vector3.Distance(GetLastNode().GetPosition(), _r.GetPosition());
		nodes.Add(_r);
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
