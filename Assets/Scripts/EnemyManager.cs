using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class EnemyManager : MonoBehaviour {
	public GameObject player;
	public TextLog log;
	
	public PizzaStack pizzaStack;
	
	
	[Header("Pickup Goal")]
	private int killed = 0;
	private int visited = 0;
	public int goalKills = -1;
	public int goalVisits = -1;
	public float percentKilled {
		get => goalKills > 0 ? (float)killed / goalKills : 1f;
	}
	public float percentVisited {
		get => goalVisits > 0 ? (float)visited / goalVisits : 1f;
	}
	public bool achievedGoalKills { get => killed >= goalKills; }
	public bool achievedGoalVisits { get => visited >= goalVisits; }
	
	void Start() {
		// Set the target number of pickups to collect
		// to the number of children this gameobject has,
		// if the automatic sentinel value is used.
		// Gosh I wish enums with payloads were in C#...
		
		goalKills = 0;
		goalVisits = 0;
		foreach (Transform child in transform) {
			if (child.GetComponent<BasicEnemy>()) goalKills++;
			if (child.GetComponent<Waypoint>()) goalVisits++;
		}
		
		ResetLevel();
	}
	
	void ResetLevel() {
		log.PrintLine($"{goalKills} Obstacles and {goalVisits} Deliveries.");
		
		killed = 0; visited = 0;
		foreach (Transform child in transform) {
			child.gameObject.SetActive(true);
			child.gameObject.SendMessage("ResetLevel");
		}
		
		pizzaStack.SendMessage("StackUpTo", goalVisits, SendMessageOptions.RequireReceiver);
	}
	
	void PrintWithTimestamp(string message) {
		log.PrintLine($"[{System.Math.Round(LevelManager.the.watch.elapsedTime, 2, System.MidpointRounding.AwayFromZero),6:0.00}s] " + message);
	}
	
	void CheckSatisfied() {
		if (killed == goalKills && visited == goalVisits)
			LevelManager.the.SendMessage("DonePlaying");
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
			CheckSatisfied();
		}
	}
	
	public void OnVisit(GameObject site) {
		PrintWithTimestamp($"'{site.transform.GetSiblingIndex()}' delivered.");
		visited++;
		pizzaStack.SendMessage("UpdateCount", goalVisits - visited, SendMessageOptions.RequireReceiver);
		
		if (visited == goalVisits) {
			PrintWithTimestamp("No deliveries remain.");
			CheckSatisfied();
		}
	}
}
