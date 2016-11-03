using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class SelectClearer : MonoBehaviour {
	Builder builder;
	Restaurant restaurant;
	bool pressed;

	void Start() {
		builder = Restaurant.instance.gameObject.GetComponent<Builder> ();
	}

	public void DeselectEverything() {
		builder.DeselectBuilding ();
		foreach (var cook in Restaurant.instance.Cooks) {
			cook.Deselect ();
		}
	}

	void OnMouseUpAsButton() {
		if (!builder.IsPointerOverUIObject())
			DeselectEverything ();
	}
}
