using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Team : MonoBehaviour {
	public int teamID;

	public Character receiver;
	public Character sender;

	public Pattern pattern;
	public Beam beam;

	[Range(0.01f, 1.0f)]
	public float inputMargin;
	
	private float inputTimeStamp;
	private bool readyForInput;

	private bool left;
	private bool right;
	private bool leftSet;
	private bool rightSet;

	private float playerDistance;

	void Start()
	{
		GameEventHandler.Subscribe(GameEvent.Rumble, SendVibration);
		GameEventHandler.Subscribe(GameEvent.PatternSuccess, PatternCompletion);
		GameEventHandler.Subscribe(GameEvent.PatternFailure, PatternFailure);

		pattern.teamID = teamID;
		pattern.GenerateNewPattern();
		
		pattern.Initialize();

		PlayerSwap(50);
	}

	void Update()
	{
		CheckSenderInput();

		if (beam.disrupted)
		{
			CheckPatternStart();

		}
		else
		{
			RaycastHit hit;
			if(Physics.Linecast(receiver.transform.position,sender.transform.position,out hit)){
				if(!hit.transform.tag.Equals("Team"+teamID)||hit.distance<beam.minDist||hit.distance>beam.maxDist){
					FAIL();
				}
			}
		}
	}

	public void PatternFailure(GameEventArgs argument)
	{
		Debug.Log("Pattern Failed");

		PatternArgs patternArgument = (PatternArgs)argument;
		if (patternArgument.teamID != teamID)
		{
			return;
		}

		if (!beam.disrupted)
		{
			FAIL();
		}
	}

	public void FAIL(){
		beam.distrupt();

		PlayerSwap(0);

		pattern.ResetIntensity();
	}

	public void PlayerSwap(int threshhold)
	{
		int randomNum = Random.Range(0, 100);

		Debug.Log(randomNum);

		if (randomNum >= threshhold)
		{
			Character temp = sender;
			sender = receiver;
			receiver = temp;
		}
	}

	public void PatternCompletion(GameEventArgs argument)
	{
		Debug.Log("Pattern Completed");

		PatternArgs patternArgument = (PatternArgs)argument;
		if (patternArgument.teamID != teamID)
		{
			return;
		}

		if (beam.disrupted)
		{
			beam.enable();
		} 
		else 
		{
			pattern.IncreaseIntensity();
		}
		
		StartCoroutine(WaitToStartPattern());
	}

	private IEnumerator WaitToStartPattern()
	{
		Debug.Log("Waiting");

		yield return new WaitForSeconds(pattern.timeBetweenNotes);

		if (!beam.disrupted && !pattern.isPlayingPattern)
		{
			pattern.StartPlayPatternCoroutine();
		}
	}

	public void CheckPatternStart()
	{
		if (!pattern.isPlayingPattern)
		{
			playerDistance = Vector3.Distance(receiver.transform.position, sender.transform.position);
			if (playerDistance > beam.minDist && playerDistance < beam.maxDist)
			{
				pattern.StartPlayPatternCoroutine();
			}
		}
	}

	public void SendInputToPatten(InputValues inputValue)
	{
		if (inputValue == InputValues.Count)
		{
			Debug.LogError("Team " + teamID + ": ERROR IN SENDING INPUT TO PATTERN");
			return;
		}

		pattern.InputValue(inputValue);
	}

	public void SendVibration(GameEventArgs arguments)
	{
		RumbleArgs rumbleArguments = (RumbleArgs)arguments;
		if (rumbleArguments.teamID == teamID)
		{
			receiver.Vibrate(rumbleArguments.vibrationType);
		}
	}

	public void CheckSenderInput()
	{
		if (!leftSet)
		{
			left = sender.state.Buttons.LeftShoulder == ButtonState.Pressed;
		}
		if (!rightSet)
		{
			right = sender.state.Buttons.RightShoulder == ButtonState.Pressed;
		}

		if (!leftSet && left)
		{
			leftSet = true;
		}

		if (!rightSet && right)
		{
			rightSet = true;
		}

		if (!readyForInput &&
			((sender.prevState.Buttons.LeftShoulder == ButtonState.Released && left && (sender.prevState.Buttons.RightShoulder == ButtonState.Released || !right)) || 
			(sender.prevState.Buttons.RightShoulder == ButtonState.Released && right && (sender.prevState.Buttons.LeftShoulder == ButtonState.Released || !left))))
		{
			readyForInput = true;
			inputTimeStamp = Time.time + inputMargin;
		}

		if (readyForInput && Time.time > inputTimeStamp)
		{
			Debug.Log("Left: " + left + " Right: " + right);
			InputValues inputValues = InputValues.Count;
			
			if (left && right)
			{
				inputValues = InputValues.Both;
			}
			else if (left)
			{
				inputValues = InputValues.Left;
			}
			else if (right)
			{
				inputValues = InputValues.Right;
			}

			SendInputToPatten(inputValues);
			readyForInput = false;

			leftSet = false;
			rightSet = false;
		}
	}
}