using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {
	EnemyManager em;
	public GameObject bullet;
	public float bulletSpeed = 12f;
	
	float timer = 0f;
	public float shootInterval = 0.75f;
	public float maximumDetectRange = 32f;
	
	public int totalHits = 1;
	private int hits;
	
	GameObject player;
	
	AudioSource audioSource;
	ShrinkDeactivate deactivate;
	
	void Awake() {
		em = GetComponentInParent<EnemyManager>();
		player = em.player;
		
		audioSource = GetComponent<AudioSource>();
		deactivate = GetComponent<ShrinkDeactivate>();
		
		ResetLevel();
	}
	
	public void ResetLevel() {
		hits = totalHits;
		
		transform.localRotation = Quaternion.identity;
		timer = float.NegativeInfinity; // shoot asap
	}
	
	void LateUpdate() {
		if (deactivate && deactivate.active) return;
		
		bool canSeePlayer = (player.transform.position - this.transform.position).magnitude <= maximumDetectRange;
		
		RaycastHit hitInfo;
		canSeePlayer = canSeePlayer && Physics.Linecast(
			this.transform.position,
			player.transform.position,
			out hitInfo,
			-1,
			QueryTriggerInteraction.Ignore
		) && hitInfo.collider.gameObject == player;
		// TODO: physics layer for just map objects
		
		if (canSeePlayer) {
			transform.localRotation = Quaternion.LookRotation(
				player.transform.position - this.transform.position, Vector3.up
			);
			
			if ((Time.time - timer) > shootInterval) {
				// I promise everything will be a ball, and not an ellipsoid.
				float avgScale = transform.localScale.z;
				
				GameObject goBullet = Instantiate(
					bullet,
					transform.position + transform.forward * avgScale * 0.75f,
					transform.rotation
				);
				
				BasicBullet bulletProps = goBullet.AddComponent<BasicBullet>();
				bulletProps.origin = this.gameObject;
				bulletProps.Shoot(bulletSpeed);
				
				timer = Time.time;
			}
		}
	}
	
	void Hurt(GameObject culprit) {
		hits--;
		if (hits == 0) {
			// Play a bleep sound, adjusting the pitch based on how many
			// enemies you've already killed.
			float pitchVariation = Random.value / Mathf.Min(4, em.goalKills) / 2;
			audioSource.pitch = 1f + em.percentKilled + pitchVariation;
			audioSource.Play();
			
			// Play animation of "shrinking into nothing", then deactivate.
			if (deactivate) deactivate.StartShrink();
			else gameObject.SetActive(false);
			
			em.OnKill(this.gameObject, culprit);
			culprit.SendMessage("Killed", this.gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
}
