using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {
	public GameObject bullet;
	
	float timer = 0f;
	public float shootInterval = 0.75f;
	
	public int hits = 1;
	
	GameObject player;
	
	void Awake() {
		timer = Time.time;
		
		EnemyManager em = transform.parent.GetComponent<EnemyManager>();
		player = em.player;
	}
	
	void LateUpdate() {
		bool canSeePlayer = (player.transform.position - this.transform.position).magnitude <= 24f;
		
		canSeePlayer = canSeePlayer && Physics.Linecast(
			this.transform.position,
			player.transform.position
		);
		// TODO: physics layer for just map objects
		
		if (canSeePlayer) {
			transform.localRotation = Quaternion.LookRotation(
				player.transform.position - this.transform.position, Vector3.up
			);
			
			if ((Time.time - timer) > shootInterval) {
				GameObject goBullet = Instantiate(
					bullet,
					transform.position + transform.forward * 0.75f,
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
		if (hits <= 0) {
			gameObject.SetActive(false);
			culprit.SendMessage("KilledEnemy", this.gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
}
