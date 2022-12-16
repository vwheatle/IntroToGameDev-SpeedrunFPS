using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
	private static LevelManager me;
	public static LevelManager the { get => me; }
	void Awake() {
		me = this;
		player = GameObject.FindGameObjectWithTag("Player")
			.transform.root.gameObject;
	}
	
	public enum State {
		Ready, Playing, /*Killed,*/ Done
	}
	
	private State currentState = State.Ready;
	
	public State state { get => currentState; }
	
	private Coroutine whateverImDoing;
	
	private GameObject player;
	public TextLog log;
	public Stopwatch watch;
	
	void PrintWithTimestamp(string message) {
		double time = System.Math.Round(watch.elapsedTime, 2, System.MidpointRounding.AwayFromZero);
		log.PrintLine($"[{time,6:0.00}s] " + message);
	}
	
	IEnumerator Start() {
		currentState = State.Ready;
		
		yield return new WaitForEndOfFrame();
		ResetEverything();
	}
	
	void StartPlaying() {
		Debug.Assert(currentState == State.Ready, 
			"Please only transition from ready -> playing!");
		currentState = State.Playing;
		watch.ResetWatch();
		watch.StartWatch();
	}
	
	// void Killed() {
	// 	Debug.Assert(currentState == State.Playing,
	// 		"Please only transition from playing -> killed!");
	// 	currentState = State.Killed;
	// }
	
	void DonePlaying() {
		Debug.Assert(currentState == State.Playing,
			"Please only transition from playing -> done!");
		
		currentState = State.Done;
		watch.StopWatch();
		PrintWithTimestamp("Completed Order.");
		
		float accuracy = player.GetComponent<PlayerShoot>().accuracyRatio;
		log.PrintLine($"Accuracy: {accuracy * 100f:00.00}%");
		
		bool goodAccuracy = accuracy >= 1f;
		if (goodAccuracy) log.PrintLine("Nominal accuracy.");
		
		if (PlayerPrefs.GetInt("offline", 0) == 0)
			StartCoroutine( SendScore(watch.elapsedTime, goodAccuracy) );
	}
	
	public void ResetEverything(string rationale = null) {
		watch.StopWatch();
		log.ClearLog();
		if (rationale != null)
			log.PrintLine($"System Restarted.\n(Reason: {rationale})");
		else
			log.PrintLine("System Initialized.");
		
		GameObject[] rootSiblings = SceneManager.GetActiveScene().GetRootGameObjects();
		foreach (GameObject rootSibling in rootSiblings)
			rootSibling.BroadcastMessage("ResetLevel", SendMessageOptions.DontRequireReceiver);
	}
	
	void ResetLevel() {
		if (whateverImDoing != null)
			StopCoroutine(whateverImDoing);
		watch.ResetWatch();
		whateverImDoing = StartCoroutine( ReadyState() );
	}
	
	IEnumerator ReadyState() {
		currentState = State.Ready;
		yield return new WaitForSeconds(0.125f);
		while (!Input.anyKey)
			yield return new WaitForFixedUpdate();
		StartPlaying();
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
}
