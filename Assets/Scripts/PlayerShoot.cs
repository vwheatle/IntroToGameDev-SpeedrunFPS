using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour {
	public GameObject pizza;
	
	void Start() {
		
	}
	
	void Update() {
		if (Input.GetButton("Fire1")) {
			// GameObject newPizza = Instantiate(pizza, transform.position + transform.forward * 1.5f, /*transform.rotation*/ Random.rotationUniform);
			
			// Rigidbody npRb = newPizza.GetComponent<Rigidbody>();
			// npRb.AddForce(newPizza.transform.forward * 999f, ForceMode.Acceleration);
			
			// BasicLife life = newPizza.AddComponent<BasicLife>();
			// life.SetLifeTime(5f);
			// Rigidbody rb = transform.parent.GetComponent<Rigidbody>();
			// rb.AddForce(-transform.forward * 16f, ForceMode.Force);
		}
	}
}
