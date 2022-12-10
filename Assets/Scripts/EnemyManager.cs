using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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
				StartCoroutine(SendScore(endTime, goodAccuracy));
		}
	}
	
	IEnumerator SendScore(float time, bool good) {
		log.PrintLine("Sending telemetrics...");
		log.blinker = true;
		
		// holy cow unity's naming schemes are very eclectic.
		WWWForm form = new WWWForm();
		form.AddField("level", SceneManager.GetActiveScene().name);
		form.AddField("time", time.ToString());
		form.AddField("name", PlayerPrefs.GetString("name", "Bot" + UnityEngine.Random.Range(0, 999).ToString()));
		form.AddField("accuracy", good ? "*" : "");
		
		UnityWebRequest www = UnityWebRequest.Post("http://localhost:8000/score/submit", form);
		www.useHttpContinue = false;
		yield return www.SendWebRequest();
		
		log.blinker = false;
		if (www.isNetworkError) {
			log.PrintLine("Network failure.");
			Debug.Log(www.error);
		} else if (www.isHttpError) {
			log.PrintLine("Server failure.");
			Debug.Log(www.error);
		} else {
			log.PrintMore(" Sent.");
			if (www.downloadHandler.isDone) {
				string message = www.downloadHandler.text;
				log.PrintLine(message);
			} else {
				log.PrintLine("ohhhm y god unity please.");
			}
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
