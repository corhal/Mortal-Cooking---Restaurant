using UnityEngine;
using System.Collections;

public class DragController : MonoBehaviour {

	GameObject draggable;
	public bool SnapToGrid;
	Vector3 offset;
	bool shouldDrag;
	public static bool ShouldDrag; // hack

	void Awake() {
		SelectionController.OnSelectionChanged += SelectionController_OnSelectionChanged;
	}

	void SelectionController_OnSelectionChanged (Selectable selection) {
		if (selection == null) {
			draggable = null;
			shouldDrag = false;
		} else if (selection.Moveable) {
			draggable = selection.gameObject;
			offset = draggable.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
			shouldDrag = true;
		}
		ShouldDrag = shouldDrag;
	}
		
	void Update() {		
		if (draggable != null && Input.GetMouseButtonDown(0)) {
			if (Utility.CastRayToMouse() != null && Utility.CastRayToMouse().GetComponent<Selectable>() == draggable.GetComponent<Selectable>()) {
				shouldDrag = true;
			} else {
				shouldDrag = false;
			}
		}
		if (shouldDrag && Input.GetMouseButton(0)) {
			Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
			draggable.transform.position = Camera.main.ScreenToWorldPoint(newPosition) + offset;
			float factor = 0.375f;
			if (SnapToGrid) {
				draggable.transform.position = new Vector3 (Utility.SnapNumberToFactor (draggable.transform.position.x, factor), Utility.SnapNumberToFactor (draggable.transform.position.y, factor), Utility.SnapNumberToFactor (draggable.transform.position.z, factor));
			}
			ZChecker checker = draggable.GetComponent<ZChecker> ();
			checker.CheckZ ();
		}
	}
}
