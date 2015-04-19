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

		foreach (Intersection intersection in _origin.GetPath().GetIntersections())
		{
			types.RouteNode routeNodeStart = new types.RouteNode(_origin);
			types.RouteNode routeNodeEnd = new types.RouteNode(intersection);
			Route route = new Route(routeNodeStart, routeNodeEnd);
			possibleRoutes.Add(route);

			checkedNodes.Add(routeNodeEnd);
		}

		int loopTimeOut = 0;
		while (loopTimeOut < 1000)
		{
			MonoBehaviour.print ("NEW LOOP");
			foreach (Route item in possibleRoutes)
			{
				MonoBehaviour.print(item);
			}

			// current shortest path
			float currentShortestDistance = 0.0f;
			int currentShortestRoute = -1;

			string distanceCompare = "DIST: ";
			for (int i = 0; i < possibleRoutes.Count; i++)
			{
				distanceCompare += i + " " + possibleRoutes[i] + ":" + possibleRoutes[i].distance + ", ";
				if (currentShortestRoute == -1 || possibleRoutes[i].distance < currentShortestDistance)
				{
					currentShortestDistance = possibleRoutes[i].distance;
					currentShortestRoute = i;
				}
			}
			MonoBehaviour.print (distanceCompare);

			Route shortestRoute = possibleRoutes[currentShortestRoute];

			// if the current shortest route ends in the destination, then return that route
			types.RouteNode n = shortestRoute.GetLastNode();
			if (shortestRoute.GetLastNode().IsSame(_destination))
			{
				MonoBehaviour.print ("FOUND DESTINATION!");
				return shortestRoute;
			}

			// take the shortest route found, and check to see if it has path branches
			List<Path> pathsFromThisIntersection = shortestRoute.GetLastNode().intersection.GetPaths();
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
				MonoBehaviour.print("NEW ROUTE LOOP for: " + routeClones[i]);

				// check for destination on current path
				Path thisPath = pathsFromThisIntersection[i];
				if (thisPath.GetStructures().Contains(_destination))
				{
					MonoBehaviour.print("adding destination: " + _destination + " to " + routeClones[i]);
					routeClones[i].AddNode(new types.RouteNode(_destination));
					possibleRoutes.Add (routeClones[i]);
				}
				else   // otherwise add the next intersection to the route
				{
					types.RouteNode newRouteNode = new types.RouteNode(routeClones[i].GetLastNode().intersection.GetOppositeIntersection(thisPath));
					MonoBehaviour.print("checking intersection: " + newRouteNode);

					bool foundDuplicate = false;
					foreach (types.RouteNode node in checkedNodes)
					{
						// if it's an intersection that's already been found don't add the route
						if (node.intersection == newRouteNode.intersection)
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
						string cnl = "";
						foreach (var item in checkedNodes)
						{
							cnl += item;
						}
						MonoBehaviour.print ("checked nodes: " + cnl + ", ");

						MonoBehaviour.print("adding intersection: " + newRouteNode + " to " + routeClones[i]);
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
