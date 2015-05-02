using UnityEngine;
using System.Collections;
using UnityEditor;
public class Prefs : MonoBehaviour
{
	public static float carBodyHue;
	public static float carBodySaturation;
	public static float carBodyLuminance;

	public static float suspensionDistance;
	public static float suspensionSpring;
	public static float suspensionDamper;


	public static void SetBodyMaterial(ref Material bodyMat)
	{ 
		bodyMat.color = EditorGUIUtility.HSVToRGB(carBodyHue,
		                                            carBodySaturation,
		                                            carBodyLuminance);
	}
	public static void Load()
	{ 

		carBodyHue = PlayerPrefs.GetFloat("carBodyHue", 0.0f);
		carBodySaturation = PlayerPrefs.GetFloat("carBodySaturation", 1.0f);
		carBodyLuminance = PlayerPrefs.GetFloat("carBodyLuminance", 1.0f);

		suspensionSpring = PlayerPrefs.GetFloat ("suspensionSpring",35000f);
		suspensionDistance = PlayerPrefs.GetFloat ("suspensionDistance", 0.2f);
		suspensionDamper = PlayerPrefs.GetFloat ("suspensionDamper", 4500f);

	}
	public static void Save()
	{ 
		PlayerPrefs.SetFloat("carBodyHue", carBodyHue);
		PlayerPrefs.SetFloat("carBodySaturation", carBodySaturation);
		PlayerPrefs.SetFloat("carBodyLuminance", carBodyLuminance);

		PlayerPrefs.SetFloat ("suspensionDistance", suspensionDistance);
		PlayerPrefs.SetFloat ("suspensionDamper", suspensionDamper);
		PlayerPrefs.SetFloat ("suspensionSpring", suspensionSpring);
	}

	public static void setSuspension(ref WheelCollider FL, ref WheelCollider FR, ref WheelCollider RR, ref WheelCollider RL)
	{
		FL.suspensionDistance = FR.suspensionDistance = RR.suspensionDistance = RL.suspensionDistance = suspensionDistance;
		JointSpring JFL = new JointSpring ();
		JointSpring JFR = new JointSpring ();
		JointSpring JRR = new JointSpring ();
		JointSpring JRL = new JointSpring (); 



		JFL.spring = JFR.spring = JRR.spring = JRL.spring = suspensionSpring;
		JFL.damper = JFR.damper = JRR.damper = JRL.damper = suspensionDamper;
		FL.suspensionSpring = JFL;
		RR.suspensionSpring = JRR;
		FR.suspensionSpring = JFR;
		RL.suspensionSpring = JRL;
	}
}