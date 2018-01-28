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

	public float patternIntervals = 2.0f;

	private bool hasSignal;
	private bool left;
	private bool right;
	private bool playerFound;

	void Start()
	{

		if(receiver == null || sender == null){
			gameObject.SetActive(false);
		}

		GameEventHandler.Subscribe(GameEvent.Rumble, SendVibration);
		GameEventHandler.Subscribe(GameEvent.PatternSuccess, PatternCompletion);
		GameEventHandler.Subscribe(GameEvent.PatternFailure, PatternFailure);
		GameEventHandler.Subscribe(GameEvent.NoteSuccess, NoteSuccess);

		pattern.teamID = teamID;
		pattern.GenerateNewPattern();
		
		pattern.Initialize();


		PlayerSwap(50);
        GameEventHandler.Subscribe(GameEvent.SignalEnter, SetReceiverSender);
        GameEventHandler.Subscribe(GameEvent.SignalExit, CheckAndStopPlayPattern);
    }
	
	void Update()
	{
		CheckSenderInput();

		if (!beam.disrupted)
		{
			RaycastHit hit;
			Vector3 playerDifference = (receiver.transform.position - sender.transform.position);
			Vector3 centerpoint = sender.transform.position + playerDifference/2;
			//if(Physics.Linecast(receiver.transform.position, sender.transform.position, out hit,LayerMask.GetMask("Hitables")))
			if (Physics.BoxCast(centerpoint, new Vector3(1.0f, 50.0f, Vector3.Magnitude(playerDifference)/2), Vector3.Normalize(playerDifference), out hit, Quaternion.identity, 100.0f, LayerMask.GetMask("Hitables")))
			{
				if(!hit.transform.tag.Equals("Team"+teamID) || hit.distance < beam.minDist)
				{
					FAIL();
					if(hit.transform.tag.Equals("Asteroid")){
						hit.transform.GetComponent<Asteroid>().Explode();
					}
				}
			}
		}
	}

	public void CheckAndStopPlayPattern(GameEventArgs arguments)
	{
		SignalArgument signalArgument = (SignalArgument)arguments;

		if (!playerFound)
		{
			return;
		}

		hasSignal = false;
	}

    public void SetReceiverSender(GameEventArgs arguments)
	{
		SignalArgument signalArgument = (SignalArgument)arguments;
		playerFound = false;

		if ((int)sender.playerIndex == signalArgument.playerID)
		{
			playerFound = true;
			PlayerSwap(0);
		} else if ((int)receiver.playerIndex == signalArgument.playerID)
		{
			playerFound = true;
		}

		if (playerFound)
		{
			StartCoroutine(RepeatedPlayback());
		}
	}

	private IEnumerator RepeatedPlayback()
	{
		hasSignal = true;

		while (hasSignal)
		{
			if (!beam.disrupted)
			{
				break;
			}

			if (!pattern.isPlayingPattern)
			{
				yield return new WaitForSeconds(patternIntervals);
				if (beam.disrupted)
				{
					pattern.StartPlayPatternCoroutine();
				}
			}

			yield return new WaitForEndOfFrame();
		}
	}

	public void NoteSuccess(GameEventArgs argument)
	{
		PatternArgs patternArgument = (PatternArgs)argument;
		if (patternArgument.teamID != teamID)
		{
			return;
		}

		sender.correctNoteFeedback.Play();
	}

	public void PatternFailure(GameEventArgs argument)
	{
		Debug.Log("Pattern Failed");

		PatternArgs patternArgument = (PatternArgs)argument;
		if (patternArgument.teamID != teamID)
		{
			return;
		}
	}

	public void FAIL(){
		GameEventHandler.TriggerEvent(GameEvent.BeamDisrupted);
		beam.distrupt();

		PlayerSwap(0);

		pattern.ResetIntensity();
	}

	public void PlayerSwap(int threshhold)
	{
		int randomNum = Random.Range(0, 100);

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
			hasSignal = false;
		}
	}

	public void SendInputToPattern(InputValues inputValue)
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
		if (sender.state.Buttons.LeftShoulder == ButtonState.Pressed && sender.prevState.Buttons.LeftShoulder == ButtonState.Released)
		{
			left = true;
		}
		
		if (sender.state.Buttons.RightShoulder == ButtonState.Pressed && sender.prevState.Buttons.RightShoulder == ButtonState.Released)
		{
			right = true;
		}
		InputValues inputValues = new InputValues();
		
		if (left)
		{
			inputValues = InputValues.Left;
		}
		else if (right)
		{
			inputValues = InputValues.Right;
		}

		if (left || right)
		{
			SendInputToPattern(inputValues);

			right = false;
			left = false;
		}
	}
}