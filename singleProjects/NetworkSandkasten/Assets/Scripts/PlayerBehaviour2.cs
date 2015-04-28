using UnityEngine;
using System.Collections;

public class PlayerBehaviour2 : MonoBehaviour
{
	public float speed = 10f;

	private float _syncTimeLast = 0f;
	private float _syncDelay = 0f;
	private float _syncTime = 0f;
	private Vector3 _syncPosStart = Vector3.zero;
	private Vector3 _syncPosEnd = Vector3.zero;

	void Update()
	{   if (GetComponent<NetworkView> ().isMine) {
			InputMovement ();
			//InputColorChange ();
		} else {
			SyncedMovement();
		}

	}

	private void SyncedMovement()
	{ _syncTime += Time.deltaTime;
		GetComponent<Rigidbody>().position = Vector3.Lerp(_syncPosStart,
		                                                  _syncPosEnd,
		                                                  _syncTime / _syncDelay);
	}

	private void InputColorChange()
	{   if (Input.GetKeyDown(KeyCode.C))
			ChangeColorTo(new Vector3(Random.value,Random.value,Random.value));
	}

//	void InputMovement()
//	{   float y = Input.GetAxis("Vertical");
//		float x = Input.GetAxis("Horizontal");
//		Rigidbody rb = GetComponent<Rigidbody>();
//		if (x != 0.0f)
//			rb.MovePosition(rb.position + Vector3.right   * x * speed * Time.deltaTime);
//		if (y != 0.0f)
//			rb.MovePosition(rb.position + Vector3.forward * y * speed * Time.deltaTime);
//	}

	[RPC] void ChangeColorTo(Vector3 color)
	{   GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z, 1f);
		if (GetComponent<NetworkView>().isMine)
			GetComponent<NetworkView>().RPC("ChangeColorTo", RPCMode.OthersBuffered, color);
	}

	void InputMovement()
	{ float y = Input.GetAxis("Vertical");
		float x = Input.GetAxis("Horizontal");
		Rigidbody rb = GetComponent<Rigidbody>();
		if (x != 0.0f)
			rb.MovePosition(rb.position + Vector3.right*x*speed*Time.deltaTime);
		if (y != 0.0f)
			rb.MovePosition(rb.position + Vector3.forward*y*speed*Time.deltaTime);
	}
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		// Outgoing sync
		Vector3 syncPos = Vector3.zero;
		if (stream.isWriting)
		{
			syncPos = GetComponent<Rigidbody>().position;
			stream.Serialize(ref syncPos);
		}
		else // Incomming sync
		{
			stream.Serialize(ref syncPos);
			_syncTime = 0f;
			_syncDelay = Time.time - _syncTimeLast;
			_syncTimeLast = Time.time;
			_syncPosStart = GetComponent<Rigidbody>().position;
			_syncPosEnd = syncPos;
		}
	}

//	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
//	{
//		Vector3 syncPos = Vector3.zero;
//
//		// Outgoing sync
//		if (stream.isWriting)
//		{   syncPos = GetComponent<Rigidbody>().position;
//			stream.Serialize(ref syncPos);
//		}
//		else // Incomming sync
//		{   stream.Serialize(ref syncPos);
//			GetComponent<Rigidbody>().position = syncPos;
//		}
//	}
}
