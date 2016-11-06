using UnityEngine;
using System.Collections;

public class SelectionController : MonoBehaviour {

	Selectable currentSelection;

	public delegate void SelectionChangedEventHandler (Selectable selection, bool selected);
	public static event SelectionChangedEventHandler OnSelectionChanged;

	bool canSelect = true;

	void Awake() {
		Selectable.OnSelectionChanged += Selectable_OnSelectionChanged;
	}

	void Selectable_OnSelectionChanged (Selectable selection, bool selected) { // Нужно ли передавать ивент через посредника?
		OnSelectionChanged (selection, selected);
	}

	void Update() {		
		if (Input.GetMouseButtonDown(0) && Utility.IsPointerOverUIObject()) {
			canSelect = false;
		}
		if (Input.GetMouseButtonUp(0) && !Utility.IsPointerOverUIObject()) {
			if (canSelect) {				
				Selectable selection = null;
				Debug.Log (Utility.CastRayToMouse ());
				if (Utility.CastRayToMouse () != null) {
					selection = Utility.CastRayToMouse ().GetComponent<Selectable> ();
				}

				if (selection != null) {				
					if (!selection.IsSelected) {						
						Select (selection);
					} 
				} else {
					ClearSelection ();
					Debug.Log ("Clearing existing selection");
				}
			}
		}
		if (Input.GetMouseButtonUp (0)) {
			canSelect = true;
		}
	}

	void ClearSelection() {
		if (currentSelection != null) {			
			currentSelection.Deselect ();
			currentSelection = null;
		}
	}

	public void Select(Selectable selection) { // Публичность этого метода выглядит сомнительно
		ClearSelection ();
		selection.Select ();
		currentSelection = selection;
	}

	void OnDestroy() {
		Selectable.OnSelectionChanged -= Selectable_OnSelectionChanged;
	}
}
