using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerJoin : MonoBehaviour {
    public bool[] controllersJoined;        // 0 = first controller, 1 = second controller, etc.
    public bool[] playersReady;
    public List<int> playersJoined;        // if [0] = 1, then first player equals second controller, if [1] = 3 then second player equals fourth controller, etc.

    private GamePadState[] state;
    private GamePadState[] prevStates;

    // Use this for initialization
    void Start () {
        playersJoined = new List<int>();
        controllersJoined = new bool[4];
        playersReady = new bool[4];
        state = new GamePadState[4];
        prevStates = new GamePadState[4];
	}

    bool firstFrame = true;
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < 4; i++)
        {
            prevStates[i] = state[i];
            state[i] = GamePad.GetState((PlayerIndex)i);

            if (state[i].IsConnected)
            {
                // Check for start
                if (controllersJoined[i])
                {
                    if (prevStates[i].Buttons.A == ButtonState.Pressed && state[i].Buttons.A == ButtonState.Released)
                    {
                        playersReady[i] = !playersReady[i];
                    }
                }

                // Check if joined
                if (prevStates[i].Buttons.Start == ButtonState.Pressed && state[i].Buttons.Start == ButtonState.Released)
                {
                    if(!controllersJoined[i]) { 
                        playersJoined.Add(i);
                        controllersJoined[i] = true;
                    }
                    else if(controllersJoined[i])
                    {
                        playersJoined.Remove(i);
                        controllersJoined[i] = false;
                    }
                }

            }
        }

        bool allPlayersReady = true;
        for (int i = 0; i < playersJoined.Count; i++)
        {
            if(!playersReady[i])
            {
                allPlayersReady = false;
            }
        }

        if (allPlayersReady)
            StartGame();
    }

    void StartGame()
    {
        for(int i = 0; i < playersJoined.Count;i++)
        {
            Debug.Log("PLAYER " + i + " IS CONTROLLER" + playersJoined[i]);
        }
    }
}
