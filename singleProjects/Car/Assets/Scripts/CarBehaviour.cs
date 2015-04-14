using UnityEngine;
using System.Collections;

public class CarBehaviour : MonoBehaviour {
	


	
	public WheelCollider 	wheelFL;
	public WheelCollider 	wheelFR;
	public WheelCollider 	wheelRL;
	public WheelCollider 	wheelRR;
	public float			minTorque = 500f;
	public float 			maxTorque = 3000f;
	public float			normalBrake = 30000f;
	public float			fullBrake = 40000f;
	public Texture2D		guiRPMDisplay;
	public Texture2D		guiRPMPointer;
	public Texture2D        guiSpeedDisplay;
	public Texture2D        guiSpeedPointer;

	public RotatableGUIItem needleKMH;
	public RotatableGUIItem needleRPM;

	//übersetzungen engine 
	public float[]			gears = {-10f,9f,6f,4.5f,3f,2.5f};
	public bool 			automaticTransmission = true;
	public int				currentGear = 1;

	public GameObject		centerOfMass;
	public AudioClip		audioClipBrake;

	public AudioClip		audioClipEngineIdle;



	public Material			brakeLightMaterial;
	public Material 		backLightMaterial;
	public GameObject		backLightL;
	public GameObject		backLightR;



	private float myMaxSpeedKMH = 150f;
	private float myMaxRPM = 10000f;
	private float myMinRPM = 800f;
	private float myRPM = 0f;
	private float myWheelTorque = 0f;


	
	private float myShiftUpRPM = 3000f;
	private float myShiftDownRPM =1000f;

	
	private float           	myCurrentSpeedKMH;
	private Rigidbody 			myRigidBody;
	private float				myMotorTorque;

	private WheelFrictionCurve 	myFrictionFL;
	private WheelFrictionCurve 	myFrictionFR;
	private WheelFrictionCurve 	myFrictionRL;
	private WheelFrictionCurve 	myFrictionRR;
	private float				myBrakeTorque;
	private ParticleSystem		myParticleSystemDustFR;
	private ParticleSystem 		myParticleSystemDustFL;
	private ParticleSystem		myParticleSystemDustRR;
	private ParticleSystem 		myParticleSystemDustRL;
	private Vector3				myLocalVelocity;

