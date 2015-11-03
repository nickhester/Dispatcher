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
	private Vector3 mouseDownPos;
	private float mouseDragThreshold = 0.01f;
	private bool isDragging = false;
	private Vector3 mousePosLastFrame;
	private Vector3 mouseDragPoint;
	private Vector3 cameraOriginalPosition;
	Plane ground;

	void Start () {
		ground = new Plane(Vector3.up, Vector3.zero);
		cameraOriginalPosition = Camera.main.transform.position;
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			mouseDownPos = Input.mousePosition;

			// move the camera with the mouse
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float hitDistance;
			ground.Raycast(ray, out hitDistance);
			mouseDragPoint = ray.direction * hitDistance;
		}
		else if (Input.GetMouseButton(0))
		{
			if (isDragging)
			{
				// for each new mouse position
				// move the camera so that the point is under the mouse
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				float hitDistance;
				ground.Raycast(ray, out hitDistance);
				Vector3 newMousePoint = ray.direction * hitDistance;

				Vector3 pointOffset = mouseDragPoint - newMousePoint;
				Vector3 finalPosition = cameraOriginalPosition + new Vector3(pointOffset.x, 0.0f, pointOffset.z);
				Camera.main.transform.position = finalPosition;

			}
			else if (((mouseDownPos - Input.mousePosition).magnitude / Screen.height) > mouseDragThreshold)
			{
				isDragging = true;
			}
		}
		if (isDragging && Input.GetMouseButtonUp(0))
		{
			cameraOriginalPosition = Camera.main.transform.position;
			isDragging = false;
		}
		else if (!isDragging && Input.GetMouseButtonUp(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000.0f))
			{
				// notify of the event
				OnClick(hit.transform.gameObject);
			}
		}
	}
}
