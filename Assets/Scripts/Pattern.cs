using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern : MonoBehaviour {
	[HideInInspector]
	public int teamID;
	
	[Range(0.5f, 5.0f)]
	public float timeBetweenNotes;
	private float initialTimeBetweenNotes;

	public int patternSize = 10;

	[Range(0.01f, 1.0f)]
	public float intensityIncreasePercentage = 0.1f;

	public bool isPlayingPattern;

	private List<InputValues> currentPattern = new List<InputValues>();	
	private int currentPositionInPattern = 0;
	private bool waitingForValue;
	private bool correctInput;
	
	private RumbleArgs rumbleArgument = new RumbleArgs();
	private PatternArgs patternArgument = new PatternArgs();

	public void Initialize()
	{
		initialTimeBetweenNotes = timeBetweenNotes;
		patternArgument.teamID = teamID;
		rumbleArgument.teamID = teamID;
	}

	public List<InputValues> GetCurrentPattern()
	{
		return currentPattern;
	}

	public void ResetIntensity()
	{
		timeBetweenNotes = initialTimeBetweenNotes;
	}

	public void IncreaseIntensity()
	{
		float temp = timeBetweenNotes - timeBetweenNotes * intensityIncreasePercentage;
		if (temp < 0.5f)
		{
			timeBetweenNotes = 0.5f;
		}
		else
		{
			timeBetweenNotes = temp;
		}
	}

	public void GenerateNewPattern()
	{
		currentPattern.Clear();

		for (int i = 0; i < patternSize; i++)
		{
			int value = Random.Range(0, (int)(InputValues.Count));
			currentPattern.Add((InputValues)value);
		}
	}

	public void InputValue(InputValues input)
	{
		if (!waitingForValue)
		{
			return;
		}

		correctInput = currentPattern[currentPositionInPattern] == input;
		
		if (correctInput)
		{
			GameEventHandler.TriggerEvent(GameEvent.NoteSuccess, patternArgument);
		}

		waitingForValue = false;
	}

	public void CallVibration(InputValues type)
	{
		rumbleArgument.vibrationType = type;
		GameEventHandler.TriggerEvent(GameEvent.Rumble, rumbleArgument);
	}

	public void StartPlayPatternCoroutine()
	{
		StartCoroutine(PlayPattern());
	}

	public IEnumerator PlayPattern()
	{
		Debug.Log("Pattern Coroutine Started");
		isPlayingPattern = true;

		while(currentPositionInPattern < patternSize)
		{
			// SEND VIBRATION INFORMATION
			CallVibration(currentPattern[currentPositionInPattern]);
			waitingForValue = true;

			// WAIT FOR ANSWER
			yield return new WaitForSeconds(timeBetweenNotes);

			// CHECK ANSWER
			if(!correctInput)
			{
				GameEventHandler.TriggerEvent(GameEvent.PatternFailure, patternArgument);

				GenerateNewPattern();
				currentPositionInPattern = 0;

				break;
			}

			// PREPARE FOR NEXT NUMBER
			currentPositionInPattern ++;
			correctInput = false;

			if (currentPositionInPattern == patternSize)
			{
				GameEventHandler.TriggerEvent(GameEvent.PatternSuccess, patternArgument);
			}
		}

		currentPositionInPattern = 0;
		isPlayingPattern = false;
	}
}
