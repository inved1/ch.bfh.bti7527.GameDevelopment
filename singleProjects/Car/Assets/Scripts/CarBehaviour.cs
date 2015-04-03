using UnityEngine;
using System.Collections;

public class CarBehaviour : MonoBehaviour {
	public WheelCollider 	wheelFL;
	public WheelCollider 	wheelFR;
	public WheelCollider	wheelRL;
	public WheelCollider	wheelRR;
	public float 			maxTorque = 500f;
	public float 			breakTorque = 1000f;
	public GUIText 			guiSpeed;
	public Texture2D        guiSpeedDisplay;
	public Texture2D        guiSpeedPointer;
	public GameObject		centerOfMass;
	public float 			fullBrakeTorque = 5000f;
	public AudioClip		brakeAudioClip;


	public Material 		brakeLightMaterial;
	public Material			ReverseLightMaterial;
	public GameObject		backLightL;
	public GameObject		backLightR;

	private float           _currentSpeedKMH;
	private Rigidbody		_rigidBody;
	private float 			_maxSpeedKMH = 100f;
	private float 			_maxSpeedBackwardKMH = 10f;
	private Vector3 		_localVelocity;
	private bool			_isFullbreaking;
	private ParticleSystem	_DustR;
	private ParticleSystem	_DustL;
	private AudioSource 	_brakeAudioSource;

	private Material		_stdBackLightMaterial;

	
	
	
	// Use this for initialization
	void Start () {
		_rigidBody = GetComponent<Rigidbody> ();
		_rigidBody.centerOfMass = centerOfMass.transform.position - centerOfMass.transform.parent.position;
		_DustR = GameObject.Find ("DustR").GetComponent<ParticleSystem> ();
		_DustL = GameObject.Find ("DustL").GetComponent<ParticleSystem> ();

		_brakeAudioSource = (AudioSource)gameObject.AddComponent<AudioSource>();
		_brakeAudioSource.clip = brakeAudioClip;
		_brakeAudioSource.loop = true;
		_brakeAudioSource.volume = 0.7f;
		_brakeAudioSource.playOnAwake = true;

		_stdBackLightMaterial = backLightR.GetComponent<Material>();

	}
	
	// Update is called once per frame constanc time per frame
	void FixedUpdate () {
		float speed = GetComponent<Rigidbody> ().velocity.magnitude * 3.6f;
		
		_localVelocity = transform.InverseTransformDirection (_rigidBody.velocity);
		
		bool velocityIsForeward = Vector3.Angle(transform.forward, _rigidBody.velocity) < 50f;
		bool doBraking = (Input.GetAxis("Vertical") < 0 && velocityIsForeward ||
		                  Input.GetAxis("Vertical") > 0 && !velocityIsForeward);
		
		Debug.Log (string.Format ("velocityIsForeward: {0} " +
		                          "lovalVelocity: {1} " +
		                          "doBreaking: {2} " +
		                          "velocityIsForeward: {3} " + "speed: {4}", 
		                          velocityIsForeward, _localVelocity, doBraking, velocityIsForeward, speed));
		
		
		
		
		float tmpMaxTorque = maxTorque;
		float tmpBreakTorque = breakTorque;
		
		if (velocityIsForeward) {
			//driving foreward
			if (doBraking) {
				//braking
				_currentSpeedKMH = Mathf.Min (speed, _maxSpeedKMH);
				tmpBreakTorque = breakTorque * Input.GetAxis ("Vertical") * -1f;
				tmpMaxTorque = 0f;
			} else {
				//accelerate
				_currentSpeedKMH = Mathf.Min (speed, _maxSpeedKMH);
				if (_currentSpeedKMH == _maxSpeedKMH) {
					Debug.Log ("max speed erreicht");
					tmpMaxTorque = 0f;
					tmpBreakTorque = breakTorque;
				} else {
					tmpMaxTorque = maxTorque * Input.GetAxis ("Vertical");
					tmpBreakTorque = 0f;
				}
			}
		} else if (!velocityIsForeward) {
			//driving backward
			if (!doBraking) {
				//accelerate
				_currentSpeedKMH = Mathf.Min (speed, _maxSpeedBackwardKMH);
				if (_currentSpeedKMH == _maxSpeedBackwardKMH) {
					Debug.Log ("max reverse speed erreicht");
					tmpMaxTorque = maxTorque * Input.GetAxis ("Vertical");
					tmpBreakTorque = 0f;
				} else {
					tmpBreakTorque = 0f;
					tmpMaxTorque = breakTorque * Input.GetAxis ("Vertical");
				}
				
			} else {
				//braking
				_currentSpeedKMH = Mathf.Min (speed, _maxSpeedBackwardKMH);
				tmpMaxTorque = 0f;
				tmpBreakTorque = maxTorque * Input.GetAxis ("Vertical");
				
			}
		} else {
			//probably standing still
		}
		
		wheelFL.motorTorque = tmpMaxTorque;
		wheelFL.brakeTorque = tmpBreakTorque;
		wheelRL.motorTorque = wheelRR.motorTorque = wheelFR.motorTorque = wheelFL.motorTorque;
		wheelRL.brakeTorque = wheelRR.brakeTorque = wheelFR.brakeTorque = wheelFL.brakeTorque;
		
		wheelFR.steerAngle = wheelFL.steerAngle = 10 * Input.GetAxis ("Horizontal");


		_isFullbreaking = FullBreaking();
		
		
		guiSpeed.text = _currentSpeedKMH.ToString("0");
	}
	
	// OnGUI is called on every frame when the orthographic GUI is rendered
	void OnGUI() 
	{   GUI.Box(new Rect(0, 0, 140, 140), guiSpeedDisplay);
		GUIUtility.RotateAroundPivot(Mathf.Abs(_currentSpeedKMH) + 40, new Vector2(70,70));
		GUI.DrawTexture(new Rect(0, 0, 140, 140), guiSpeedPointer, ScaleMode.StretchToFill);
	}

	void Update(){
		SetAudioPitch ();
	}
	void SetAudioPitch(){
		float gearSpeedDelta = 25.0f;
		int gear = System.Math.Min ((int)(_currentSpeedKMH / gearSpeedDelta), 5);
		float gearSpeedMin = gear * gearSpeedDelta;
		GetComponent<AudioSource> ().pitch = (_currentSpeedKMH - gearSpeedMin) / gearSpeedDelta * 0.5f + 0.4f;


	}
	bool FullBreaking()
	{
		if (Input.GetKey("space"))
		{ 
			wheelRL.brakeTorque = wheelFL.brakeTorque = wheelRL.brakeTorque = wheelRR.brakeTorque =  fullBrakeTorque;
			_DustR.enableEmission = _DustL.enableEmission = true;
			_brakeAudioSource.Play();
			return true;
		}
		else
		{ 
			wheelRL.brakeTorque = wheelFL.brakeTorque = wheelRL.brakeTorque = wheelRR.brakeTorque =  0f;
			_DustL.enableEmission = _DustR.enableEmission = false;
			_brakeAudioSource.Stop();
			return false;
		}
	}
}