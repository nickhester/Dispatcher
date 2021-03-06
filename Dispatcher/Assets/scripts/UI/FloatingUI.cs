﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FloatingUI : BaseUI {

	public GameObject pin;
	private float PinHeight = 20.0f;
	private float PinForward = 10.0f;
	private float headHeight = 200.0f;
	private float headSpacingHorizontal = 200.0f;
	public List<GameObject> currentHeads = new List<GameObject>();

	public GameObject SpawnPin(Vector3 _loc, Sprite _sprite)
	{
		GameObject instance = SpawnObjectWithSprite(pin, _sprite, "UI_pinCrime");
		_loc += instance.transform.up * PinHeight;
		_loc += instance.transform.forward * -PinForward;
		instance.transform.position = _loc;

		return instance;
	}
	
	public List<GameObject> SpawnHeads(Vector3 _loc, List<Officer> _officers)
	{
		ClearCurrentHeads();
		List<GameObject> retVal = new List<GameObject>();
		for (int i = 0; i < _officers.Count; i++)
		{
			GameObject instance = SpawnObjectWithSprite(_officers[i].GetMyHead(), _officers[i].GetMySprite(), "UI_headOfficer");
			instance.transform.position = (instance.transform.up * headHeight) + SpaceOutElements(_officers.Count, i, headSpacingHorizontal);
			// enable officer state
			if (_officers[i].GetCurrentCrimeIsActive())
			{
				EnableElementWithTag(instance, "UI_officerState");
			}
			retVal.Add(instance);
			currentHeads.Add(instance);
		}
		return retVal;
	}

	public void ClearCurrentHeads()
	{
		foreach (GameObject head in currentHeads)
		{
			Destroy(head);
		}
	}
}
