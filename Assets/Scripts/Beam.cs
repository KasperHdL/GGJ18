using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Beam : MonoBehaviour {

	public GameObject player1;
	public GameObject player2;
	public float minDist;
	public float maxDist;
	private Color currentColor;
	private bool disrupted;
	// Use this for initialization
	private LineRenderer line;
	public Material lineMaterial;
	void Start () {
		line = GetComponent<LineRenderer>();
		line.positionCount = 2;
		line.GetComponent<Renderer>().sharedMaterial = lineMaterial;
		currentColor = Color.white;
		disrupted = false;
		enable();
	}
	
	// Update is called once per frame

	private float distruptTimer = 0f;
	public float distruptionDelay = 2f;
	public Color distruptColor;
	public Color enabledColor;
	void Update () {
		if(!disrupted){
		RaycastHit hit;
			if(Physics.Linecast(player1.transform.position,player2.transform.position,out hit)){
				if(!hit.transform.tag.Equals("Player")||hit.distance<minDist||hit.distance>maxDist){
					distruptTimer+=Time.deltaTime;
					Debug.Log(Map(distruptTimer,0f,0f,distruptionDelay,1f));
					currentColor = Color.Lerp(currentColor,distruptColor,Map(distruptTimer,0f,0f,distruptionDelay,1f));
					if(distruptTimer>distruptionDelay){
						distrupt();
					}
				}
			}
		}
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
		//lineMaterial.color = currentColor;
		distruptTimer = 0f;
		line.enabled = false;
	}
	public void enable(){
		disrupted = false;
		currentColor = enabledColor;
		line.enabled = true;
	}
}
