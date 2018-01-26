using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Beam : MonoBehaviour {

	public GameObject player1;
	public GameObject player2;
	public float minDist;
	public float maxDist;
	private int layer;
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
	private float enableTimer = 0f;
	public float distruptionDelay = 2f;
	public Color distruptColor;
	public float enableCooldown = 2f;
	public Color enabledColor;
	void Update () {
		if(disrupted){
			enableTimer+=Time.deltaTime;
			//currentColor = Color.Lerp(currentColor,enabledColor,Map(enableTimer,0f,enableCooldown,0f,1f));
			if(enableTimer>enableCooldown){
				enable();
			}
		}
		RaycastHit hit;
			if(Physics.Linecast(player1.transform.position,player2.transform.position,out hit)){
				if(!hit.transform.tag.Equals("Player")||hit.distance<minDist||hit.distance>maxDist){
					distruptTimer+=Time.deltaTime;
					//currentColor = Color.Lerp(currentColor,distruptColor,Map(distruptTimer,0f,distruptionDelay,0f,1f));
					if(distruptTimer>distruptionDelay){
						distrupt();
					}
				}
			}
		Debug.DrawLine(player1.transform.position,player2.transform.position,currentColor,Time.deltaTime);
		line.SetPosition(0,player1.transform.position);
		line.SetPosition(1,player2.transform.position);
		lineMaterial.SetColor("_TintColor",currentColor);
	}
	private float Map(float x, float from1, float to1, float from2, float to2)
    {
        return (x - from1) * (to2 - to1) / (from2 - from1) + to1;
    }

	public void distrupt(){
		disrupted = true;
		currentColor = Color.red;
		//lineMaterial.color = currentColor;
		distruptTimer = 0f;
		line.enabled = false;
	}
	public void enable(){
		disrupted = false;
		currentColor = Color.green;
		//lineMaterial.color = currentColor;
		enableTimer = 0f;
		line.enabled = true;
	}
}
