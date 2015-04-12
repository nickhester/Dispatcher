using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FloatingUI : MonoBehaviour {

	public GameObject pin;
	public GameObject canvas;
	private float PinHeight = 40.0f;

	public GameObject SpawnPin(Vector3 _loc)
	{
		GameObject instance = Instantiate(pin) as GameObject;
		instance.transform.SetParent(canvas.transform, false);
		Vector3 screenSpacePoint = Camera.main.WorldToScreenPoint(_loc);
		Vector2 pointConverted = worldPointToCanvas(screenSpacePoint);
		instance.GetComponent<RectTransform>().localPosition = pointConverted;
		return instance;
	}

	Vector2 worldPointToCanvas(Vector3 _v)
	{
		return new Vector2(_v.x - (Screen.width/2), _v.y - (Screen.height/2) + PinHeight);
	}

	/*
	public GameObject SpawnHead(Vector3 _loc)
	{
		//
	}
	*/

	// subscribe the Activity to the element's clicks
	public void SubscribeToOnClick(Activity _caller, GameObject _UI)
	{
		Button b = _UI.GetComponent<Button>();
		b.onClick.AddListener(() => _caller.OnPinClick());
	}
}
