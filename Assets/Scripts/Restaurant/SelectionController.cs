using UnityEngine;
using System.Collections;

public class SelectionController : MonoBehaviour {

	Selectable currentSelection;

	public delegate void SelectionChangedEventHandler (Selectable selection, bool selected);
	public static event SelectionChangedEventHandler OnSelectionChanged;

	bool canSelect = true;

	public int layerMask;

	void Awake() {
		Selectable.OnSelectionChanged += Selectable_OnSelectionChanged;
		//layerMask = LayerMask.NameToLayer ("Buildings");
		layerMask = LayerMask.GetMask ("Buildings");
	}

	void Selectable_OnSelectionChanged (Selectable selection, bool selected) { // Нужно ли передавать ивент через посредника?
		OnSelectionChanged (selection, selected);
	}

	void Update() {		
		if (!FloorController.isInFloorMode) { // Dirty hax time
			if (Input.GetMouseButtonDown(0) && Utility.IsPointerOverUIObject()) {
				canSelect = false;
			}
			if (Input.GetMouseButtonUp(0) && !Utility.IsPointerOverUIObject()) {
				if (canSelect) {				
					Selectable selection = null;
					if (Utility.CastRayToMouse (layerMask) != null) {
						selection = Utility.CastRayToMouse (layerMask).GetComponent<Selectable> ();
					}

					if (selection != null) {				
						if (!selection.IsSelected) {						
							Select (selection);
						} 
					} else {
						ClearSelection ();
					}
				}
			}
			if (Input.GetMouseButtonUp (0)) {
				canSelect = true;
			}
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
