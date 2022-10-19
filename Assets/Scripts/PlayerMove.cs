using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
	CharacterController cc;
	float moveSpeed = 4f;
	
	void Start() {
		cc = GetComponent<CharacterController>();
	}
	
	void Update() {
		Vector2 wasd = new Vector2(
			Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical")
		).normalized;
		
		Vector3 movement = new Vector3(wasd.x, -1f, wasd.y) * moveSpeed;
		movement = transform.localRotation * movement;
		
		Vector3 gravity = new Vector3(0f, -1f, 0f);
		movement += gravity;
		
		cc.Move(movement * Time.deltaTime);
	}
}
