using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
	CharacterController cc;
	float moveSpeed = 8f;
	float jumpHeight = 8f;
	
	bool touchingGround = false;
	
	private float upward = -4f;
	
	void Start() {
		cc = GetComponent<CharacterController>();
	}
	
	void Update() {
		RaycastHit hit;
		touchingGround = Physics.Raycast(
			new Ray(transform.position, Vector3.down),
			out hit, cc.height / 2 + 0.1f
		);
		
		Vector2 wasd = new Vector2(
			Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical")
		)/*.normalized*/ * moveSpeed;
		
		if (touchingGround && Input.GetButton("Jump")) {
			upward = jumpHeight;
		} else {
			upward = Mathf.Max(-8f, upward - Mathf.Abs(Time.deltaTime * 12f));
		}
		
		Vector3 movement = new Vector3(wasd.x, upward, wasd.y);
		movement = transform.localRotation * movement;
		
		cc.Move(movement * Time.deltaTime);
	}
}
