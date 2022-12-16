using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {
	public GameObject levelsPanel;
	
	public void ButtonStart() => levelsPanel.gameObject.SetActive(true);
	public void ButtonHide() => levelsPanel.gameObject.SetActive(false);
	public void ButtonExit() => Application.Quit(0);
	
	void Start() {
		Cursor.lockState = CursorLockMode.None;
		levelsPanel.SetActive(false);
	}
}
