﻿using UnityEngine;
using System.Collections;

public class WheelBehaviour : MonoBehaviour {
	public WheelCollider wheelCol; // wheel colider object
	// Use this for initialization
	private SkidmarkBehaviour _skidmarks; // skidmark script
	private int _skidmarkLast; // index of last skidmark
	private Vector3 _skidmarkLastPos;// position of last skidmark


	void Start ()
	{
		// Get skidmarks script (not available in sceneMenu)
		GameObject skidmarksGO = GameObject.Find("Skidmarks");
		if (skidmarksGO)
			_skidmarks = skidmarksGO.GetComponent<SkidmarkBehaviour>();
			_skidmarkLast = -1;
	}

	// Update is called once per frame
	void Update ()
	{
		// Get the wheel position and rotation from the wheelcolider
		Quaternion quat;
		Vector3 position;
		wheelCol.GetWorldPose(out position, out quat);
		transform.position = position;

		if (Application.loadedLevel != 1) {
			transform.rotation = quat;
		}

		WheelHit hit;
		if (wheelCol.GetGroundHit(out hit))
			DoSkidmarking(hit);


	}
	void DoSkidmarking(WheelHit hit)
	{
		// absolute velocity at wheel in world space
		Vector3 wheelVelo = wheelCol.attachedRigidbody.GetPointVelocity(hit.point);
		if(Input.GetKey("space"))
		{ if (Vector3.Distance(_skidmarkLastPos, hit.point) > 0.1f)			
			{ 
				Vector3 tmpPosNew = hit.point;
				if (wheelCol.name.Substring(1,1) == "R") {
					tmpPosNew.x = tmpPosNew.x +- 0.15f;
				}
				else {
					tmpPosNew.x = tmpPosNew.x + 0.15f;
				}

				_skidmarkLast = _skidmarks.Add(tmpPosNew + wheelVelo*Time.deltaTime,
				                                 hit.normal,
				                                 0.5f,
				                                 _skidmarkLast);
				_skidmarkLastPos = hit.point;
			}
		} else _skidmarkLast = -1;
	}
}