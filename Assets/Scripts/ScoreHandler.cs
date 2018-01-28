using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour {
	public Text team0Text;
	public Text team1Text;

	public float timeForWin = 30;
	public int scoreIncrements = 10;

	public float team0Score;
	public float team1Score;

	private bool team0HasBeam;
	private bool team1HasBeam;

	private GameOverArgs winnerArguments = new GameOverArgs();

	// Use this for initialization
	void Start () 
	{
		GameEventHandler.Subscribe(GameEvent.PatternSuccess, OnPatternSuccess);
		GameEventHandler.Subscribe(GameEvent.BeamDisrupted, OnBeamDisruption);
	}

	void Update()
	{
		CheckForWinners();

		UpdatePoints();
	}

	private void CheckForWinners()
	{
		if (team0Score >= timeForWin)
		{
			winnerArguments.teamID = 0;
			GameEventHandler.TriggerEvent(GameEvent.GameOver, winnerArguments);
		} 
		else if (team1Score >= timeForWin)
		{
			winnerArguments.teamID = 1;
			GameEventHandler.TriggerEvent(GameEvent.GameOver, winnerArguments);
		}
	}

	private void UpdatePoints()
	{
		if (team0HasBeam)
		{
			team0Score += Time.deltaTime;
			UpdateScoreText(0);
		}
		else if (team1HasBeam)
		{
			team1Score += Time.deltaTime;
			UpdateScoreText(1);
		}
	}

	private void UpdateScoreText(int team)
	{
		string score = "[";
		float relevantScore;
		if (team == 0)
		{
			relevantScore = team0Score;
		} 
		else if (team == 1)
		{
			relevantScore = team1Score;
		} 
		else 
		{
			return;
		}

		int relevantScoreIncrement = (int)(timeForWin/scoreIncrements);
		int translatedScore = (int)(relevantScore/relevantScoreIncrement);

		for (int i = 1; i <= scoreIncrements; i++)
		{
			if (i < translatedScore)
			{
				score += "=";
				continue;
			}
			else if (i == translatedScore)
			{
				score += ">";
				continue;
			}
			score += " ";
		}
		score += "]";

		if (team == 0)
		{
			team0Text.text = score;
		} 
		else
		{
			team1Text.text = score;
		}
	}

	private void OnPatternSuccess(GameEventArgs arguments)
	{
		PatternArgs patternArguments = (PatternArgs)arguments;

		if (patternArguments.teamID == 0)
		{
			team0HasBeam = true;
		}

		if (patternArguments.teamID == 1)
		{
			team1HasBeam = true;
		}
	}

	private void OnBeamDisruption(GameEventArgs arguments)
	{
		team0HasBeam = false;
		team1HasBeam = false;
	}
}
