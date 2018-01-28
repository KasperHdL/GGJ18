using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingSignalSpawner : MonoBehaviour {

	public GameObject incomingSignalPrefab;
	public GameObject astroidPrefab;
	public GameObject asteroidImpactIcon;
	public float yOffset;
	public float asteriodSinkage;
	private bool gameover;
	public float asteriodSpawnFreq = 10;
	
	void Start()
	{
		SpawnSignal(null);
		gameover = false;

		GameEventHandler.Subscribe(GameEvent.BeamDisrupted, SpawnSignal);
		GameEventHandler.Subscribe(GameEvent.GameOver,stopSpawning);
	}

	/// <summary>
	/// This function is called when the object becomes enabled and active.
	/// </summary>
	void OnEnable()
	{
		startSpawning();
	}

	private void startSpawning(){
		Debug.Log("hello");
		StartCoroutine(Spawn());
	}
	private void stopSpawning(GameEventArgs args){
		gameover = true;
	}
	private IEnumerator Spawn(){
		while(!gameover){
			yield return new WaitForSeconds(asteriodSpawnFreq);
			SpawnAstroid();
		}
	}

	private void SpawnSignal(GameEventArgs argument)
	{
		GameObject temp = Instantiate(incomingSignalPrefab,RandomPointInBox(this.transform.position,this.transform.localScale),Quaternion.identity);
		RaycastHit hit;
		if(Physics.Raycast(temp.transform.position,Vector3.down,out hit,Mathf.Infinity,LayerMask.GetMask("Ground"))){
			temp.transform.position = new Vector3(temp.transform.position.x,hit.point.y+yOffset,temp.transform.position.z);
		}
	}

	public void SpawnAstroid(){
		GameObject temp = Instantiate(astroidPrefab,RandomPointInBox(this.transform.position,this.transform.localScale),Quaternion.identity);
		temp.GetComponent<Asteroid>().asteroidSinkFactor = asteriodSinkage;
		GameObject impactSprite = Instantiate(asteroidImpactIcon,temp.transform.position,asteroidImpactIcon.transform.rotation);
		RaycastHit hit;
		if(Physics.Raycast(temp.transform.position,Vector3.down,out hit,Mathf.Infinity,LayerMask.GetMask("Ground"))){
			impactSprite.transform.position = new Vector3(temp.transform.position.x,hit.point.y+yOffset,temp.transform.position.z);
		}
		StartCoroutine(RemoveImpactIcon(impactSprite));
	}

	private IEnumerator RemoveImpactIcon(GameObject go){
		yield return new WaitForSeconds(3.5f);
		Destroy(go);
	}

	private Vector3 RandomPointInBox(Vector3 center, Vector3 size) {
 
			return center + new Vector3(
				(Random.value - 0.5f) * size.x,
				(Random.value - 0.5f) * size.y,
				(Random.value - 0.5f) * size.z
			);
		}
}
