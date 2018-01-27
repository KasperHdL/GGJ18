using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverVisuals : MonoBehaviour {
	public Character.RoverType roverType;
	public Transform[] wheels;

	[Range(0,2)]
	public float wheelRotationMult;

	public void UpdateWheels(Transform[] simWheels){
		//front
        for(int i = 0; i < 2; i++){
			wheels[i].localRotation = Quaternion.Euler(0, -90 + simWheels[i].localRotation.eulerAngles.y * wheelRotationMult, -90);
        }

        //back
		for(int i = 2; i < 4; i++){
			wheels[i].localRotation = Quaternion.Euler(0, -90 - simWheels[i-2].localRotation.eulerAngles.y * wheelRotationMult, -90);
        }
	}
}
