﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Character : MonoBehaviour {

    public bool use_keyboard = false;
    public PlayerIndex playerIndex;

    public float moveForce;

    public Vector2 steering;

    GamePadState state;
    GamePadState prevState;

    [HideInInspector] public Rigidbody body;
	void Start () {
        body = GetComponent<Rigidbody>();
		
	}
	
	void Update () {

        if(use_keyboard){
            steering.x = Input.GetAxis("Horizontal");
            steering.y = Input.GetAxis("Vertical");
        }else{
            prevState = state;
            state = GamePad.GetState(playerIndex);

            steering.x = state.ThumbSticks.Left.X;
            steering.y = state.ThumbSticks.Left.Y;

        }

        Vector3 move = Vector3.right * steering.x + Vector3.forward * steering.y;
        body.AddForce(move * Time.deltaTime * moveForce);

		
	}
}
