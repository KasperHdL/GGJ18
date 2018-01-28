using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingSignalSpawner : MonoBehaviour {

	public GameObject incomingSignalPrefab;
	public GameObject astroidPrefab;
	public float yOffset;
	public float asteriodSinkage;

	public GameObject asteroidProjector;
	[Range(0.1f,6f)]
	public float asteroidProjectorTime;
	
	void Start()
	{
		SpawnSignal(null);

		GameEventHandler.Subscribe(GameEvent.BeamDisrupted, SpawnSignal);
	}
	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.H)){
			SpawnAstroid();
		}
	}

	private void SpawnSignal(GameEventArgs argument)
	{
		GameObject temp = Instantiate(incomingSignalPrefab,RandomPointInBox(this.transform.position,this.transform.localScale),Quaternion.identity);
		RaycastHit hit;
		if(Physics.Raycast(temp.transform.position,Vector3.down,out hit,Mathf.Infinity,LayerMask.GetMask("Ground"))){
			Debug.Log(hit.transform.name);
			temp.transform.position = new Vector3(temp.transform.position.x,hit.point.y+yOffset,temp.transform.position.z);
		}
	}

	public void SpawnAstroid(){
		GameObject temp = Instantiate(astroidPrefab,RandomPointInBox(this.transform.position,this.transform.localScale),Quaternion.identity);
		temp.GetComponent<Asteroid>().asteroidSinkFactor = asteriodSinkage;
		asteroidProjector.transform.position = temp.transform.position;
		StartCoroutine(AsteroidProjectorTimer());
	}

	private IEnumerator AsteroidProjectorTimer(){
		asteroidProjector.gameObject.SetActive(true);
		yield return new WaitForSeconds(asteroidProjectorTime);
		asteroidProjector.gameObject.SetActive(false);
	}

	private Vector3 RandomPointInBox(Vector3 center, Vector3 size) {
 
			return center + new Vector3(
				(Random.value - 0.5f) * size.x,
				(Random.value - 0.5f) * size.y,
				(Random.value - 0.5f) * size.z
			);
		}
}
