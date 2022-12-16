using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelemetryInput : MonoBehaviour {
	public void SetName(string name) => PlayerPrefs.SetString("name", name);
	public void SetServer(string url) => PlayerPrefs.SetString("server", url);
}
