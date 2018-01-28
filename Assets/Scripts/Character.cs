using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

[RequireComponent(typeof(ParticleSystem))]
public class Character : MonoBehaviour {
    public AudioSource motorAudio;

    [HideInInspector]
    public ParticleSystem correctNoteFeedback;

    public enum Thumbstick{
        Left,
        Right,
    }
    public enum RoverType{
        Russian,
        Chinese,
        Count
    }

    [HideInInspector] public RoverType roverType;

    [HideInInspector] public bool noControl;
    private SignalArgument signalArgument = new SignalArgument();

    public Vector2 steering;

    [Header("Controller Settings")]
    public bool use_keyboard = false;

    public PlayerIndex playerIndex;
    public Thumbstick thumbstick;

    public GamePadState state;
    public GamePadState prevState;

    [Range(0.1f, 1.0f)]
    public float vibrationStrength = 1.0f;
    [Range(0.1f, 0.5f)]
    public float vibrationLength = 0.5f;


    public CharacterSettings settings;
    private int wheelsOffGround = 0;
    private float startFlipAction = -1;


    [Header("References")]
    public Transform[] wheels;
    public RoverVisuals[] visuals;

    public bool isWithinSignal;

    private float nextFlipAllowed;
    private float nextFlipDelay = 1f;

    [HideInInspector] public Rigidbody body;


    [Header("Debug Info")]
    public float[] wheelForce = {0,0,0,0};


	void Start () {
        correctNoteFeedback = GetComponent<ParticleSystem>();
        body = GetComponent<Rigidbody>();
        motorAudio = GetComponent<AudioSource>();

        roverType = (RoverType) Random.Range(0, (int)RoverType.Count);
        visuals[(int)roverType].gameObject.SetActive(true);

        signalArgument.playerID = (int)playerIndex;
    }

	void Update () {
        if (noControl) return;

        if(use_keyboard){
            steering.x = Input.GetAxis("Horizontal");
            steering.y = Input.GetAxis("Vertical");
        }else{
            prevState = state;
            state = GamePad.GetState(playerIndex);

            if(thumbstick == Thumbstick.Left){
                steering.x = state.ThumbSticks.Left.X;
                steering.y = state.ThumbSticks.Left.Y;
            }else{
                steering.x = state.ThumbSticks.Right.X;
                steering.y = state.ThumbSticks.Right.Y;
            }
        }

        float mag = Vector2.SqrMagnitude(steering);
        motorAudio.volume = Mathf.Clamp(mag, 0, 1);
        motorAudio.pitch = Mathf.Clamp(mag, 0.8f, 1f);

        bool noInput = steering == Vector2.zero;
        steering = steering.normalized;

        if(Mathf.Abs(steering.x) == 1)
            steering.y += 0.01f;


        Vector3 steering3 = Vector3.right * steering.x + Vector3.forward * steering.y;

        float forwardDot = Vector3.Dot(steering3, transform.forward);
        float dot = Vector3.Dot(steering3, transform.right);

        if(forwardDot < settings.reverseMaxDot){
            steering3 += Mathf.Sign(dot) * transform.right * settings.reverseOffset;
            steering3 = steering3.normalized * mag;
        }

        dot = Vector3.Dot(steering3, transform.right);
        if(Mathf.Abs(dot) < settings.innerOuterDeadzone) dot = 0;

        int drivingDirection = 0;

        if(dot != 0)
            drivingDirection = (int)Mathf.Sign(dot);

        wheelsOffGround = 0;
        for(int i = 0; i < wheels.Length; i++){
            bool wheelIsRight = i % 2 == 0;

            //ground check
            bool onGround = Physics.Raycast(wheels[i].transform.position, -transform.up, settings.onGroundCheckDistance);
            Debug.DrawRay(wheels[i].transform.position, -transform.up * settings.onGroundCheckDistance, Color.red);

            if(!onGround) {
                wheelsOffGround++;
            }

            if(noInput) continue;


            float dir = Vector3.Dot(wheels[i].transform.forward, steering3);


            if(i < 2){ //assume first two wheels are front wheels and therefor rotate
                wheels[i].transform.rotation = Quaternion.LookRotation(steering3, Vector3.right);

                dir = settings.frontWheelForce.Evaluate(dir);

                Debug.DrawRay(wheels[i].transform.position + 0.5f * Vector3.up, steering3.normalized, Color.green);
            }else{
                bool drivingBackwards = dir < 0;
                dir = settings.backWheelForce.Evaluate(dir) ;
            }

            if(!onGround)continue;

            Vector3 force = wheels[i].transform.forward * settings.moveForce * dir;

            Color c;
            if((wheelIsRight && drivingDirection == 1) || (!wheelIsRight &&  drivingDirection == -1)){
                force *= settings.wheelIsInnerForce;
                c = Color.blue;
            }else if((wheelIsRight && drivingDirection == -1) || (!wheelIsRight && drivingDirection == 1)){
                force *= settings.wheelIsOuterForce;
                c = Color.red;
            }else{
                force *= settings.wheelStraightForce;
                c = Color.magenta;
            }

            if(drivingDirection == -1)
                force *= 1.1f;

            Debug.DrawRay(wheels[i].transform.position, force.normalized, c);

            wheelForce[i] = force.magnitude;

            body.AddForceAtPosition(force * Time.deltaTime, wheels[i].transform.position);
		}

        float upDot = Vector3.Dot(transform.up, Vector3.up);

		if(upDot < 0.75 && wheelsOffGround > 1 && body.velocity.magnitude < settings.flipMinVelocity){
            if(startFlipAction == -1){
                startFlipAction = Time.time + settings.flipDelay;
            }

            if(startFlipAction < Time.time){
                startFlipAction = -1;

                transform.rotation = Quaternion.Euler(0,0,0);
                transform.position += Vector3.up;
            }
        }else{
            startFlipAction = -1;
        }

        if(state.Buttons.Back == ButtonState.Pressed && prevState.Buttons.Back == ButtonState.Released){
            if(nextFlipAllowed < Time.time){
                nextFlipAllowed = Time.time + nextFlipDelay;
                transform.rotation = Quaternion.Euler(0,0,0);
                transform.position += Vector3.up;
            }
        }

        //Visual Wheels
        visuals[(int)roverType].UpdateWheels(wheels);
	}

    public void SetRoverType(RoverType type){
        visuals[(int)roverType].gameObject.SetActive(false);
        roverType = type;
        visuals[(int)roverType].gameObject.SetActive(true);
    }

    public void Vibrate(InputValues vibrationType)
    {
        switch(vibrationType)
        {
            case InputValues.Left:
                GamePad.SetVibration(playerIndex, vibrationStrength, 0.0f);
            break;

            case InputValues.Right:
                GamePad.SetVibration(playerIndex, 0.0f, vibrationStrength);
            break;
        }

        StartCoroutine(StopVibrationAfterTimer());
    }

    public IEnumerator StopVibrationAfterTimer()
    {
        yield return new WaitForSeconds(vibrationLength);

        GamePad.SetVibration(playerIndex, 0.0f, 0.0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("SignalZone"))
        {
            isWithinSignal = true;
            signalArgument.teamID = (int) roverType;
            GameEventHandler.TriggerEvent(GameEvent.SignalEnter, signalArgument);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("SignalZone"))
        {
            isWithinSignal = false;
            signalArgument.teamID = (int) roverType;
            GameEventHandler.TriggerEvent(GameEvent.SignalExit, signalArgument);
        }
    }
}
