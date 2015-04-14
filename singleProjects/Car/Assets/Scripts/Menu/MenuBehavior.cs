using UnityEngine;
using System.Collections;

public class MenuBehavior : MonoBehaviour {

	public Material carBodyMaterial;

	// Use this for initialization
	void Start () {
		Prefs.Load ();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void OnButtonStartClick() {
		Prefs.Save ();

		Application.LoadLevel (0);

	}

	public void OnExitButtonClick(){
		Prefs.Save ();
		Application.Quit ();
	}

	public void OnBackToMenuButtonClick(){
		Application.LoadLevel (1);
	}

	public void OnSliderHueChanged(float hue){
		//, float luminance, float saturation
		Prefs.carBodyHue = hue;
		//Prefs.carBodyLuminance = luminance;
		//Prefs.carBodySaturation = saturation;

		Prefs.SetBodyMaterial (ref carBodyMaterial);


	}

	public void OnSliderSaturationChanged(float saturation){
		//, float luminance, float saturation
		//Prefs.carBodyHue = hue;
		//Prefs.carBodyLuminance = luminance;
		Prefs.carBodySaturation = saturation;
		
		Prefs.SetBodyMaterial (ref carBodyMaterial);
		
		
	}

	public void OnSliderValueChanged(float luminance){
		//, float luminance, float saturation
		//Prefs.carBodyHue = hue;
		Prefs.carBodyLuminance = luminance;
		//Prefs.carBodySaturation = saturation;
		
		Prefs.SetBodyMaterial (ref carBodyMaterial);
		
		
	}

	public void OnSliderDistanceChanged(float distance){

	}

	public void OnSliderForceChanged(float force){

	}

	public void OnSliderSuspensionChanged(float suspension){

	}
}
