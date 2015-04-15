using UnityEngine;
using System.Collections;

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
}
