using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {
	public SceneReference startLevel;
	
	public void ButtonStart() => SceneManager.LoadScene(startLevel);
	public void ButtonExit() => Application.Quit(0);
}
