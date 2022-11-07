using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {
	Transform head, body;
	
	Vector2 rotation;
	
	// TODO: there's a built-in mouse sensitivity thing in
	//       the mouse 
	public Vector2 sensitivity = new Vector2(1.2f, 1.2f);
	public bool invertX = false, invertY = true;
	
	Vector2 invertVector() {
		return new Vector2(
			invertX ? -1f : 1f,
			invertY ? -1f : 1f
		);
	}
	
	void Start() {
		head = transform.Find("Head");
		body = transform.Find("Body");
		
		// Lock cursor inside the window.
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// more todo:
	// - https://github.com/wfowler1/Unity3D-BSP-Importer
	// - https://trenchbroom.github.io/manual/latest/index.html#game_configuration_files
	// - Documents\Apps\TrenchBroom-Win64-v2021.1-Release\games\SpeedrunFPS
	
	void Update() {
		// Based on reddit people here https://redd.it/8k7w7v but good
		
		Vector2 mouseMovement = new Vector2(
			Input.GetAxisRaw("Mouse X"),
			Input.GetAxisRaw("Mouse Y")
		);
		
		rotation += mouseMovement * sensitivity * invertVector();
		
		// Disallow upside-down neck-breaking antics.
		rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);
		
		// These also override any other rotations to the player or head.
		transform.localRotation = Quaternion.Euler(0f, rotation.x, 0f);
		head     .localRotation = Quaternion.Euler(rotation.y, 0f, 0f);
		
		body.localRotation = Quaternion.Euler(rotation.y * 0.4f, 0f, 0f);
	}
}
