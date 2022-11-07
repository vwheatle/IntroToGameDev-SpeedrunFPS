using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
	CharacterController cc;
	float moveSpeed = 8f;
	float jumpHeight = 8f;
	
	bool touchingGround = false;
	
	private float swayTime = 0f;
	private float swayMagnitude = 0f;
	private float swayDampenVelocity;
	
	private Vector3 acceleration = Vector3.zero;
	private float upward = -4f;
	
	private Transform head, body;
	
	void Start() {
		cc = GetComponent<CharacterController>();
		
		head = transform.Find("Head"); // where's your head at?
		body = transform.Find("Body");
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
		
		if (touchingGround) {
			if (Input.GetButton("Jump")) {
				upward = jumpHeight;
			} else {
				upward = 0;
			}
		} else {
			upward = Mathf.Max(-8f, upward - Mathf.Abs(Time.deltaTime * 12f));
		}
		
		Vector3 movement = new Vector3(wasd.x, upward, wasd.y);
		movement = transform.localRotation * movement;
		
		// Debug Jetpack
		if (Input.GetButton("Fire1")) {
			Vector3 h = -head.forward * 16f;
			cc.Move(h * Time.deltaTime);
		}
		
		cc.Move(movement * Time.deltaTime);
		
		// == Sway body around while walking. ==
		
		// If touching ground, sway faster.
		swayTime += (touchingGround ? 1f : 0.25f) * wasd.magnitude * Time.deltaTime;
		if (wasd.sqrMagnitude > 0f) {
			swayMagnitude = Mathf.Min(swayMagnitude + wasd.magnitude * Time.deltaTime, 1f);
		} else {
			// If you've stopped moving, dampen towards resting position.
			// swayMagnitude = Mathf.Lerp(swayMagnitude, 0f, 0.1f);
			swayMagnitude = Mathf.SmoothDamp(swayMagnitude, 0f, ref swayDampenVelocity, 0.1f);
			
			// and if it's close enough to the resting position, reset the
			// clock, just in case it starts to lose precision.
			if (Mathf.Approximately(swayDampenVelocity, 0f)) swayTime = 0f;
		}
		
		// Move your body!
		body.localPosition = new Vector3(
			Mathf.Sin(Mathf.PI * swayTime * 0.4f) * swayMagnitude / 12f,
			Mathf.Abs(Mathf.Cos(Mathf.PI * swayTime * 0.4f)) * swayMagnitude / 16f,
			0f
		);
	}
}
