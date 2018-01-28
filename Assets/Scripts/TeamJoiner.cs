﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamJoiner : MonoBehaviour {

	private GameHandler gameHandler;
	public Character.RoverType teamType;
	public Text missingPlayers;

	public Team team;

	private int numPlayers;


	// Use this for initialization
	void Start () {
		gameHandler = GameHandler.instance;
		missingPlayers.text = 2-numPlayers + " Players Missing";
		
		GameEventHandler.Subscribe(GameEvent.GameStarted, GameStarted);
	}
	
	public void GameStarted(GameEventArgs eventArgs){
		Collider c = GetComponent<Collider>();

		Destroy(c);
		Destroy(this);
		missingPlayers.enabled = false;
		

		GameEventHandler.Unsubscribe(GameEvent.GameStarted, GameStarted);
	}

	void OnTriggerEnter(Collider coll){
		if(coll.tag == "Team0" || coll.tag  == "Team1"){
			Character c = coll.GetComponent<Character>();
			c.SetRoverType(teamType);
			numPlayers++;
			gameHandler.PlayerJoinedTeam(teamType);
			missingPlayers.text = 2-numPlayers + " Players Missing";
		}
	}

	void OnTriggerExit(Collider coll){
		if(coll.tag == "Team0" || coll.tag  == "Team1"){
			numPlayers--;
			gameHandler.PlayerLeftTeam(teamType);
			missingPlayers.text = 2-numPlayers + " Players Missing";
		}
	}
}
