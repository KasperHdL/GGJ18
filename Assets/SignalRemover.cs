using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalRemover : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameEventHandler.Subscribe(GameEvent.PatternSuccess, RemoveSignal);
	}

	private void RemoveSignal(GameEventArgs argument)
	{
		GameEventHandler.Unsubscribe(GameEvent.PatternSuccess, RemoveSignal);
		Destroy(this.gameObject);
	}
}
