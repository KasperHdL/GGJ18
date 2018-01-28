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

	private SphereCollider sphereCollider;

	// Use this for initialization
	void Start () {
		gameHandler = GameHandler.instance;
		missingPlayers.text = 2-numPlayers + " Players Missing";
		
		GameEventHandler.Subscribe(GameEvent.GameStarted, GameStarted);
		sphereCollider = GetComponent<SphereCollider>();
	}
	
	public void GameStarted(GameEventArgs eventArgs){
		Collider c = GetComponent<Collider>();

		Destroy(c);
		Destroy(this);
		missingPlayers.enabled = false;
		

		GameEventHandler.Unsubscribe(GameEvent.GameStarted, GameStarted);
	}

	void Update(){
		numPlayers = 0;
		foreach(Character rover in GameHandler.instance.playerJoin.characters){
			if(!rover.gameObject.activeSelf)continue;

			Vector3 delta = rover.transform.position - transform.position;

			if(delta.magnitude < sphereCollider.radius){
				numPlayers++;
			}
		}
		missingPlayers.text = 2-numPlayers + " Players Missing";

		gameHandler.UpdatePlayerForTeam(teamType, numPlayers);
	}

	void OnTriggerEnter(Collider coll){
		if(coll.tag == "Team0" || coll.tag  == "Team1"){
			Character c = coll.GetComponent<Character>();
			c.SetRoverType(teamType);
		}
	}

	void OnTriggerExit(Collider coll){
		if(coll.tag == "Team0" || coll.tag  == "Team1"){
		}
	}
}
