using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {


	// event handler
	public delegate void OnClickEvent(GameObject go);
	public event OnClickEvent OnClick;
	// singleton
	private static InputManager instance;
	// constructor
	private InputManager() {}
	// instance
	public static InputManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType(typeof(InputManager)) as InputManager;
			}
			return instance;
		}
	}

	void Start () {
	}

	void Update ()
	{
		if (Input.GetMouseButtonUp(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 100.0f))
			{
				// notify of the event
				OnClick(hit.transform.gameObject);
			}
		}
	}
}
