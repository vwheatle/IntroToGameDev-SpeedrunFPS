using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkDeactivate : MonoBehaviour {
	bool _active = false;
	
	public bool active { get => _active; }
	
	float startTime = 0f;
	public float deathTime = 0.25f;
	
	Vector3 initialScale = Vector3.one;
	
	new Collider collider;
	// omg c# who asked
	// (loll the warning was referring to a removed field...)
	
	void Start() {
		collider = GetComponent<Collider>();
		initialScale = transform.localScale;
	}
	
	// Shrink into nothing and then deactivate object.
	public void StartShrink() {
		if (collider) collider.enabled = false;
		
		startTime = Time.unscaledTime;
		_active = true;
	}
	
	public void Reset() {
		transform.localScale = initialScale;
		gameObject.SetActive(true);
		_active = false;
		
		if (collider) collider.enabled = true;
	}
	
	void LateUpdate() {
		if (!_active) return;
		
		float diffTime = Time.unscaledTime - startTime;
		float percentDead = diffTime / deathTime;
		
		float scalarScale = Mathf.Max(0f, 1f - percentDead);
		transform.localScale = initialScale * scalarScale;
		
		if (percentDead >= 1f) {
			gameObject.SetActive(false);
			_active = false;
		}
	}
}
