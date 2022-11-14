using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
	public GameObject player;
	public TextLog log;
	
	
	[Header("Pickup Goal")]
	private int _killed = 0;
	public int goalKills = -1;
	public int killed {
		get => _killed;
		set {
			_killed = value;
			if (achievedGoalPickups) {
				if (_killed == goalKills)
					log.PrintLine($"No obstacles remain.");
				
				// asdf
			} else {
				// asdf
			}
		}
	}
	public float percentKilled {
		get => (float)killed / goalKills;
	}
	public bool achievedGoalPickups {
		get => killed >= goalKills;
	}
	
	void Start() {
		// Set the target number of pickups to collect
		// to the number of children this gameobject has,
		// if the automatic sentinel value is used.
		// Gosh I wish enums with payloads were in C#...
		if (goalKills < 0)
			goalKills = transform.childCount;
		
		Reset();
	}
	
	void Reset() {
		log.ClearLog();
		log.PrintLine($"== BEGIN ORDER [0] ==\n{goalKills} Obstacles");
		// TODO,
	}
}
