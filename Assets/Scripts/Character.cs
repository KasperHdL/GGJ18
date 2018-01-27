﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Character : MonoBehaviour {

    public enum Thumbstick{
        Left,
        Right,
    }

    public Vector2 steering;

    [Header("Controller Settings")]
    public bool use_keyboard = false;

    public PlayerIndex playerIndex;
    public Thumbstick thumbstick;

    public GamePadState state;
    public GamePadState prevState;


    [Header("Move Settings")]
    public float moveForce;

    public float innerOuterDeadzone;
    public float wheelIsInnerForce;
    public float wheelIsOuterForce;
    public float wheelStraightForce;

    public AnimationCurve frontWheelForce;
    public AnimationCurve backWheelForce;

    public float onGroundCheckDistance;

    public bool noWheelsOnGround = true;
    public float flipMinVelocity;
    public float flipDelay;
    public float startFlipAction = -1;


    [Header("References")]
    public Transform[] wheels;

    [HideInInspector] public Rigidbody body;


    [Header("Debug Info")]
    public float[] wheelForce = {0,0,0,0};
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

        bool noInput = steering == Vector2.zero;
        steering = steering.normalized;

        if(Mathf.Abs(steering.x) == 1)
            steering.y += 0.01f;


        Vector3 steering3 = Vector3.right * steering.x + Vector3.forward * steering.y;

        float dot = Vector3.Dot(steering3, transform.right);
        if(Mathf.Abs(dot) < innerOuterDeadzone) dot = 0;

        int drivingDirection = 0;

        if(dot != 0)
            drivingDirection = (int)Mathf.Sign(dot);

        noWheelsOnGround = true;
        for(int i = 0; i < wheels.Length; i++){
            bool wheelIsRight = i % 2 == 0;

            //ground check
            bool onGround = Physics.Raycast(wheels[i].transform.position, -transform.up, onGroundCheckDistance);
            Debug.DrawRay(wheels[i].transform.position, -transform.up * onGroundCheckDistance, Color.red);
            if(!onGround) {
                noWheelsOnGround = false;
                continue;
            }

            if(noInput) continue;


            float dir = Vector3.Dot(wheels[i].transform.forward, steering3);

            if(i < 2){ //assume first two wheels are front wheels and therefor rotate
                wheels[i].transform.rotation = Quaternion.LookRotation(steering3, Vector3.right);

                dir = frontWheelForce.Evaluate(dir);

                Debug.DrawRay(wheels[i].transform.position + 0.5f * Vector3.up, steering3.normalized, Color.green);
            }else{
                bool drivingBackwards = dir < 0;
                dir = backWheelForce.Evaluate(dir);
            }

            Vector3 force = wheels[i].transform.forward * moveForce * dir;

            Color c;
            if((wheelIsRight && drivingDirection == 1) || (!wheelIsRight &&  drivingDirection == -1)){
                force *= wheelIsInnerForce;
                c = Color.blue;
            }else if((wheelIsRight && drivingDirection == -1) || (!wheelIsRight && drivingDirection == 1)){
                force *= wheelIsOuterForce;
                c = Color.red;
            }else{
                force *= wheelStraightForce;
                c = Color.magenta;
            }

            if(drivingDirection == -1)
                force *= 1.1f;

            Debug.DrawRay(wheels[i].transform.position, force.normalized, c);

            wheelForce[i] = force.magnitude;

            body.AddForceAtPosition(force * Time.deltaTime, wheels[i].transform.position);
        }

        if(!noWheelsOnGround && body.velocity.magnitude < flipMinVelocity){
            if(startFlipAction == -1){
                startFlipAction = Time.time + flipDelay;
            }
            print(Time.time + " " + startFlipAction);
            if(startFlipAction < Time.time){
                startFlipAction = -1;

                transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
                transform.position += Vector3.up;
            }
        }else{
            startFlipAction = -1;
        }
	}
}
