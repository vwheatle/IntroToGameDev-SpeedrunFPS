using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReadyText : MonoBehaviour {
	private TextLog log;
	
	Coroutine animCoroutine;
	
	void Awake() {
		log = GetComponent<TextLog>();
	}
	
	void Update() {
		if (animCoroutine != null
		&& LevelManager.the.state != LevelManager.State.Ready) {
			StopCoroutine(animCoroutine);
			animCoroutine = null;
			HideText();
		}
	}
	
	void ResetLevel() {
		if (animCoroutine != null)
			StopCoroutine(animCoroutine);
		animCoroutine = StartCoroutine( Animation() );
	}
	
	IEnumerator Animation() {
		log.ClearLog();
		log.animateNewLines = true;
		log.blinker = true;
		yield return log.PrintLineAndWait("Ready?");
		yield return new WaitForSeconds(0.75f);
		
		// shhhh
		log.animateNewLines = false;
		log.PrintMore("<size=16>");
		log.animateNewLines = true;
		
		yield return log.PrintLineAndWait("Press any key...");
		yield return new WaitWhile( () => LevelManager.the.state == LevelManager.State.Ready );
		log.animateNewLines = false;
		HideText();
	}
	
	void HideText() {
		log.ClearLog();
		log.blinker = false;
	}
}
