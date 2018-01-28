using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISounds : MonoBehaviour {


    [Header("Assets")]
    public AudioClip success;
    public AudioClip fail;

    // Use this for initialization
    void Start () {
        GameEventHandler.Subscribe(GameEvent.PatternSuccess, OnSucces);
        GameEventHandler.Subscribe(GameEvent.PatternFailure, OnFail);
    }
	
	public void OnSucces(GameEventArgs args)
    {
        AudioSource.PlayClipAtPoint(success, transform.position);
    }

    public void OnFail(GameEventArgs args)
    {
        AudioSource.PlayClipAtPoint(fail, transform.position, 0.8f);
    }
}
