using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextLog : MonoBehaviour {
	public TMP_Text textLog;
	
	public int maxLines = 1;
	public bool antispam = false;
	
	public void ClearLog() {
		textLog.text = "";
	}
	
	public void PrintLine(string l) {
		if (antispam && WasJustPrinted(l)) return;
		
		if (textLog.text.Length > 0) textLog.text += '\n';
		textLog.text += l;
		
		LimitLines();
	}
	
	bool WasJustPrinted(string l) {
		int lastLine = textLog.text.LastIndexOf('\n') + 1;
		return textLog.text.Substring(lastLine) == l;
	}
	
	void LimitLines() {
		List<int> lineInstances = new List<int>();
		int nextInstance = -1;
		
		do {
			nextInstance = textLog.text.IndexOf('\n', nextInstance + 1);
			if (nextInstance < 0) break; // sentinel values are sooo 1995
			lineInstances.Add(nextInstance);
		} while (nextInstance >= 0);
		
		int[] lines = lineInstances.ToArray();
		
		if (lines.Length >= maxLines) {
			int keepPast = lines[lines.Length - maxLines];
			textLog.text = textLog.text.Substring(keepPast);
		}
	}
}
