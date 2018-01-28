using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour {

	[Header("References")]
	public PlayerJoin playerJoin;
	public Team[] teams;
	public Text countdownText;
	public IncomingSignalSpawner signalSpawner;
	public GameObject introColliders;


	private Character[] characters;
	private int[] numPlayersOnTeam;




	[Header("Start Settings")]
	public float startDelay;
	private float startGameTime = -1;
	private bool gameIsStarted = false;


	public static GameHandler instance;

	public void Awake(){
		if(instance != null){
			Destroy(gameObject);
			return;
		}
		instance = this;
	}

	public void Start(){
		characters = playerJoin.characters;
		numPlayersOnTeam = new int[(int)Character.RoverType.Count];
		countdownText.enabled = false;
	}

	public void FixedUpdate(){
		if(gameIsStarted) return;

		bool allTeamsReady = true;
		for (int i = 0; i < numPlayersOnTeam.Length; i++){
			if(numPlayersOnTeam[i] != 2){
				allTeamsReady = false;
			}
		}

		if(allTeamsReady || Input.GetKey(KeyCode.Space)){
			countdownText.enabled = true;
			if(startGameTime == -1){
				startGameTime = Time.time + startDelay;
			}

			float t = (startGameTime - Time.time);
			countdownText.text = t.ToString("0");

			if(Time.time > startGameTime){
				StartGame();
			}
		}else{
			countdownText.enabled = false;
			startGameTime = -1;
		}
	}

	public void PlayerJoinedTeam(Character.RoverType type){
		numPlayersOnTeam[(int) type]++;
	}
	public void PlayerLeftTeam(Character.RoverType type){
		numPlayersOnTeam[(int) type]--;
		startGameTime = -1;
	}

	public void StartGame(){
		gameIsStarted = true;
		countdownText.enabled = false; 
		signalSpawner.enabled = true;

		Destroy(introColliders);

		for(int i = 0; i < characters.Length; i++){
			if(!characters[i].gameObject.activeSelf) continue;

			int index = (int) characters[i].roverType;

			if(teams[index].receiver == null){
				teams[index].receiver = characters[i];
				continue;
			}

			if(teams[index].sender == null){
				teams[index].sender = characters[i];
			}

		}

		for(int i = 0; i < teams.Length; i++){
			teams[i].gameObject.SetActive(true);
			teams[i].teamID = i;
		}
		GameEventHandler.TriggerEvent(GameEvent.GameStarted);

	}

}
