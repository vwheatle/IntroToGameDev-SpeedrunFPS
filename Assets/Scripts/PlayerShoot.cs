﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour {
	public GameObject bullet;
	
	AudioSource dieSound, shootSound;
	
	Transform head;
	Transform gunArm, pizzaArm;
	Animator pizzaArmAnim;
	
	float gunArmRecoil = 0f;
	float gunArmRecoilVelocity;
	
	float pizzaArmRecoil = 0f;
	float pizzaArmRecoilVelocity;
	
	public float bulletSpeed = 12f;
	
	int shots, hits;
	public float accuracyRatio {
		get => shots > 0 ? (float)hits / shots : 1f;
	}
	
	void Awake() {
		head = transform.Find("Head");
		gunArm = transform.Find("Body/Left Arm/Forearm");
		pizzaArm = transform.Find("Body/Right Arm/Forearm");
		
		dieSound = GetComponent<AudioSource>();
		shootSound = gunArm.GetComponentInChildren<AudioSource>();
		
		pizzaArmAnim = pizzaArm.GetComponentInParent<Animator>();
	}
	
	void ResetLevel() {
		shots = 0; hits = 0;
		
		gunArmRecoil = 0f;
		gunArmRecoilVelocity = 0f;
		
		pizzaArmRecoil = 10f;
		pizzaArmRecoilVelocity = 0f;
		
		pizzaArmAnim.SetBool("Done", false);
	}
	
	void Update() {
		if (LevelManager.the.state == LevelManager.State.Playing
		&&  Input.GetButtonDown("Fire1")) {
			GameObject goBullet = Instantiate(
				bullet,
				head.position + head.forward * 0.75f,
				head.rotation
			);
			
			BasicBullet bulletProps = goBullet.AddComponent<BasicBullet>();
			bulletProps.origin = this.gameObject;
			bulletProps.Shoot(bulletSpeed);
			shots++;
			
			gunArmRecoil += 20f;
			
			shootSound.pitch = 1f + (Random.value / 16);
			shootSound.Play();
		}
	}
	
	void LateUpdate() {
		bool done = LevelManager.the.state == LevelManager.State.Done;
		if (done) {
			pizzaArmAnim.SetBool("Done", done);
		} else {
			gunArmRecoil = Mathf.SmoothDampAngle(gunArmRecoil, 0f, ref gunArmRecoilVelocity, 0.25f);
			pizzaArmRecoil = Mathf.SmoothDampAngle(pizzaArmRecoil, 0f, ref pizzaArmRecoilVelocity, 0.25f);
			gunArm.localRotation = Quaternion.Euler(0f, gunArmRecoil, -10f);
			pizzaArm.localRotation = Quaternion.Euler(pizzaArmRecoil, 0f, 0f);
		}
		
		if (Input.GetButtonDown("Reset")) {
			LevelManager.the.ResetEverything("Reconsidering");
			// Why not restart the entire scene?
			// Well, because Unity is bad at making inputs persist
			// across scene reloads -- and if I can avoid a load from
			// disk, I will.
		}
	}
	
	void Hurt() {
		dieSound.Play();
		LevelManager.the.ResetEverything("Environmental Factor");
	}
	
	void Killed(GameObject victim) {
		hits++;
	}
	
	void PlacePizza() {
		pizzaArmRecoil += 20f;
	}
}
