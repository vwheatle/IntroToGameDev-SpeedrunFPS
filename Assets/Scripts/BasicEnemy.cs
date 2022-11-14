using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {
	EnemyManager em;
	public GameObject bullet;
	
	float timer = 0f;
	public float shootInterval = 0.75f;
	public float maximumDetectRange = 32f;
	
	public int hits = 1;
	
	GameObject player;
	
	AudioSource audioSource;
	ShrinkDeactivate deactivate;
	
	void Awake() {
		timer = float.NegativeInfinity; // shoot asap
		
		em = transform.parent.GetComponent<EnemyManager>();
		player = em.player;
		
		audioSource = GetComponent<AudioSource>();
		deactivate = GetComponent<ShrinkDeactivate>();
	}
	
	void LateUpdate() {
		if (deactivate.active) return;
		
		bool canSeePlayer = (player.transform.position - this.transform.position).magnitude <= maximumDetectRange;
		
		RaycastHit hitInfo;
		canSeePlayer = canSeePlayer && Physics.Linecast(
			this.transform.position,
			player.transform.position,
			out hitInfo
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
				bulletProps.Shoot();
				
				timer = Time.time;
			}
		} else {
			timer = Time.time;
		}
	}
	
	void Hurt(GameObject culprit) {
		hits--;
		if (hits == 0) {
			em.killed++;
			
			// Play a bleep sound, adjusting the pitch based on how many
			// enemies you've already killed.
			audioSource.pitch = 1f + em.percentKilled;
			audioSource.Play();
			
			// Play animation of "shrinking into nothing", then deactivate.
			deactivate.StartShrink();
			
			culprit.SendMessage("KilledEnemy", this.gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
}
