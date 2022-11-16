using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyManager : MonoBehaviour {
	public GameObject player;
	public TextLog log;
	public TMP_Text timer;
	
	
	[Header("Pickup Goal")]
	private int killed = 0;
	public int goalKills = -1;
	public float percentKilled {
		get => (float)killed / goalKills;
	}
	public bool achievedGoalKills {
		get => killed >= goalKills;
	}
	
	float startTime;
	
	void Start() {
		// Set the target number of pickups to collect
		// to the number of children this gameobject has,
		// if the automatic sentinel value is used.
		// Gosh I wish enums with payloads were in C#...
		if (goalKills < 0)
			goalKills = transform.childCount;
		
		Reset();
	}
	
	void Update() {
		timer.text = $"{Time.time - startTime,6:0.00}s";
	}
	
	void Reset() {
		log.PrintLine($"{goalKills} Obstacles.");
		
		killed = 0;
		foreach (Transform child in transform) {
			child.gameObject.SetActive(true);
			child.gameObject.SendMessage("Reset");
		}
		
		startTime = Time.time;
	}
	
	void PrintWithTimestamp(string message) {
		log.PrintLine($"[{Time.time - startTime,6:0.00}s] " + message);
	}
	
	public void OnKill(GameObject victim, GameObject killer = null) {
		if (killer.CompareTag("Player")) {
			PrintWithTimestamp($"Dismissed '{victim.transform.GetSiblingIndex()}'.");
		} else {
			PrintWithTimestamp($"'{victim.transform.GetSiblingIndex()}' was dismissed.");
		}
		
		killed++;
		
		if (killed == goalKills) {
			PrintWithTimestamp("No obstacles remain.");
		}
	}
}
