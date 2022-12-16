using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TelemetryInput : MonoBehaviour {
	public TMP_InputField inputName;
	public TMP_InputField inputServer;
	
	void Start() {
		inputName.text = PlayerPrefs.GetString("name", "");
		inputServer.text = PlayerPrefs.GetString("server", "");
		PlayerPrefs.SetInt("offline", inputServer.text.Length > 0 ? 0 : 1);
	}
	
	public void SetName() => PlayerPrefs.SetString("name", inputName.text);
	public void SetServer() {
		PlayerPrefs.SetString("server", inputServer.text);
		PlayerPrefs.SetInt("offline", inputServer.text.Length > 0 ? 0 : 1);
	}
}
