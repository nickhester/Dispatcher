using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Activity : MonoBehaviour
{
	protected Building building;
	protected Neighborhood neighborhood;

	protected TimeOnClock plannedStartTime;
	protected float duration;
	protected float progressToDuration = 0.0f;
	protected GameObject pin;
	private bool isActive = false;
	private FloatingUI floatingUIManager;

	public void Start()
	{
		floatingUIManager = GameObject.Find("FloatingUIManager").GetComponent<FloatingUI>();
	}

	public void Update()
	{
		if (isActive)
		{
			UpdateDuration(Clock.GetDeltaTime());
			pin.transform.GetChild(0).GetComponent<Image>().fillAmount = GetNormalizedProgress();
		}
	}

	public void OnPinClick()
	{
		print ("clicked " + name);
		GameObject.Find ("GameManager").GetComponent<GameManager>().GetGameEvents(this);
	}

	public void Activate(Building _building)
	{
		building = _building;
		// spawn pin
		pin = floatingUIManager.SpawnPin(GetBuilding().transform.position);
		floatingUIManager.SubscribeToOnClick(this, pin);
		isActive = true;
	}

	public void Deactivate()
	{
		// destroy pin
		Destroy (pin);
		isActive = false;
	}

	public void UpdateDuration(float _amount)
	{
		progressToDuration += _amount;
	}
	
	public bool CheckIfDurationReached()
	{
		return (progressToDuration > duration);
	}

	public bool CheckIfDurationEmptied()
	{
		return (progressToDuration < 0.0f);
	}

	float GetNormalizedProgress()
	{
		return (progressToDuration / duration);
	}

	float GetProgress()
	{
		return progressToDuration;
	}

	protected void ShowNotification()
	{
		// add the appropriate graphic
	}

	protected void RemoveNotification()
	{
		// add the appropriate graphic
	}

	protected void RespondToSelection()
	{
		// respond to when the icon is selected
	}

	protected void SetIconCounterState(float _amount)
	{
		// a zero-to-one value of the countdown timer state
	}
	
	public bool GetIsActive()
	{
		return isActive;
	}
	
	public Building GetBuilding()
	{
		return building;
	}
}
