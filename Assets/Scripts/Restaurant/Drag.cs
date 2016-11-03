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
			float factor = 0.375f;
			if (SnapToGrid) {
				transform.position = new Vector3 (Snap (transform.position.x, factor), Snap (transform.position.y, factor), Snap (transform.position.z, factor));
			}
			ZChecker checker = GetComponent<ZChecker> ();
			checker.CheckZ ();
		}
	}

	float Snap(float number, float factor) {
		int multiple =  Mathf.RoundToInt(number/factor);

		return multiple*factor;
	}

	public void ToggleSelect() {
		selected = !selected;
		ButtonContainer.SetActive (!ButtonContainer.activeSelf);
	}

	public void ClickButton(bool confirm) {
		OnButtonClicked (confirm);
	}
}
