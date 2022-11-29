using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour {
	// Which object created this bullet?
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
	}
	
	void Start() {
		rb.useGravity = false;
		rb.isKinematic = false;
		
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		
		rb.interpolation = RigidbodyInterpolation.Interpolate;
	}
	
	void ResetLevel() {
		Destroy(this.gameObject);
	}
	
	// Actually cause the bullet to move forward.
	public void Shoot(float force) {
		rb.AddForce(
			transform.forward * force,
			ForceMode.VelocityChange
		);
	}
	
	// Is the given GameObject actually this bullet's origin?
	bool IsOrigin(GameObject other) {
		return origin && other.transform == origin.transform;
		// || other.transform.IsChildOf(origin.transform);
		// // uncomment to allow origin children to be valid too.
	}
	
	void OnTriggerEnter(Collider other) {
		// If the bullet is inside the origin, disregard the collision.
		if (insideOrigin && IsOrigin(other.gameObject))
			return;
		
		// (No variable shadowing in C# so I have to resort to pun names...)
		BasicBullet bother = other.gameObject.GetComponent<BasicBullet>();
		if (bother) {
			// If this bullet hit another bullet...
			
			// if they both originate from the same person, wtf.
			if (origin == bother.origin) {
				origin.SendMessage("Hurt", origin, SendMessageOptions.DontRequireReceiver);
			} else {
				// Again, if this bullet hit another bullet...
				
				// ...and this bullet is from a player...
				if (origin.CompareTag("Player"))
					// ...hurt the non-player entity.
					bother.origin.SendMessage("Hurt", origin, SendMessageOptions.DontRequireReceiver);
				
				// ...and the other bullet is from a player...
				if (bother.origin.CompareTag("Player"))
					// ...hurt the non-player entity.
					origin.SendMessage("Hurt", bother.origin, SendMessageOptions.DontRequireReceiver);
			}
			
			// Interacting with the other bullet destroys it.
			Destroy(other.gameObject);
		} else {
			// TODO: what if bullets hurt every object in a radius..
			// (maybe using Physics.CollideSphere or something.)
			other.gameObject.SendMessage("Hurt", origin, SendMessageOptions.DontRequireReceiver);
		}
		
		// And yeah, bullets get destroyed after they hit a thing.
		Destroy(this.gameObject);
	}
	
	void OnTriggerExit(Collider other) {
		if (insideOrigin && IsOrigin(other.gameObject))
			insideOrigin = false;
	}
}