	private AudioSource			myAudioSourceBrake;
	private AudioSource			myAudioSourceIdle;
	private Material			myBackLightMaterial;
	private AudioSource			myAudioSource;


	
	void Start ()
	{
		myRigidBody = GetComponent<Rigidbody> ();
		myParticleSystemDustFR = GameObject.Find ("DustFR").GetComponent<ParticleSystem> ();
		myParticleSystemDustFL = GameObject.Find ("DustFL").GetComponent<ParticleSystem> ();
		myParticleSystemDustRR = GameObject.Find ("DustRR").GetComponent<ParticleSystem> ();
		myParticleSystemDustRL = GameObject.Find ("DustRL").GetComponent<ParticleSystem> ();

		myAudioSource = GetComponent<AudioSource> ();

		myAudioSourceBrake = (AudioSource)gameObject.AddComponent<AudioSource>();
		myAudioSourceBrake.clip = audioClipBrake;
		myAudioSourceBrake.loop = true;
		myAudioSourceBrake.volume = 0.7f;
		myAudioSourceBrake.playOnAwake = false;


		
		
		myAudioSourceIdle = (AudioSource)gameObject.AddComponent<AudioSource>();
		myAudioSourceIdle.clip = audioClipEngineIdle;
		myAudioSourceIdle.loop = true;
		myAudioSourceIdle.volume = 0.7f;
		myAudioSourceIdle.playOnAwake = false;
		
		myAudioSource = myAudioSourceIdle;
		myAudioSourceIdle.Play ();

		myBackLightMaterial = backLightL.GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame constanc time per frame
	void FixedUpdate ()
	{


		myCurrentSpeedKMH = myRigidBody.velocity.magnitude * 3.6f;

		myLocalVelocity = transform.InverseTransformDirection (myRigidBody.velocity);



		bool velocityIsForeward = Vector3.Angle(transform.forward, myRigidBody.velocity) < 50f;

		myMotorTorque = maxTorque * Input.GetAxis("Vertical");

		//guiSpeed.text = myCurrentSpeedKMH.ToString("0") + " KMH";


		// MAX SPEED
		if (myCurrentSpeedKMH > myMaxSpeedKMH)
		{
			myRigidBody.velocity = (myMaxSpeedKMH/3.6f) * myRigidBody.velocity.normalized;
		}


		bool doBraking = (Input.GetAxis("Vertical") < 0 && velocityIsForeward ||
		                  Input.GetAxis("Vertical") > 0 && !velocityIsForeward);

		myWheelTorque = 0f;
		
		myRPM = 0f;
	
		
		//rpm			umfang 2*pi*radius  *60 in der stunde (meter) /1000 = km	
		myRPM = ((wheelFL.rpm + wheelFR.rpm) / 2 )  * 60 / 1000f; 
		myRPM = myRPM /gears[currentGear] ;

		if (doBraking || FullBrake()) {
			

			if(FullBrake())
			{
				myBrakeTorque = fullBrake;
			} 
			else
			{
				myBrakeTorque = normalBrake;
			}
			
			wheelFL.motorTorque = wheelFR.motorTorque = wheelRL.motorTorque = wheelRR.motorTorque = 0;
			wheelFL.brakeTorque =  wheelFR.brakeTorque = wheelRL.brakeTorque = wheelRR.brakeTorque =  myBrakeTorque;

			
			// SMOKE / BRAKE AUDIO
			if(myCurrentSpeedKMH > 5)
			{
				myParticleSystemDustFR.emissionRate = myParticleSystemDustFL.emissionRate = 
					myParticleSystemDustRR.emissionRate = myParticleSystemDustRL.emissionRate = Mathf.Max(myCurrentSpeedKMH * 1.3f, 2);
				
				if(wheelRL.isGrounded) myParticleSystemDustRL.enableEmission = true;
				if(wheelRR.isGrounded) myParticleSystemDustRR.enableEmission = true;
				if(wheelFL.isGrounded) myParticleSystemDustFL.enableEmission = true;
				if(wheelFR.isGrounded) myParticleSystemDustFR.enableEmission = true;
				
				if(!myAudioSourceBrake.isPlaying && wheelRL.isGrounded && wheelRR.isGrounded)
				{
					myAudioSourceBrake.Play();
				}
			}
			else
			{
				myParticleSystemDustFR.enableEmission = myParticleSystemDustFL.enableEmission = 
					myParticleSystemDustRR.enableEmission = myParticleSystemDustRL.enableEmission = false;
				
				if(myAudioSourceBrake.isPlaying)
				{
					myAudioSourceBrake.Stop();
				}
			}
			
			backLightL.GetComponent<Renderer>().material = brakeLightMaterial;
			backLightR.GetComponent<Renderer>().material = brakeLightMaterial;
			
		} else {
			

			
			wheelFL.brakeTorque = wheelFR.brakeTorque = wheelRL.brakeTorque = wheelRR.brakeTorque = 0f;
			wheelFL.motorTorque = wheelFR.motorTorque = wheelRL.motorTorque = wheelRR.motorTorque = myMotorTorque;

			// SMOKE
			myParticleSystemDustFR.enableEmission = myParticleSystemDustFL.enableEmission = 
				myParticleSystemDustRR.enableEmission = myParticleSystemDustRL.enableEmission  = false;
			
			// BRAKE AUDIO
			if(myAudioSourceBrake.isPlaying)
			{
				myAudioSourceBrake.Stop();
			}
			
			if(!velocityIsForeward)
			{
				backLightL.GetComponent<Renderer>().material = backLightMaterial;
				backLightR.GetComponent<Renderer>().material = backLightMaterial;
			}
			else
			{
				backLightL.GetComponent<Renderer>().material = myBackLightMaterial;
				backLightR.GetComponent<Renderer>().material = myBackLightMaterial;
			}
		}
		
		// STEERING
		float steerFactor = Mathf.Max (-10f / myMaxSpeedKMH * myCurrentSpeedKMH + 20f);
		wheelFL.steerAngle = steerFactor * Input.GetAxis("Horizontal");
		wheelFR.steerAngle = wheelFL.steerAngle;

	
		needleKMH.angle = myCurrentSpeedKMH + 40f;
		needleRPM.angle = myRPM + 80f; 
		//Debug.Log (myRPM);
		//Debug.Log (gears [currentGear]);

		//shifting
		if (automaticTransmission && (currentGear == 1) && !velocityIsForeward) {
			ShiftDown (); //reverse
		} else if (automaticTransmission && (currentGear == 0) && velocityIsForeward){
			ShiftUp(); //from reverse to forward
		} else if (automaticTransmission && (myRPM > myShiftUpRPM) && velocityIsForeward) {
			ShiftUp();
		} else if (automaticTransmission && (myRPM < myShiftDownRPM) && (currentGear > 1)) {
			ShiftDown();
		}



	}
	
	// OnGUI is called on every frame when the orthographic GUI is rendered
	void OnGUI() 

	{   

		GUI.Box(new Rect(0, 0, 140, 140), guiSpeedDisplay);

		GUI.Box(new Rect(150, 0, 140, 140), guiRPMDisplay);




	}
	

	void Update(){
		SetAudioPitch ();
	}

	void SetAudioPitch()
	{

	
	}
	
	
	bool FullBrake()
	{
		return Input.GetKey ("space");
	}

	void ShiftUp(){
		if (currentGear < gears.Length - 1) {
			currentGear++;
		}
	}

	void ShiftDown() {
		if (currentGear > 0) {
			currentGear--;
		}
	}

}