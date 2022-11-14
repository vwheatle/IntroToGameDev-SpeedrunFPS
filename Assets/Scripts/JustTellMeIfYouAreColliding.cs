using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustTellMeIfYouAreColliding : MonoBehaviour {
	public Collider c;
	
	public bool colliding;
	
	private List<MonoBehaviour> subscribers;
	
	void Start() {
		if (c == null || c.gameObject != this.gameObject) {
			Debug.LogError("umm");
		}
		c.isTrigger = false;
	}
	
	public void Subscribe(MonoBehaviour you) {
		subscribers.Add(you);
	}
	
	void OnCollisionEnter(Collision bonk) {
		Collider other = bonk.collider;
		
		if (other.CompareTag("Player")) return;
		
		foreach (MonoBehaviour subscriber in subscribers) {
			subscriber.SendMessage("RemoteCollisionEnter", bonk, SendMessageOptions.DontRequireReceiver);
		}
	}
}
