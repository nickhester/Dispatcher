using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BaseUI : MonoBehaviour {

	protected Vector3 SpaceOutElements(int _numElementsTotal, int _elementIndex, float _horizontalSpacing)
	{
		Vector3 loc = Vector3.up;
		float spacing = (_horizontalSpacing * _elementIndex) - ((_horizontalSpacing * ((float)_numElementsTotal - 1)) / 2.0f);
		loc += Vector3.right * spacing;
		return loc;
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

	protected GameObject SpawnObjectWithSprite(GameObject _obj, Sprite _sprite, int _levelsDeep)
	{
		GameObject retVal = Instantiate(_obj) as GameObject;
		if (_levelsDeep == 0)
			retVal.GetComponent<Image>().sprite = _sprite;
		else if (_levelsDeep == 1)
			retVal.transform.GetChild(0).GetComponent<Image>().sprite = _sprite;
		else if (_levelsDeep == 2)
			retVal.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = _sprite;
		return retVal;
	}
}
