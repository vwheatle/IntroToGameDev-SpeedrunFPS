using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ListLevels : MonoBehaviour {
	public GameObject button;
	public List<SceneReference> levels;
	
	private RectTransform rectTransform;
	
	void Awake() {
		rectTransform = GetComponent<RectTransform>();
	}
	
	void Start() {
		foreach (Transform child in transform)
			Destroy(child.gameObject);
		
		int i = 0;
		float cursorY = 0f;
		for (i = 0; i < levels.Count; i++) {
			GameObject newButton = Instantiate(button, this.transform);
			
			RectTransform rt = newButton.GetComponent<RectTransform>();
			rt.localPosition = new Vector2( 4f * i - 200f, -cursorY ); // hack
			cursorY += rt.rect.height;
			
			RectTransform textRt = rt.Find("ButtonText").GetComponent<RectTransform>();
			TMP_Text text = textRt.GetComponent<TMP_Text>();
			string the = levels[i].ScenePath;
			int lastSlash = the.LastIndexOf('/') + 1;
			int lastDot = the.LastIndexOf('.');
			text.text = the.Substring(lastSlash, lastDot - lastSlash);
			
			Button buttonEvents = newButton.GetComponent<Button>();
			buttonEvents.onClick.AddListener(() => SceneManager.LoadScene(the));
		}
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, cursorY);
	}
}
