using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour {
	public GameObject bullet;
	
	public TextLog log;
	
	Transform head;
	
	float startTime;
	
	void Start() {
		head = transform.Find("Head");
		startTime = Time.time;
	}
	
	void Update() {
		if (Input.GetButtonDown("Fire1")) {
			GameObject goBullet = Instantiate(
				bullet,
				head.position + head.forward * 0.75f,
				head.rotation
			);
			
			BasicBullet bulletProps = goBullet.AddComponent<BasicBullet>();
			bulletProps.origin = this.gameObject;
			bulletProps.Shoot();
		}
	}
	
	void Hurt() {
		log.PrintLine("Hurt (this should kill you)!");
	}
	
	void KilledEnemy(GameObject victim) {
		log.PrintLine($"[{Time.time - startTime,6:0.00}s] Dismissed '{victim.name}'.");
	}
}
