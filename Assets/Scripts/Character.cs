using System.Collections;
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

    [Range(0.1f, 1.0f)]
    public float vibrationStrength;
    [Range(0.1f, 0.5f)]
    public float vibrationLength = 0.5f;


    [Header("Character Settings")]
    public float moveForce;


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

    public void Vibrate(InputValues vibrationType)
    {
        switch(vibrationType)
        {
            case InputValues.Both:
                GamePad.SetVibration(playerIndex, vibrationStrength, vibrationStrength);
            break;

            case InputValues.Left:
                GamePad.SetVibration(playerIndex, vibrationStrength, 0.0f);
            break;

            case InputValues.Right:
                GamePad.SetVibration(playerIndex, 0.0f, vibrationStrength);
            break;
        }

        StartCoroutine(StopVibrationAfterTimer());
    }

    public IEnumerator StopVibrationAfterTimer()
    {
        yield return new WaitForSeconds(vibrationLength);

        GamePad.SetVibration(playerIndex, 0.0f, 0.0f);
    }
}
