using UnityEngine;
using System.Collections;

public class SelectionController : MonoBehaviour {

	Selectable currentSelection;

	public delegate void SelectionChangedEventHandler (Selectable selection);
	public static event SelectionChangedEventHandler OnSelectionChanged;

	void Update() {		
		if (Input.GetMouseButtonUp(0)) {
			Selectable selection = Utility.CastRayToMouse ().GetComponent<Selectable> ();

			if (selection != null) {				
				if (selection.IsSelected) {
					//ClearSelection ();
				} else {
					Select (selection);
				}
			} else {
				ClearSelection ();
			}
		}
	}

	void ClearSelection() {
		if (currentSelection != null) {
			currentSelection.Deselect ();
			currentSelection = null;
			OnSelectionChanged (null);
		}
	}

	void Select(Selectable selection) {
		ClearSelection ();
		selection.Select ();
		currentSelection = selection;
		OnSelectionChanged (selection);
	}
}
