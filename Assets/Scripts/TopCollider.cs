using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Team0") || other.CompareTag("Team1"))
        {
            other.GetComponent<Character>().noControl = true;
            other.GetComponent<Character>().body.AddForce(Vector3.down);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Team0") || other.CompareTag("Team1"))
        {
            other.GetComponent<Character>().noControl = false;
        }
    }
}
