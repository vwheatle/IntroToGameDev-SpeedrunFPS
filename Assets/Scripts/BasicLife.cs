using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLife : MonoBehaviour {
	public float deathTime = 10f;
	private float startTime = 0f;
	
	void Wake() {
		startTime = Time.unscaledTime;
	}
	
	public void SetLifeTime(float seconds) {
		startTime = Time.unscaledTime;
		deathTime = seconds;
	}
	
	void LateUpdate() {
		if ((Time.unscaledTime - startTime) > deathTime) {
			Destroy(gameObject);
		}
	}
}
