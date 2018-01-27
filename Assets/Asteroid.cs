using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {
    public Mesh[] meshes;

    public ParticleSystem ps; 

	
	void Start () {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        int rnd = Random.Range(0, meshes.Length);
        meshFilter.mesh = meshes[rnd];
        meshCollider.sharedMesh = meshes[rnd];
	}
	
	
	void Update ()
    {
		
	}
}
