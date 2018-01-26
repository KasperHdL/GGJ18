using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Character : MonoBehaviour {

    public enum Thumbstick{
        Left,
        Right,
    }

    public bool use_keyboard = false;
    public PlayerIndex playerIndex;
    public Thumbstick thumbstick;


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

            if(thumbstick == Thumbstick.Left){
                steering.x = state.ThumbSticks.Left.X;
                steering.y = state.ThumbSticks.Left.Y;
            }else{
                steering.x = state.ThumbSticks.Right.X;
                steering.y = state.ThumbSticks.Right.Y;
            }
        }

        Vector3 move = Vector3.right * steering.x + Vector3.forward * steering.y;
        body.AddForce(move * Time.deltaTime * moveForce);

		
	}
}
