using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCrawl : MonoBehaviour {
	TextLog log;
	SkipText skip;
	
	void Awake() {
		log = GetComponent<TextLog>();
		skip = transform.parent.GetComponentInChildren<SkipText>();
	}
	
	void Start() {
		StartCoroutine( TextCrawl() );
	}
	
	static readonly (string, float)[] crawlLines = {
		// 48 column limit..
		("It is the year 30,828 AD.", 0.75f),
		("Humans are long extinct, but artifacts\nof their time on Earth still remain.", 1.5f),
		("One of them: the numerous robots built for\nmundane labor, such as food processing,\nbuilding repairs, maintaining server rooms,\nand more.", 1.5f),
		("Humans have not been kind to Earth, and\nconditions have become less optimal\nfor the robots' operation.", 1.5f),
		("The planet's atmosphere, while not\ncompletely diminished, has become less and less\nable to withstand high-energy cosmic rays.", 1.5f),
		("The cosmic rays, growing in frequency, have the\npotential to cause faults in the memory banks\nof computer systems. These faults manifest as\nerratic behavior.", 1.5f),
		("Robots exhibiting this behavior tend to create\na \"domino effect\" of errors, causing other\nuncorrupted systems to misbehave in turn.", 1.5f),
		("Somewhere, an automated pizza delivery system\nroars to life with new orders from nowhere.\nIngredients are sourced, synthesized, and\ndelivered in a matter of minutes.", 1.5f),
		("Eventually a stack of pizzas is in\na delivery robot's hands.", 1.5f),
		("Let's deliver these pizzas!", 1.5f),
	};
	
	// this coroutine will get destroyed when you navigate away anyway.
	// aaauagh doesn't read from a script, it just.. does this.
	IEnumerator TextCrawl() {
		log.ClearLog();
		yield return new WaitForSeconds(0.25f);
		yield return StartCoroutine( log.PrintLineAndWait($"A game by {Application.companyName}.") );
		yield return StartCoroutine( log.PrintLineAndWait("Made for Intro to Game Programming 2022.") );
		yield return new WaitForSeconds(5f);
		log.ClearLog();
		
		log.animateNewLines = false;
		log.PrintLine("$ ");
		yield return new WaitForSeconds(1f);
		yield return StartCoroutine( TypeText("cat scenario.txt") );
		yield return new WaitForSeconds(0.25f);
		log.PrintBreak();
		yield return new WaitForSeconds(0.5f);
		
		log.animateNewLines = true;
		foreach ((string lines, float time) in crawlLines) {
			yield return StartCoroutine( log.PrintLineAndWait(lines) );
			yield return new WaitForSeconds(time);
			// log.PrintMore("^C");
			log.PrintBreak();
		}
		log.animateNewLines = false;
		
		log.PrintLine("$ ");
		yield return new WaitForSeconds(3f);
		yield return StartCoroutine( TypeText("rm scenario.txt; clear") );
		yield return new WaitForSeconds(0.5f);
		log.PrintBreak();
		yield return new WaitForSeconds(0.1f);
		log.blinker = false;
		log.ClearLog();
		
		skip.SendMessage("SwitchScene", SendMessageOptions.RequireReceiver);
	}
	
	IEnumerator TypeText(string text) {
		int hesitation = Random.Range(1, 5) / 2;
		foreach (char c in text) {
			if (char.IsWhiteSpace(c)) hesitation = Random.Range(1, 5) / 2;
			float secs = hesitation > 0 ? Random.Range(0.2f, 0.35f) : Random.Range(0.05f, 0.15f);
			log.PrintChar(c);
			yield return new WaitForSeconds(secs);
			if (hesitation > 0) hesitation--;
		}
	}
}
