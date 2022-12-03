using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {
	GameObject visual;
	
	void Awake() {
		visual = this.transform.Find("Visual").gameObject;
	}
	
	void Start() {
		
	}
	
	void Update() {
		visual.transform.eulerAngles = new Vector3(
			-75f, Time.time * 90f, 30f
		);
	}
}
