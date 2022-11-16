using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
		other.gameObject.SendMessage("Hurt", this.gameObject, SendMessageOptions.DontRequireReceiver);
	}
}
