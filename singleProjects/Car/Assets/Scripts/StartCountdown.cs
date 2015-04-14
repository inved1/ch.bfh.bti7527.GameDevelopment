using UnityEngine;
using System.Collections;

public class StartCountdown : MonoBehaviour {

	public int countMax = 3;

	public AudioClip		audioClipStart;


	private int myCountDown;
	private CarBehaviour myCarScript;


	private AudioSource			myAudioSourceStart;



	// Use this for initialization
	void Start () {
	
		myAudioSourceStart = (AudioSource)gameObject.AddComponent<AudioSource>();
		myAudioSourceStart.clip = audioClipStart;
		myAudioSourceStart.loop = false;
		myAudioSourceStart.volume = 0.7f;
		myAudioSourceStart.playOnAwake = false;

		myAudioSourceStart.Play ();

		Debug.Log (string.Format ("begin start, time: {0}", Time.time));

		myCarScript = GameObject.Find ("COSWORTH").GetComponent<CarBehaviour> ();

		myCarScript.enabled = false;

		StartCoroutine (GamerStart ());



		Debug.Log (string.Format ("end start, time: {0}", Time.time));

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	IEnumerator GamerStart() {

		for (myCountDown = countMax; myCountDown > 0; myCountDown--) {
			yield return new WaitForSeconds(1);

			Debug.Log (string.Format ("wait for seconds: {0}",Time.time));
		}

		myCarScript.enabled = true;


	}
}
