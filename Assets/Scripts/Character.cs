using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Character : MonoBehaviour {

    public bool use_keyboard = false;
    public PlayerIndex playerIndex;

    public Vector2 steering;

    public float moveForce;

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
     void OnGUI()
    {
        string text = "Use left stick to turn the cube, hold A to change color\n";
        text += string.Format("IsConnected {0} Packet #{1}\n", state.IsConnected, state.PacketNumber);
        text += string.Format("\tTriggers {0} {1}\n", state.Triggers.Left, state.Triggers.Right);
        text += string.Format("\tD-Pad {0} {1} {2} {3}\n", state.DPad.Up, state.DPad.Right, state.DPad.Down, state.DPad.Left);
        text += string.Format("\tButtons Start {0} Back {1} Guide {2}\n", state.Buttons.Start, state.Buttons.Back, state.Buttons.Guide);
        text += string.Format("\tButtons LeftStick {0} RightStick {1} LeftShoulder {2} RightShoulder {3}\n", state.Buttons.LeftStick, state.Buttons.RightStick, state.Buttons.LeftShoulder, state.Buttons.RightShoulder);
        text += string.Format("\tButtons A {0} B {1} X {2} Y {3}\n", state.Buttons.A, state.Buttons.B, state.Buttons.X, state.Buttons.Y);
        text += string.Format("\tSticks Left {0} {1} Right {2} {3}\n", state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y, state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), text);
    }
}
