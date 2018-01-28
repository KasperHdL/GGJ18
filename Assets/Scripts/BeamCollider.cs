using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamCollider : MonoBehaviour {
	public Team team;

	void OnTriggerEnter(Collider other)
	{
		if(other.transform.tag.Equals("Asteroid")){
			team.FAIL();
			other.transform.GetComponent<Asteroid>().Explode();
		}

		if(other.transform.tag.Equals("Team"+((1 + team.teamID) % 2)))
		{
			team.FAIL();
		}
	}
}
