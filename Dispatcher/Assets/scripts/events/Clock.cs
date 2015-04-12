using UnityEngine;
using System.Collections;

public static class Clock {

	private static float GameTimeOffset;
	private static int numSecondsInDay = 10;
	private static float accumulatedPauseTime = 0.0f;
	private static float pauseTime = 0.0f;
	private static bool isPaused = false;

	public static void StartGameClock()
	{
		GameTimeOffset = Time.time;
	}

	public static float GetTotalSecondsFromStart()
	{
		if (isPaused)
		{
			UpdatePauseTime();
		}

		return Time.time - GameTimeOffset - accumulatedPauseTime;
	}

	public static int GetCurrentDay()
	{
		return ((int)GetTotalSecondsFromStart()) / numSecondsInDay;
	}
	
	public static float GetSecondsThisDay()
	{
		return GetTotalSecondsFromStart() - (float)(GetCurrentDay() * numSecondsInDay);
	}

	public static void PauseClock()
	{
		isPaused = true;
		pauseTime = Time.time;
	}

	public static void ResumeClock()
	{
		isPaused = false;
		UpdatePauseTime();
	}

	private static void UpdatePauseTime()
	{
		accumulatedPauseTime += Time.time - pauseTime;
		pauseTime = Time.time;
	}

	public static float GetDeltaTime()
	{
		if (isPaused)
		{
			return 0.0f;
		}
		else
		{
			return Time.deltaTime;
		}
	}
}
