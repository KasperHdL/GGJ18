using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinController : MonoBehaviour {

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	GameOverArgs args;
	void Start()
	{
		GameEventHandler.Subscribe(GameEvent.GameOver,WIN);
		args = new GameOverArgs();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.R)){
			args.teamID = 0;
			GameEventHandler.TriggerEvent(GameEvent.GameOver,args);
		}
		if(Input.GetKeyDown(KeyCode.C)){
			args.teamID = 1;
			GameEventHandler.TriggerEvent(GameEvent.GameOver,args);
		}
	}
	[Header("Objects")]
	public GameObject mainCamera;
	public GameObject chinaFlag;
	public GameObject russianFlag;
	[Header("Positions")]
	public Transform winCamPos;
	public Transform roverWinPos1;
	public Transform roverWinPos2;
	public Transform roverLosePos1;
	public Transform roverLosePos2;
	[Header("Variables")]
	public float camSpeed;

	private Character[] rovers;
	public void WIN(GameEventArgs args){
		GameOverArgs winArgs = (GameOverArgs)args;
		rovers = GameHandler.instance.playerJoin.characters;
		bool winsaved = false;
		bool losesaved = false;
		foreach(Character rover in rovers){
			if((int)rover.roverType==winArgs.teamID){
				if(!winsaved){
					rover.transform.position = roverWinPos1.transform.position;
					rover.transform.rotation = roverWinPos1.transform.rotation;
					winsaved = true;
				}else{
					rover.transform.position = roverWinPos2.transform.position;
					rover.transform.rotation = roverWinPos2.transform.rotation;
				}
			}else{
				if(!losesaved){
					rover.transform.position = roverLosePos1.transform.position;
					rover.transform.rotation = roverLosePos1.transform.rotation;
					losesaved = true;
				}else{
					rover.transform.position = roverLosePos2.transform.position;
					rover.transform.rotation = roverLosePos2.transform.rotation;
				}
			}
		}
		if(winArgs.teamID == 0){
			chinaFlag.transform.eulerAngles = new Vector3(chinaFlag.transform.eulerAngles.x,chinaFlag.transform.eulerAngles.y,85f);
			chinaFlag.transform.position = new Vector3(chinaFlag.transform.position.x,chinaFlag.transform.position.y-2f,chinaFlag.transform.position.z);
		}else{
			russianFlag.transform.eulerAngles = new Vector3(russianFlag.transform.eulerAngles.x,russianFlag.transform.eulerAngles.y,-80f);
			russianFlag.transform.position = new Vector3(russianFlag.transform.position.x,russianFlag.transform.position.y-2.5f,russianFlag.transform.position.z);
		}
		mainCamera.GetComponent<CameraHandler>().enabled = false;
		StartCoroutine(MoveCamera());
	}
	private IEnumerator MoveCamera(){
		while(mainCamera.transform.position!=winCamPos.position||mainCamera.transform.rotation!=winCamPos.rotation){
			yield return new WaitForEndOfFrame();
			mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,winCamPos.position,camSpeed);
			mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation,winCamPos.rotation,camSpeed*2);
		}
	}
}
