using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {
	EnemyManager em;
	
	GameObject visual;
	AudioSource audioSource;
	
	bool visited = false;
	
	void Awake() {
		em = GetComponentInParent<EnemyManager>();
		visual = this.transform.Find("Visual").gameObject;
		audioSource = GetComponent<AudioSource>();
	}
	
	void Start() {
		
	}
	
	void Update() {
		visual.transform.eulerAngles = new Vector3(
			-75f, Time.time * 90f, 30f
		);
	}
	
	public void ResetLevel() {
		visual.SetActive(true);
		visited = false;
	}
	
	void OnTriggerEnter(Collider other) {
		if (visited) return;
		if (!other.CompareTag("Player")) return;
		
		float pitchVariation = Random.value / Mathf.Min(4, em.goalKills) / 2;
		audioSource.pitch = 1f + em.percentKilled + pitchVariation;
		audioSource.Play();
		
		visual.SetActive(false);
		visited = true;
		
		em.OnVisit(this.gameObject);
	}
}
