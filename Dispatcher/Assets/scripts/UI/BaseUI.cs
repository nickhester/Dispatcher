using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BaseUI : MonoBehaviour {

	protected Vector3 SpaceOutElements(int _numElementsTotal, int _elementIndex, float _horizontalSpacing, bool _isHorizontal)
	{
		Vector3 loc = Vector3.up;
		float spacing = (_horizontalSpacing * _elementIndex) - ((_horizontalSpacing * ((float)_numElementsTotal - 1)) / 2.0f);
		loc += Vector3.right * spacing;
		if (!_isHorizontal)
		{
			float x = loc.x;
			float y = loc.y;
			loc.x = y;
			loc.y = x;
		}
		return loc;
	}

	protected Vector3 SpaceOutElements(int _numElementsTotal, int _elementIndex, float _horizontalSpacing)
	{
		return SpaceOutElements(_numElementsTotal, _elementIndex, _horizontalSpacing, true);
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

	protected GameObject SpawnObjectWithSprite(GameObject _obj, Sprite _sprite, string _tagName)
	{
		GameObject retVal = Instantiate(_obj) as GameObject;
		GameObject targetObject = retVal;
		if (targetObject.tag == _tagName)
		{
			targetObject = retVal;
		}
		else
		{
			foreach (Transform transform in targetObject.GetComponentsInChildren<Transform>())
			{
				if (transform.tag == _tagName)
				{
					targetObject = transform.gameObject;
					break;
				}
			}
		}

		targetObject.GetComponent<Image>().sprite = _sprite;
		return retVal;
	}

	protected void UpdateText(GameObject _obj, string text, string _objectName)
	{
		GameObject targetObject = null;
		if (_obj.name == _objectName)
		{
			targetObject = _obj;
		}
		else
		{
			foreach (Transform transform in _obj.GetComponentsInChildren<Transform>())
			{
				if (transform.name == _objectName)
				{
					targetObject = transform.gameObject;
					break;
				}
			}
		}
		targetObject.GetComponent<Text>().text = text;
	}

	protected void EnableElementWithTag(GameObject _obj, string _tag)
	{
		foreach (Image imageComponent in _obj.GetComponentsInChildren<Image>())
		{
			if (imageComponent.tag == _tag)
			{
				imageComponent.enabled = true;
				break;
			}
		}
	}
}
