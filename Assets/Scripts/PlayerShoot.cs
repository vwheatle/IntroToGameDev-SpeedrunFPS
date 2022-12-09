using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerShoot : MonoBehaviour {
	public GameObject bullet;
	
	public TextLog log;
	
	AudioSource dieSound, shootSound;
	
	Transform head;
	Transform gunArm;
	
	float armRecoil = 0f;
	float armRecoilVelocity;
	
	public float bulletSpeed = 12f;
	
	int shots, hits;
	public float accuracyRatio {
		get => (float)hits / shots;
	}
	
	void Awake() {
		dieSound = GetComponent<AudioSource>();
		
		head = transform.Find("Head");
		gunArm = transform.Find("Body").Find("Left Arm").Find("Forearm");
		shootSound = gunArm.GetComponentInChildren<AudioSource>();
	}
	
	void Start() {
		log.ClearLog();
		log.PrintLine("System Initialized.");
	}
	
	void ResetEverything(string rationale = null) {
		log.ClearLog();
		if (rationale != null)
			log.PrintLine($"System Restarted.\n(Reason: {rationale})");
		else
			log.PrintLine("System Initialized.");
		
		GameObject[] rootSiblings = SceneManager.GetActiveScene().GetRootGameObjects();
		foreach (GameObject rootSibling in rootSiblings)
			rootSibling.BroadcastMessage("ResetLevel", SendMessageOptions.DontRequireReceiver);
		
		shots = 0; hits = 0;
	}
	
	void Update() {
		if (Input.GetButtonDown("Fire1") && Time.timeScale > 0.5f) { // HACK
			GameObject goBullet = Instantiate(
				bullet,
				head.position + head.forward * 0.75f,
				head.rotation
			);
			
			BasicBullet bulletProps = goBullet.AddComponent<BasicBullet>();
			bulletProps.origin = this.gameObject;
			bulletProps.Shoot(bulletSpeed);
			shots++;
			
			armRecoil += 20f;
			
			shootSound.pitch = 1f + (Random.value / 16);
			shootSound.Play();
		}
		
		if (armRecoil > 0.001f) {
			armRecoil = Mathf.SmoothDampAngle(armRecoil, 0f, ref armRecoilVelocity, 0.25f);
		} else {
			armRecoil = 0f;
		}
		gunArm.localRotation = Quaternion.Euler(0f, armRecoil, 0f);
	}
	
	void LateUpdate() {
		if (Input.GetButtonDown("Reset")) {
			ResetEverything("Reconsidering");
			// Why not restart the entire scene?
			// Well, because Unity is bad at making inputs persist
			// across scene reloads -- and if I can avoid a load from
			// disk, I will.
		}
	}
	
	void Hurt() {
		ResetEverything("Environmental Factor");
		dieSound.Play();
	}
	
	void Killed(GameObject victim) {
		hits++;
	}
}
