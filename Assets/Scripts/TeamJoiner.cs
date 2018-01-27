using System.Collections;
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
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider coll){
		if(coll.tag == "Player"){
			Character c = coll.GetComponent<Character>();
			c.SetRoverType(teamType);
			numPlayers++;
			gameHandler.PlayerJoinedTeam(teamType);
			missingPlayers.text = 2-numPlayers + " Players Missing";
		}
	}

	void OnTriggerExit(Collider coll){
		if(coll.tag == "Player"){
			numPlayers--;
			gameHandler.PlayerLeftTeam(teamType);
			missingPlayers.text = 2-numPlayers + " Players Missing";
		}
	}
}
