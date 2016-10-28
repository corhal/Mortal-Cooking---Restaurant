using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider2D))]

public class Drag : MonoBehaviour {

	public bool SnapToGrid;

	Vector3 offset;

	void OnMouseDown() {
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
	}

	void OnMouseDrag() {
		Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
		transform.position = Camera.main.ScreenToWorldPoint(newPosition) + offset;

		if (SnapToGrid) {
			transform.position = new Vector3 (CeilTo05 (transform.position.x), CeilTo05 (transform.position.y), CeilTo05 (transform.position.z));
		}
	}

	float CeilTo05(float number) {
		return Mathf.Floor (number) + 0.5f;
	}
}
