using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour {

	private Vector3 startLocation;
	public Transform endLocation;

	public AnimationCurve startCurve;

	public float startDuration;

	[Header("Height Movement")]
	public float minHeight;
	public float maxHeight;
	public float minDistHeightMap;
	public float maxDistHeightMap;


	[Header("Movement")]
	public float maxCameraMovement;
	public float planeMovementMultiplier;

	private bool startHasRun = false;

	private Character[] characters;

	void Start(){
		startLocation = transform.position;
		GameEventHandler.Subscribe(GameEvent.GameStarted, GameStarted);

		characters = GameHandler.instance.playerJoin.characters;
	}

	public void GameStarted(GameEventArgs eventArgs){
		if(startHasRun) return;

		StartCoroutine(LerpToGame(startDuration));
		GameEventHandler.Unsubscribe(GameEvent.GameStarted, GameStarted);

	}

	// Update is called once per frame
	void Update () {
		if(!startHasRun) return;

		Vector3 sum = Vector3.zero;

			
		int activePlayers = 0;
		for(int i = 0; i < characters.Length;i++){
			if(characters[i].gameObject.activeSelf){
				activePlayers++;
				sum += characters[i].transform.position;
			}
		}

		sum.y = 0;
		sum /= activePlayers;


		float maxDistance = 0;

		for(int i = 0; i < characters.Length; i++){
			if(!characters[i].gameObject.activeSelf)continue;

			for(int j = 1; j < characters.Length; j++){
				if(!characters[j].gameObject.activeSelf)continue;
				Vector3 delta = characters[i].transform.position - characters[j].transform.position;

				if(delta.magnitude > maxDistance) maxDistance = delta.magnitude;
			}
		}

		float distT = (maxDistance - minDistHeightMap) / (maxDistHeightMap - minDistHeightMap);

		sum.y = Mathf.Lerp(minHeight, maxHeight, distT);

		Vector3 desired = sum - transform.position;

		if(desired.magnitude > maxCameraMovement){
			desired = desired.normalized * maxCameraMovement;
		}

		transform.position += desired * Time.deltaTime;
//		transform.position = sum;
	}

	IEnumerator LerpToGame(float duration){
		float startTime = Time.time;
		float endTime = startTime + duration;
		
		Vector3 startPos = transform.position;
		Vector3 endPos = endLocation.transform.position;

		Quaternion startRot = transform.rotation;
		Quaternion endRot = endLocation.transform.rotation;

		while(endTime > Time.time){
			float t = (Time.time - startTime) /duration;

			transform.position = Vector3.Lerp(startPos, endPos, startCurve.Evaluate(t));
			transform.rotation = Quaternion.Slerp(startRot, endRot, startCurve.Evaluate(t));

			yield return null;

		}

		transform.position = endPos;
		transform.rotation = endRot;

		//delay
		yield return new WaitForSeconds(.1f);
		startHasRun = true;
	}
}
