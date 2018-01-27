using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamJoiner : MonoBehaviour {

	public Character.RoverType teamType;

	public Team team;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider coll){
		if(coll.tag == "Player"){
			Character c = coll.GetComponent<Character>();
			c.SetRoverType(teamType);
		}
	}

	void OnTriggerExit(Collider coll){

	}
}
