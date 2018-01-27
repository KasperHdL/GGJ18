using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingSignalSpawner : MonoBehaviour {

	public GameObject incomingSignalPrefab;
	public float yOffset;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.A)){
			GameObject temp = Instantiate(incomingSignalPrefab,RandomPointInBox(this.transform.position,this.transform.localScale),Quaternion.identity);
			RaycastHit hit;
			if(Physics.Raycast(temp.transform.position,Vector3.down,out hit,Mathf.Infinity,LayerMask.GetMask("Ground"))){
				temp.transform.position = new Vector3(temp.transform.position.x,hit.transform.position.y+yOffset,temp.transform.position.z);
			}
		}
	}

	private Vector3 RandomPointInBox(Vector3 center, Vector3 size) {
 
			return center + new Vector3(
				(Random.value - 0.5f) * size.x,
				(Random.value - 0.5f) * size.y,
				(Random.value - 0.5f) * size.z
			);
		}
}
