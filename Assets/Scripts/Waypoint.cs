using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {
	EnemyManager em;
	
	public Material unvisitedMaterial;
	public Material visitedMaterial;
	
	GameObject visual;
	Renderer visualRenderer;
	ShrinkDeactivate visualShrink;
	
	AudioSource audioSource;
	
	bool visited = false;
	
	private float ignoreTime;
	
	private float offsetTime;
	
	void Awake() {
		em = GetComponentInParent<EnemyManager>();
		visual = this.transform.Find("Visual").gameObject;
		audioSource = GetComponent<AudioSource>();
		visualRenderer = visual.GetComponent<Renderer>();
		
		visualShrink = visual.AddComponent<ShrinkDeactivate>();
	}
	
	void Start() {
		offsetTime = -Mathf.Abs((transform.GetSiblingIndex() * 7.25f % 3.69f) % 1f);
	}
	
	void Update() {
		if (visited) return;
		visual.transform.eulerAngles = new Vector3(
			-75f, (offsetTime + Time.time) * 90f, 30f
		);
		if (ignoreTime > 0f) ignoreTime -= Time.deltaTime;
	}
	
	public void ResetLevel() {
		visual.SetActive(true);
		visited = false;
		ignoreTime = 0.05f;
		visualRenderer.material = unvisitedMaterial;
	}
	
	void OnTriggerEnter(Collider other) {
		if (visited) return;
		if (ignoreTime > 0f) return; // HACK :(
		if (!other.CompareTag("Player")) return;
		
		float pitchVariation = Random.value / Mathf.Min(4, em.goalKills) / 2;
		audioSource.pitch = 0.9f + em.percentKilled + pitchVariation;
		audioSource.Play();
		
		visualShrink.StartShrink();
		visualRenderer.material = visitedMaterial;
		visited = true;
		
		em.OnVisit(this.gameObject);
		other.SendMessage("PlacePizza", SendMessageOptions.DontRequireReceiver);
	}
}
