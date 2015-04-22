using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinder {

	private City city;
	private List<types.RouteNode> checkedNodes;


	public Route FindPath(Structure _origin, Structure _destination)
	{
		// TODO: need to first check to see if on same street as destination, and can just early out and travel straight there!

		checkedNodes = new List<types.RouteNode>();
		List<Route> possibleRoutes = new List<Route>();

		// check to see if you're starting on an intersection already
		Intersection _originIntersection = _origin as Intersection;
		if (_originIntersection != null)
		{
			types.RouteNode routeNodeStart = new types.RouteNode(_origin);
			Route route = new Route(routeNodeStart);
			possibleRoutes.Add(route);
			
			checkedNodes.Add(routeNodeStart);
		}
		else
		{
			foreach (Intersection intersection in _origin.GetPath().GetIntersections())
			{
				types.RouteNode routeNodeStart = new types.RouteNode(_origin);
				types.RouteNode routeNodeEnd = new types.RouteNode(intersection);
				Route route = new Route(routeNodeStart, routeNodeEnd);
				possibleRoutes.Add(route);

				checkedNodes.Add(routeNodeEnd);
			}
		}

		int loopTimeOut = 0;
		while (loopTimeOut < 1000)
		{
			// current shortest path
			float currentShortestDistance = 0.0f;
			int currentShortestRoute = -1;

			string distanceCompare = "DIST: ";
			for (int i = 0; i < possibleRoutes.Count; i++)
			{
				distanceCompare += i + " " + possibleRoutes[i] + ":" + possibleRoutes[i].GetDistance() + ", ";
				if (currentShortestRoute == -1 || possibleRoutes[i].GetDistance() < currentShortestDistance)
				{
					currentShortestDistance = possibleRoutes[i].GetDistance();
					currentShortestRoute = i;
				}
			}

			Route shortestRoute = possibleRoutes[currentShortestRoute];

			// if the current shortest route ends in the destination, then return that route
			types.RouteNode n = shortestRoute.GetLastNode();
			if (shortestRoute.GetLastNode().IsSame(_destination))
			{
				return shortestRoute;
			}

			// take the shortest route found, and check to see if it has path branches
			List<Path> pathsFromThisIntersection = ((Intersection)shortestRoute.GetLastNode().structure).GetPaths();
			// create a duplicate of the route for each possible path it could take
			List<Route> routeClones = new List<Route>();
			foreach (Path path in pathsFromThisIntersection)
			{
				Route routeClone = new Route(shortestRoute);
				routeClones.Add(routeClone);
			}
			// delete the original, since one of its clone will count for it
			possibleRoutes.Remove(shortestRoute);
			// check each route option
			for (int i = 0; i < routeClones.Count; i++)
			{
				// check for destination on current path
				Path thisPath = pathsFromThisIntersection[i];
				if (thisPath.GetStructures().Contains(_destination))
				{
					routeClones[i].AddNode(new types.RouteNode(_destination));
					possibleRoutes.Add (routeClones[i]);
				}
				else   // otherwise add the next intersection to the route
				{
					Structure _intersection = ((Intersection)routeClones[i].GetLastNode().structure).GetOppositeIntersection(thisPath);
					types.RouteNode newRouteNode = new types.RouteNode(_intersection);

					bool foundDuplicate = false;
					foreach (types.RouteNode node in checkedNodes)
					{
						// if it's an intersection that's already been found don't add the route
						if (node.structure == newRouteNode.structure)
						{
							foundDuplicate = true;
						}
					}

					if (foundDuplicate)
					{
						// don't add route back to possible list
					}
					else
					{
						checkedNodes.Add(newRouteNode);
						routeClones[i].AddNode(newRouteNode);
						possibleRoutes.Add (routeClones[i]);
					}
				}
			}
			loopTimeOut++;
		}
		Debug.LogError("no possible route found");
		return null;
	}


	float FindDistance(Structure _start, Structure _end)
	{
		Vector3 _s = _start.gameObject.transform.position;
		Vector3 _e = _end.gameObject.transform.position;
		return Vector3.Distance(_s, _e);
	}
}
