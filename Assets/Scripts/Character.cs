using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    public bool noController = true;

    public Vector2 steering;

    public float moveForce;

    public Rigidbody body;
	void Start () {
        body = GetComponent<Rigidbody>();
		
	}
	
	void Update () {

        if(noController){
            steering.x = Input.GetAxis("Horizontal");
            steering.y = Input.GetAxis("Vertical");
        }

        Vector3 move = Vector3.right * steering.x + Vector3.forward * steering.y;
        body.AddForce(move * Time.deltaTime * moveForce);

		
	}
}
