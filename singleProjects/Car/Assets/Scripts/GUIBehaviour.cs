using UnityEngine;
using System.Collections;

public class GUIBehaviour : MonoBehaviour {

	public float 			pastTime = 0f;
	public GUIText	 		guiTime;
	public WheelCollider 	wheelCollider;

	private bool			myIsFinished;

	private CarBehaviour myCarScript;

	// Use this for initialization
	void Start () {
	
		myCarScript = GameObject.Find ("COSWORTH").GetComponent<CarBehaviour> ();


	}
	
	// Update is called once per frame
	void Update () {


		if (myCarScript.enabled) {


			WheelHit hit;
			if (wheelCollider.GetGroundHit (out hit)) {

				if (hit.collider.gameObject.tag == "Finish") {

					myIsFinished = true;
				}
			}
			if (!myIsFinished) {
				pastTime += Time.deltaTime;
			}


		}

		guiTime.text = pastTime.ToString ("0.00");
	
	}
}
