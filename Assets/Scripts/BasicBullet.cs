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
	Collider coll;
	
	void Awake() {
		rb = GetComponent<Rigidbody>();
		if (!rb) rb = gameObject.AddComponent<Rigidbody>();
		coll = GetComponent<Collider>();
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
				// origin.SendMessage("Hurt", origin, SendMessageOptions.DontRequireReceiver);
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
			if (origin.CompareTag("Player") || other.gameObject.CompareTag("Player")) {
				// Hack to not allow enemies to kill eachother.
			
			// Bullet Hitbox Cheese
			// DebugDrawBall(this.transform.position, this.coll.bounds.extents.magnitude);
			foreach (Collider c in Physics.OverlapSphere(
				this.transform.position, this.coll.bounds.extents.magnitude,
				1 << LayerMask.NameToLayer("Default"),
				QueryTriggerInteraction.Ignore
			)) {
				if (c.gameObject == other.gameObject) continue;
				c.gameObject.SendMessage("Hurt", origin, SendMessageOptions.DontRequireReceiver);
			}
			
			other.gameObject.SendMessage("Hurt", origin, SendMessageOptions.DontRequireReceiver);
			
			}
		}
		
		// And yeah, bullets get destroyed after they hit a thing.
		Destroy(this.gameObject);
	}
	
	// void DebugDrawBall(Vector3 position, float radius) {
	// 	Debug.DrawLine(position - Vector3.right * radius, position + Vector3.right * radius, Color.cyan, 1f);
	// 	Debug.DrawLine(position - Vector3.forward * radius, position + Vector3.forward * radius, Color.cyan, 1f);
	// 	Debug.DrawLine(position - Vector3.up * radius, position + Vector3.up * radius, Color.cyan, 1f);
	// 	
	// 	Debug.DrawLine(position - Vector3.up * radius, position + Vector3.forward * radius, Color.cyan, 1f);
	// 	Debug.DrawLine(position - Vector3.up * radius, position - Vector3.forward * radius, Color.cyan, 1f);
	// 	Debug.DrawLine(position - Vector3.up * radius, position + Vector3.right * radius, Color.cyan, 1f);
	// 	Debug.DrawLine(position - Vector3.up * radius, position - Vector3.right * radius, Color.cyan, 1f);
	// 	
	// 	Debug.DrawLine(position + Vector3.up * radius, position + Vector3.forward * radius, Color.cyan, 1f);
	// 	Debug.DrawLine(position + Vector3.up * radius, position - Vector3.forward * radius, Color.cyan, 1f);
	// 	Debug.DrawLine(position + Vector3.up * radius, position + Vector3.right * radius, Color.cyan, 1f);
	// 	Debug.DrawLine(position + Vector3.up * radius, position - Vector3.right * radius, Color.cyan, 1f);
	// }
	
	void OnTriggerExit(Collider other) {
		if (insideOrigin && IsOrigin(other.gameObject))
			insideOrigin = false;
	}
}
