using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputValues
{
	left,
	right,
	both,
	count
}

public class Pattern : MonoBehaviour {
	[Range(0.0f, 5.0f)]
	public float timeBetweenNotes;

	private List<int> currentPattern = new List<int>();
	
	private int patternSize = 10;
	
	private int currentPositionInPattern = 0;

	private bool waitingForValue;
	private bool correctInput;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			Debug.Log("A PRESSED");
			GenerateNewPattern();
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			Debug.Log("S PRESSED");
			StartCoroutine("PlayPattern");
		}

		if (Input.GetKeyDown(KeyCode.Keypad0))
		{
			InputValue(InputValues.left);
		}

		if (Input.GetKeyDown(KeyCode.Keypad1))
		{
			InputValue(InputValues.right);
		}

		if (Input.GetKeyDown(KeyCode.Keypad2))
		{
			InputValue(InputValues.both);
		}
	}

	public void GenerateNewPattern()
	{
		for (int i = 0; i < patternSize; i++)
		{
			int value = Random.Range(0, (int)(InputValues.count));
			currentPattern.Add(value);

			Debug.Log(value);
		}
	}

	public void InputValue(InputValues input)
	{
		if (!waitingForValue)
		{
			return;
		}

		Debug.Log("INSERTED VALUE: " + (int)input);

		correctInput = currentPattern[currentPositionInPattern] == (int)input;
		
		waitingForValue = false;
	}

	public List<int> GetCurrentPattern()
	{
		return currentPattern;
	}

	public IEnumerator PlayPattern()
	{
		while(currentPositionInPattern < patternSize)
		{
			// SEND VIBRATION INFORMATION
			Debug.Log(currentPattern[currentPositionInPattern]);
			waitingForValue = true;

			// WAIT FOR ANSWER
			yield return new WaitForSeconds(timeBetweenNotes);

			// CHECK ANSWER
			if(!correctInput)
			{
				Debug.Log("WRONG INPUT!!!");
				GenerateNewPattern();
				currentPositionInPattern = 0;

				break;
			}

			Debug.Log("YAAAY");

			// PREPARE FOR NEXT NUMBER
			currentPositionInPattern ++;
			correctInput = false;
		}
	}
}
