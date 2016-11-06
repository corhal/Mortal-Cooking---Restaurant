using UnityEngine;
using System.Collections;

public class ZChecker : MonoBehaviour {

	void Start() {
		CheckZ ();
	}

	public void CheckZ() {
		Building building = gameObject.GetComponent<Building> ();
		int layer = Restaurant.instance.BuildingLayers [building.TypeId];
		transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.y - layer);
	}
}
