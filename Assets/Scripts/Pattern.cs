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
	private int currentCheckPosition = 0;

	private bool waitingForValue;
	private bool correctInput;
	private bool stopPatternPlayback = false;
	
	private RumbleArgs rumbleArgument = new RumbleArgs();
	private PatternArgs patternArgument = new PatternArgs();

	void Start()
	{
		GameEventHandler.Subscribe(GameEvent.SignalExit,StopPatternPlayback);
		GameEventHandler.Subscribe(GameEvent.SignalEnter,StartPatternPlayback);
	}

	private void StopPatternPlayback(GameEventArgs argument){
		SignalArgument signal = (SignalArgument) argument;
		if(signal.teamID != teamID)
		{
			return;
		}

		stopPatternPlayback = true;
		StopAllCoroutines();
	}
	private void StartPatternPlayback(GameEventArgs argument){
		stopPatternPlayback = false;
	}

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

		Debug.Log("Index: " + currentCheckPosition + " List Size: " + currentPattern.Count);
		correctInput = currentPattern[currentCheckPosition] == input;
		
		if (correctInput)
		{
			GameEventHandler.TriggerEvent(GameEvent.NoteSuccess, patternArgument);
			currentCheckPosition ++;
		}
		else
		{
			currentCheckPosition = 0;
			GameEventHandler.TriggerEvent(GameEvent.PatternFailure, patternArgument);
		}

		if (currentCheckPosition == patternSize)
		{
			GenerateNewPattern();
			currentCheckPosition = 0;
			waitingForValue = false;
			GameEventHandler.TriggerEvent(GameEvent.PatternSuccess, patternArgument);
		}
	}

	public void CallVibration(InputValues type)
	{
		rumbleArgument.vibrationType = type;
		GameEventHandler.TriggerEvent(GameEvent.Rumble, rumbleArgument);
	}

	public void StartPlayPatternCoroutine()
	{
		StopAllCoroutines();
		StartCoroutine(PlayPattern());
	}

	public IEnumerator PlayPattern()
	{
		Debug.Log("Pattern Coroutine Started");
		isPlayingPattern = true;
		waitingForValue = true;
		
		int currentPositionInPattern = 0;

		while(currentPositionInPattern < patternSize)
		{
			// SEND VIBRATION INFORMATION
			CallVibration(currentPattern[currentPositionInPattern]);
			
			yield return new WaitForSeconds(timeBetweenNotes);

			// PREPARE FOR NEXT NUMBER
			currentPositionInPattern ++;
			correctInput = false;
		}

		isPlayingPattern = false;
	}
}
