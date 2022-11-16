using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerShoot : MonoBehaviour {
	public GameObject bullet;
	
	public TextLog log;
	
	Transform head;
	
	int shots, hits;
	
	void Start() {
		head = transform.Find("Head");
	}
	
	void ResetEverything(string rationale = null) {
		log.ClearLog();
		log.PrintLine("== BEGIN ORDER [level number go here] ==");
		if (rationale != null)
			log.PrintLine($"System Restarted.\nRestart reason: {rationale}.");
		else
			log.PrintLine("System Initialized.");
		
		GameObject[] rootSiblings = SceneManager.GetActiveScene().GetRootGameObjects();
		foreach (GameObject rootSibling in rootSiblings)
			rootSibling.BroadcastMessage("Reset", SendMessageOptions.DontRequireReceiver);
	}
	
	void Update() {
		if (Input.GetButtonDown("Reset"))
			ResetEverything("Reconsidering");
		
		if (Input.GetButtonDown("Fire1")) {
			GameObject goBullet = Instantiate(
				bullet,
				head.position + head.forward * 0.75f,
				head.rotation
			);
			
			BasicBullet bulletProps = goBullet.AddComponent<BasicBullet>();
			bulletProps.origin = this.gameObject;
			bulletProps.Shoot();
			shots++;
		}
	}
	
	void Hurt() {
		ResetEverything("Environmental Factor");
	}
}
