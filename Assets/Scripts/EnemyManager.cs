using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EnemyManager : MonoBehaviour {
	public GameObject player;
	public TextLog log;
	public TMP_Text timer;
	
	
	[Header("Pickup Goal")]
	private int killed = 0;
	private int visited = 0;
	public int goalKills = -1;
	public int goalVisits = -1;
	public float percentKilled {
		get => (float)killed / goalKills;
	}
	public bool achievedGoalKills {
		get => killed >= goalKills;
	}
	
	float startTime;
	
	private HttpClient client;
	
	void Start() {
		// : )
		client = new HttpClient();
		
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
	
	void Update() {
		timer.text = $"{Time.time - startTime,6:0.00}s";
	}
	
	void ResetLevel() {
		log.PrintLine($"{goalKills} Obstacles and {goalVisits} Deliveries.");
		
		Time.timeScale = 1f;
		killed = 0; visited = 0;
		foreach (Transform child in transform) {
			child.gameObject.SetActive(true);
			child.gameObject.SendMessage("ResetLevel");
		}
		
		startTime = Time.time;
	}
	
	void PrintWithTimestamp(string message) {
		log.PrintLine($"[{System.Math.Round(Time.time - startTime, 2, System.MidpointRounding.AwayFromZero),6:0.00}s] " + message);
	}
	
	void CheckSatisfied() {
		if (killed == goalKills && visited == goalVisits) {
			PrintWithTimestamp("Completed Order.");
			float endTime = Time.time - startTime;
			Time.timeScale = 0f;
			
			float accuracy = player.GetComponent<PlayerShoot>().accuracyRatio;
			
			log.PrintLine($"Accuracy: {accuracy * 100f:00.00}%");
			bool goodAccuracy = accuracy >= 1f;
			
			if (goodAccuracy) log.PrintLine("Nominal accuracy.");
			if (PlayerPrefs.GetInt("offline", 0) == 0)
				SendScore(endTime, goodAccuracy);
		}
	}
	
	async void SendScore(float time, bool good) {
		log.PrintLine("Sending telemetrics...");
		HttpRequestMessage request = new HttpRequestMessage(
			HttpMethod.Post, "http://localhost:8000/score/submit");
		request.Content = new FormUrlEncodedContent(new Dictionary<string, string> {
			{ "level", SceneManager.GetActiveScene().name },
			{ "time", time.ToString() },
			{ "name", PlayerPrefs.GetString("name", "Bot" + UnityEngine.Random.Range(0, 999).ToString()) },
			{ "accuracy", good ? "*" : "" }
		});
		try {
			HttpResponseMessage response = await client.SendAsync(request);
			
			if (response.IsSuccessStatusCode) {
				log.PrintMore(" Sent.");
				string message = await response.Content.ReadAsStringAsync();
				// Debug.Log(message);
				log.PrintLine(message);
			} else {
				log.PrintLine("Server failure.");
			}
		} catch (HttpRequestException ex) {
			log.PrintLine("Communication failure.");
			Debug.Log(ex.Message);
		}
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
		
		if (visited == goalVisits) {
			PrintWithTimestamp("No deliveries remain.");
			CheckSatisfied();
		}
	}
}
