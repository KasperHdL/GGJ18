using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour {

	[Header("References")]
	public PlayerJoin playerJoin;
	private Character[] characters;

	private int[] numPlayersOnTeam;

	public float startDelay;
	private float startGameTime = -1;

	private bool gameIsStarted = false;

	public Text countdownText;

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
	}

	public void StartGame(){
		gameIsStarted = true;
		countdownText.enabled = false;

	}

}
