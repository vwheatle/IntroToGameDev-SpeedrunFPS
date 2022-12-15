using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SkipText : MonoBehaviour {
	TMP_Text text;
	Coroutine blink;
	
	public float interval = 0.75f;
	
	public SceneReference nextScene;
	
	void Awake() {
		text = GetComponent<TMP_Text>();
	}
	
	void Start() {
		blink = StartCoroutine( BlinkText() );
	}
	
	void Update() {
		if (Input.anyKeyDown) {
			StopCoroutine(blink);
			text.CrossFadeAlpha(1f, 0f, true);
			text.color = Color.red;
			SwitchScene();
		}
	}
	
	IEnumerator BlinkText() {
		while (true) {
			yield return new WaitForSeconds(interval);
			text.CrossFadeAlpha(0f, interval / 2, false);
			yield return new WaitForSeconds(interval);
			text.CrossFadeAlpha(1f, interval / 8, true);
		}
	}
	
	void SwitchScene() { SceneManager.LoadScene(nextScene); }
}
