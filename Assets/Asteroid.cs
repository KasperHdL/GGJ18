using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {
    public Mesh[] meshes;

    public AudioClip[] impactSounds;

    public ParticleSystem trailEffect, impactEffect; 
    private Rigidbody rb;
    public float asteroidSinkFactor;

	
	void Start () {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        int rnd = Random.Range(0, meshes.Length);
        meshFilter.mesh = meshes[rnd];
        meshCollider.sharedMesh = meshes[rnd];
        rb = GetComponent<Rigidbody>();
	}

    public void Explode(){
        //explode particle and stuff

        Destroy(this.gameObject);
    }

    public void Impact(){
        //Impact particle system and stuff

        trailEffect.gameObject.SetActive(false);
        impactEffect.Play();
        rb.isKinematic = true;
        AudioSource.PlayClipAtPoint(impactSounds[Random.Range(0, impactSounds.Length)], transform.position);      
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y-asteroidSinkFactor, this.transform.position.z);
    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        if(other.transform.tag.Equals("Ground")){
            Impact();
        }
        if(other.transform.tag.Equals("Asteroid")){
            this.Explode();
            other.gameObject.GetComponent<Asteroid>().Explode();
        }
    }
}
