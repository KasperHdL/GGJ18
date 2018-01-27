using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Beam : MonoBehaviour {
	[HideInInspector]
	public GameObject player1;
	[HideInInspector]
	public GameObject player2;
	
	public float minDist;
	public bool disrupted;
	
	private Color currentColor;
	private LineRenderer line;
	public Material lineMaterial;
	float dist;
	
	void Start () 
	{

		line = GetComponent<LineRenderer>();
		line.positionCount = 2;
		line.GetComponent<Renderer>().sharedMaterial = lineMaterial;
		currentColor = Color.black;
		disrupted = true;
		distrupt();
	}

	private float distruptTimer = 0f;
	public Color distruptColor;
	public Color enabledColor;

	void Update () {
		dist = Vector3.Distance(player1.transform.position,player2.transform.position);

		currentColor = Color.Lerp(enabledColor,distruptColor,minDist/dist);

		Debug.DrawLine(player1.transform.position,player2.transform.position,currentColor,Time.deltaTime);
		line.SetPosition(0,player1.transform.position);
		line.SetPosition(1,player2.transform.position);
		lineMaterial.SetColor("_Color",currentColor);
	}
	private float Map(float from, float to, float from2, float to2, float value){
        if(value <= from2){
            return from;
        }else if(value >= to2){
            return to;
        }else{
            return (to - from) * ((value - from2) / (to2 - from2)) + from;
        }
    }

	public void distrupt(){
		disrupted = true;
		currentColor = distruptColor;
		distruptTimer = 0f;
		line.enabled = false;
	}
	public void enable(){
		disrupted = false;
		currentColor = enabledColor;
		line.enabled = true;
	}
}
