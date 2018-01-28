using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {
    public Mesh[] meshes;

    public AudioClip[] impactSounds;

    public ParticleSystem trailEffect, impactEffect; 
    private Rigidbody rb;
    public float asteroidSinkFactor;

	public AnimationCurve pushForceCurve;
	public float forceMultiplier;

    private bool hasImpacted = false;
	
	void Start () {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        int rnd = Random.Range(0, meshes.Length);
        meshFilter.mesh = meshes[rnd];
        meshCollider.sharedMesh = meshes[rnd];
        rb = GetComponent<Rigidbody>();
	}

    public void Explode(){
        StartCoroutine(Exploder());
    }
    private IEnumerator Exploder(){
        impactEffect.Play();
        GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(impactEffect.main.startLifetime.constant);
        Destroy(this.gameObject);
    }

    public void Impact(){
        //Impact particle system and stuff

        trailEffect.gameObject.SetActive(false);
        impactEffect.Play();
        rb.isKinematic = true;
        AudioSource.PlayClipAtPoint(impactSounds[Random.Range(0, impactSounds.Length)], transform.position);      
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y-asteroidSinkFactor, this.transform.position.z);

        hasImpacted = true;

		//Push everyone
		Vector3 recPos = transform.position;
		Character[] chars = GameHandler.instance.playerJoin.characters;

		for(int i = 0; i < chars.Length;i++){
            Vector3 delta = chars[i].transform.position - recPos;

            float force = pushForceCurve.Evaluate(delta.magnitude) * forceMultiplier;
            chars[i].body.AddForce(delta.normalized * force, ForceMode.Impulse);
		}


    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        if(!hasImpacted){
            if(other.transform.tag == "Team0" || other.transform.tag == "Team1"){
                Impact();
            }
        }
        if(other.transform.tag.Equals("Ground")){
            Impact();
        }
        if(other.transform.tag.Equals("Asteroid")){
            this.Explode();
            other.gameObject.GetComponent<Asteroid>().Explode();
        }
    }
    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("SignalZone")){
            this.Explode();
        }
    }
}
