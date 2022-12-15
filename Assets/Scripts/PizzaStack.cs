using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaStack : MonoBehaviour {
	public GameObject pizzaBox;
	
	private List<GameObject> indexedPizzaBoxes;
	
	void Awake() {
		indexedPizzaBoxes = new List<GameObject>();
	}
	
	void StackUpTo(int pizzas = 1) {
		foreach (Transform pizza in transform)
			Destroy(pizza.gameObject);
		
		indexedPizzaBoxes.Clear();
		for (int i = 0; i < pizzas; i++) {
			Vector3 pos = Vector3.up * (0.01f + i * 0.09f);
			Quaternion rot = Quaternion.Euler(-90f, Random.Range(120f, 240f), 0f);
			GameObject pizzaInstance = Instantiate(pizzaBox, pos, rot, transform);
			pizzaInstance.transform.localPosition = pos;
			pizzaInstance.transform.localRotation = rot;
			indexedPizzaBoxes.Add(pizzaInstance);
		}
	}
	
	void UpdateCount(int pizzasLeft) {
		pizzasLeft = Mathf.Clamp(pizzasLeft, 0, indexedPizzaBoxes.Count);
		
		int i;
		for (i = 0; i < pizzasLeft; i++)
			indexedPizzaBoxes[i].SetActive(true);
		for (; i < indexedPizzaBoxes.Count; i++)
			indexedPizzaBoxes[i].SetActive(false);
	}
}
