using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustRotate : MonoBehaviour {
	float turnSine(float x) => Mathf.Sin(x * 2 * 3.141592f);
	
	void Update() {
		transform.localRotation = Quaternion.Euler(
			-100f + turnSine(Time.time / 5f) * 5f,
			Time.time * 60f,
			180f
		);
	}
}
