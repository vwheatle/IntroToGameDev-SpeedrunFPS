using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stopwatch : MonoBehaviour {
	TMP_Text text;
	
	float? startTime;
	float? endTime;
	
	public float elapsedTime {
		get {
			if (!startTime.HasValue) return 0f;
			float endTimeUnwrap = 
				endTime.HasValue ? endTime.Value : Time.time;
			return Mathf.Max(0f, endTimeUnwrap - startTime.Value);
		}
	}
	
	void Awake() {
		text = GetComponent<TMP_Text>();
	}
	
	public void ResetWatch() => startTime = endTime = null;
	
	public void StartWatch() => startTime = Time.time;
	public void StopWatch() => endTime = Time.time; // ha ha.
	
	void Update() {
		text.text = $"{elapsedTime,6:0.00}s";
	}
}
