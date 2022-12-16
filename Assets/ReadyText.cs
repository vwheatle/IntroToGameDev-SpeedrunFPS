using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReadyText : MonoBehaviour {
	TMP_Text text;
	
	void Awake() => text = GetComponent<TMP_Text>();
	
	void Update() {
		text.alpha = LevelManager.the.state == LevelManager.State.Ready ? 1f : 0f;
	}
	
	void ResetLevel() {
		text.alpha = 1f;
	}
}
