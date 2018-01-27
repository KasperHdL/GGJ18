using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerJoin : MonoBehaviour {
    public bool[] controllersJoined;        // 0 = first controller, 1 = second controller, etc.
    private bool[] isConnected = {false};
    public List<int> playersJoined;        // if [0] = 1, then first player equals second controller, if [1] = 3 then second player equals fourth controller, etc.

    private GamePadState[] state;
    private GamePadState[] prevStates;

    public Character[] characters;

    // Use this for initialization
    void Start () {
        playersJoined = new List<int>();
        controllersJoined = new bool[4];
        isConnected = new bool[4];
        state = new GamePadState[4];
        prevStates = new GamePadState[4];

        for(int i = 0; i < characters.Length;i++)
        {
            characters[i].enabled = false;
        }
	}

    bool firstFrame = true;
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < 4; i++)
        {
            prevStates[i] = state[i];
            state[i] = GamePad.GetState((PlayerIndex)i);

            if (!isConnected[i] && state[i].IsConnected)
            {
                isConnected[i] = true;
                playersJoined.Add(i);

                characters[i].gameObject.SetActive(true);
                characters[i].enabled = true;
                characters[i].playerIndex = (PlayerIndex)i;

                controllersJoined[i] = true;

            }else if(isConnected[i] && !state[i].IsConnected){
                isConnected[i] = false;
                characters[i].gameObject.SetActive(false);
                characters[i].enabled = false;
                playersJoined.Remove(i);
                controllersJoined[i] = false;

            }
        }
    }
}
