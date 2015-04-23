using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FloatingUI : BaseUI {

	public GameObject pin;
	private float PinHeight = 1.0f;
	private float headHeight = 200.0f;
	private float headSpacingHorizontal = 200.0f;
	public List<GameObject> currentHeads = new List<GameObject>();

	public GameObject SpawnPin(Vector3 _loc)
	{
		GameObject instance = Instantiate(pin) as GameObject;
		_loc += instance.transform.up * PinHeight;
		instance.transform.position = _loc;
		return instance;
	}
	
	public List<GameObject> SpawnHeads(Vector3 _loc, List<Officer> _officers)
	{
		ClearCurrentHeads();
		List<GameObject> retVal = new List<GameObject>();
		for (int i = 0; i < _officers.Count; i++)
		{
			GameObject instance = Instantiate(_officers[i].GetMyHead()) as GameObject;
			instance.transform.position = (instance.transform.up * headHeight) + SpaceOutElements(_officers.Count, i, headSpacingHorizontal);
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

	// subscribe the object to the element's clicks
	public void SubscribeToOnClick(Object _caller, GameObject _UI)
	{
		Activity caller_activity = _caller as Activity;
		Officer caller_officer = _caller as Officer;

		Button b = _UI.GetComponent<Button>();

		if (caller_activity != null)
			b.onClick.AddListener(() => caller_activity.OnPinClick());
		else if (caller_officer != null)
			b.onClick.AddListener(() => caller_officer.OnHeadClick());
	}
}
