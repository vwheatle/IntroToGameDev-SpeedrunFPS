using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// struct PlayerBonkInfo {
// 	bool aroundHead, aroundBody, aroundFeet;
	
// 	static PlayerBonkInfo FromDirection(Vector3 direction) {
// 	}
// };

public class PlayerMove : MonoBehaviour {
	CharacterController cc;
	
	public TextLog log;
	
	private Vector3 startPosition;
	private float startRotationY;
	
	public float moveSpeed = 8f;
	public float jumpHeight = 8.1f;
	
	bool touchingGround = false;
	
	private float upward = -4f;
	
	// Body Sway
	private float swayTime = 0f;
	private float swayMagnitude = 0f;
	private float swayDampenVelocity;
	
	private Transform head, body;
	
	void Start() {
		startPosition = transform.localPosition;
		startRotationY = transform.localEulerAngles.y;
		
		cc = GetComponent<CharacterController>();
		
		head = transform.Find("Head"); // where's your head at?
		body = transform.Find("Body");
		
		ResetLevel();
	}
	
	void ResetLevel() {
		transform.localPosition = startPosition;
		// rotation resets are handled in mouselook...
		
		// https://forum.unity.com/threads/36149/#post-5360436
		Physics.SyncTransforms();
		
		upward = 0f;
	}
	
	void Update() {
		// RaycastHit hit;
		// touchingGround = Physics.Raycast(
		// 	new Ray(transform.position, Vector3.down),
		// 	out hit, cc.height / 2 + 0.1f
		// );
		
		touchingGround = cc.isGrounded;
		
		Vector2 wasd = new Vector2(
			Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical")
		).normalized * moveSpeed * Mathf.Sqrt(2);
		// always strafewalking speed,
		
		if (touchingGround) {
			if (Input.GetButton("Jump")) {
				upward = jumpHeight;
				touchingGround = false;
			} else {
				upward = 0;
			}
		} else {
			upward = Mathf.Max(-8f, upward - Mathf.Abs(Time.deltaTime * 12f));
		}
		
		Vector3 movement = new Vector3(wasd.x, upward, wasd.y);
		movement = transform.localRotation * movement;
		
		cc.Move(movement * Time.deltaTime);
		
		// == Sway body around while walking. ==
		
		// If touching ground, sway faster.
		swayTime += (touchingGround ? 1f : 0.25f) * wasd.magnitude * Time.deltaTime;
		if (wasd.sqrMagnitude > 0f) {
			swayMagnitude = Mathf.Min(swayMagnitude + wasd.magnitude * Time.deltaTime, 1f);
		} else if (Time.timeScale > 0.25f) { // HACK
			// If you've stopped moving, dampen towards resting position.
			// swayMagnitude = Mathf.Lerp(swayMagnitude, 0f, 0.1f);
			swayMagnitude = Mathf.SmoothDamp(swayMagnitude, 0f, ref swayDampenVelocity, 0.1f);
			
			// and if it's close enough to the resting position, reset the
			// clock, just in case it starts to lose precision.
			if (Mathf.Approximately(swayDampenVelocity, 0f)) swayTime = 0f;
		}
		
		// Move your body!
		body.localPosition = head.localPosition + new Vector3(
			Mathf.Sin(Mathf.PI * swayTime * 0.4f) * swayMagnitude / 12f,
			Mathf.Abs(Mathf.Cos(Mathf.PI * swayTime * 0.4f)) * swayMagnitude / 16f,
			0f
		);
		// body.localEulerAngles += Vector3.up *
		// 	Mathf.Sin(Mathf.PI * swayTime * 0.1f) * swayMagnitude * 3f;
	}
}
