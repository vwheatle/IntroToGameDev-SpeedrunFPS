using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReadyText : MonoBehaviour {
	private TextLog log;
	
	Coroutine animCoroutine, endAnimCoroutine;
	
	void Awake() {
		log = GetComponent<TextLog>();
	}
	
	void Update() {
		if (animCoroutine != null
		&& LevelManager.the.state == LevelManager.State.Playing) {
			if (endAnimCoroutine != null) {
				StopCoroutine(endAnimCoroutine);
				endAnimCoroutine = null;
			}
			StopCoroutine(animCoroutine);
			animCoroutine = null;
			HideText();
		}
		if (endAnimCoroutine == null
		&& LevelManager.the.state == LevelManager.State.Done) {
			if (animCoroutine != null) {
				StopCoroutine(animCoroutine);
				animCoroutine = null;
			}
			endAnimCoroutine = StartCoroutine( PostGameAnimation() );
		}
	}
	
	void ResetLevel() {
		if (animCoroutine != null)
			StopCoroutine(animCoroutine);
		if (endAnimCoroutine != null)
			StopCoroutine(endAnimCoroutine);
		animCoroutine = StartCoroutine( Animation() );
		endAnimCoroutine = null;
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
	
	IEnumerator PostGameAnimation() {
		log.ClearLog();
		log.animateNewLines = true;
		log.blinker = true;
		yield return log.PrintLineAndWait("Complete!");
		yield return new WaitForSeconds(0.75f);
		
		// shhhh
		log.animateNewLines = false;
		log.PrintMore("<size=16>");
		log.animateNewLines = true;
		
		yield return log.PrintLineAndWait("Press any key...");
		yield return new WaitWhile( () => LevelManager.the.state == LevelManager.State.Done );
		log.animateNewLines = false;
		HideText();
	}
	
	void HideText() {
		log.ClearLog();
		log.blinker = false;
		log.Flush();
	}
}
