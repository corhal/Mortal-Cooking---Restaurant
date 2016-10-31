using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider2D))]

public class Drag : MonoBehaviour {

	public GameObject ButtonContainer;

	public delegate void DraggableClickedEventHandler (Drag drag);
	public static event DraggableClickedEventHandler OnDraggableClicked;

	public delegate void ButtonClickedEventHandler (bool confirm);
	public event ButtonClickedEventHandler OnButtonClicked;

	public bool selected;
	public bool SnapToGrid;

	Vector3 offset;

	void OnMouseDown() {
		if (selected) {
			offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
		}
	}

	void OnMouseUpAsButton() {
		if (!selected) {
			OnDraggableClicked (this);
		}
	}

	void OnMouseDrag() {
		if (selected) {
			Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
			transform.position = Camera.main.ScreenToWorldPoint(newPosition) + offset;

			if (SnapToGrid) {
				transform.position = new Vector3 (CeilTo05 (transform.position.x), CeilTo05 (transform.position.y), CeilTo05 (transform.position.z));
			}
		}
	}

	float CeilTo05(float number) {
		return Mathf.Floor (number) + 0.5f;
	}

	public void ToggleSelect() {
		selected = !selected;
		ButtonContainer.SetActive (!ButtonContainer.activeSelf);
	}

	public void ClickButton(bool confirm) {
		OnButtonClicked (confirm);
	}
}
