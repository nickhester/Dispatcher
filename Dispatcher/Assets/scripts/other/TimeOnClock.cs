using UnityEngine;
using System.Collections;

public class TimeOnClock {
	
	public float secondsFromStart;

	public TimeOnClock(bool isTimeFromNow, float _secondsFromNow)
	{
		if (isTimeFromNow)
			secondsFromStart = _secondsFromNow + Clock.GetTotalSecondsFromStart();
		else
			secondsFromStart = _secondsFromNow;
	}

	public TimeOnClock(bool isTimeFromNow, int _secondsFromNow)
	{
		if (isTimeFromNow)
			secondsFromStart = (float)_secondsFromNow + Clock.GetTotalSecondsFromStart();
		else
			secondsFromStart = (float)_secondsFromNow;
	}
}
