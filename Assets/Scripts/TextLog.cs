using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

class TextLogItem {
	public TextLogItem(string text, bool animate = true) {
		this.time = 0f;
		this.text = text;
		this.done = !animate;
	}
	
	public float time;
	public string text;
	public bool done;
}

public class TextLog : MonoBehaviour {
	public TMP_Text textLog;
	private List<TextLogItem> buffer;
	
	public int maxLines = 1;
	public bool antispam = false;
	
	public bool animateNewLines = true;
	
	public bool blinker = false;
	public float blinkerBlinkInterval = 1/4f;
	
	public float charsPerSecond = 32f;
	
	void Awake() {
		buffer = new List<TextLogItem>();
	}
	
	void Update() {
		textLog.text = "";
		
		// moved up here because otherwise it'd double-print the cursor..
		bool lastDone = buffer.Count == 0 || buffer[buffer.Count - 1].done;
		
		int i = 0;
		for (i = 0; i < buffer.Count; i++) {
			if (textLog.text.Length > 0) textLog.text += "\n";
			
			if (buffer[i].done) {
				textLog.text += buffer[i].text;
			} else {
				float speed = 1f + (buffer.Count - i) * 0.75f;
				buffer[i].time += Time.unscaledDeltaTime * speed * charsPerSecond;
				
				int amount = Mathf.Min(Mathf.CeilToInt(buffer[i].time), buffer[i].text.Length);
				textLog.text += buffer[i].text.Substring(0, amount) + "█";
				if (amount >= buffer[i].text.Length) buffer[i].done = true;
			}
		}
		
		if (blinker && lastDone) {
			bool blink = Time.unscaledTime % (blinkerBlinkInterval * 2) < blinkerBlinkInterval;
			textLog.text += blink ? "█" : " ";
		}
	}
	
	public void ClearLog() {
		buffer.Clear();
		textLog.text = "";
	}
	
	public void PrintMore(string lines) {
		bool youAlreadyAddedOne = false;
		foreach (var line in lines.Split('\n')) {
			if (buffer.Count <= 0 || youAlreadyAddedOne) {
				buffer.Add(new TextLogItem(line, animateNewLines));
			} else {
				TextLogItem last = buffer[buffer.Count - 1];
				
				last.done = animateNewLines;
				last.time = (float)last.text.Length;
				last.text += line;
				
				youAlreadyAddedOne = true;
			}
		}
		LimitLines();
	}
	
	public void PrintChar(char c) {
		if (buffer.Count <= 0) {
			buffer.Add(new TextLogItem(c.ToString(), animateNewLines));
		} else {
			TextLogItem last = buffer[buffer.Count - 1];
			
			last.done = animateNewLines;
			last.time = (float)last.text.Length;
			last.text += c;
		}
	}
	
	public void PrintLine(string lines) {
		foreach (var line in lines.Split('\n')) {
			if (antispam && WasJustPrinted(line)) continue;
			buffer.Add(new TextLogItem(line, animateNewLines));
		}
		LimitLines();
	}
	
	public IEnumerator PrintLineAndWait(string lines) {
		foreach (var line in lines.Split('\n')) {
			PrintLine(line);
			if (buffer.Count <= 0) yield break;
			
			TextLogItem last = buffer[buffer.Count - 1];
			while (!last.done) {
				yield return new WaitForFixedUpdate();
			}
		}
	}
	
	public void PrintBreak() => buffer.Add(new TextLogItem("", false));
	
	bool WasJustPrinted(string l) {
		return buffer.Count > 0 && buffer[buffer.Count - 1].text == l;
	}
	
	void LimitLines() {
		buffer.RemoveRange(0, Mathf.Max(0, buffer.Count - maxLines));
	}
}
