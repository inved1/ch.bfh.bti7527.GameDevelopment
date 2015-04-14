using UnityEngine;
using System.Collections;
using UnityEditor;
public class Prefs : MonoBehaviour
{
	public static float carBodyHue;
	public static float carBodySaturation;
	public static float carBodyLuminance;
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

	}
	public static void Save()
	{ 
		PlayerPrefs.SetFloat("carBodyHue", carBodyHue);
		PlayerPrefs.SetFloat("carBodySaturation", carBodySaturation);
		PlayerPrefs.SetFloat("carBodyLuminance", carBodyLuminance);
	}
}