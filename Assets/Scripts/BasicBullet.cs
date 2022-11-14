using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour {
	GameObject origin_;
	public bool insideOrigin = true;
	
	public GameObject origin {
		get => origin_;
		set {
			insideOrigin = true;
			origin_ = value;
		}
	}
	
	Rigidbody rb;
	
	void Awake() {
		rb = gameObject.GetComponent<Rigidbody>();
		if (!rb) rb = gameObject.AddComponent<Rigidbody>();
		
		rb.useGravity = false;
		rb.isKinematic = false;
		
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		
		rb.interpolation = RigidbodyInterpolation.Interpolate;
	}
	
	public void Shoot(float force = 12f) {
		rb.AddForce(
			transform.forward * force,
			ForceMode.VelocityChange
		);
	}
	
	bool IsOrigin(GameObject other) {
		return origin && other.transform == origin.transform;
		// || other.transform.IsChildOf(origin.transform);
	}
	
	void OnTriggerEnter(Collider other) {
		if (insideOrigin && IsOrigin(other.gameObject))
			return;
		
		// TODO: what if it hurt every object in a radius..
		other.gameObject.SendMessage("Hurt", origin, SendMessageOptions.DontRequireReceiver);
		
		BasicBullet otherBullet = other.gameObject.GetComponent<BasicBullet>();
		if (otherBullet) {
			// return;
			Debug.Log($"Hurt {otherBullet.origin.name}, originator of {other.name}");
			// otherBullet.origin.SendMessage("Hurt", origin, SendMessageOptions.DontRequireReceiver);
		}
		
		// Debug.Log($"Bullet from {origin.name} hit {other.name} ({insideOrigin})");
		Destroy(this.gameObject);
	}
	
	void OnTriggerExit(Collider other) {
		if (insideOrigin && IsOrigin(other.gameObject)) {
			insideOrigin = false;
		}
	}
}
