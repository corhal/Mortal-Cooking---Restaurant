using UnityEngine;
using System.Collections;

public class Selectable : MonoBehaviour {

	public GameObject SelectionObject;

	bool isSelected;
	public bool IsSelected { get { return isSelected; } }

	bool moveable;
	public bool Moveable { get { return moveable; } }

	public bool StartingMoveableTemp;

	public delegate void SelectionChangedEventHandler (Selectable selection, bool selected);
	public static event SelectionChangedEventHandler OnSelectionChanged;

	void Awake() {
		moveable = StartingMoveableTemp;
	}

	void ChangeSelection(bool shouldSelect) {
		isSelected = shouldSelect;
		OnSelectionChanged (this, isSelected);
		if (SelectionObject != null) {
			SelectionObject.SetActive (shouldSelect);
		}		 
	}

	public void Select() {
		ChangeSelection (true);
	}

	public void Deselect() {
		ChangeSelection (false);
	}

	public void ToggleSelect() {
		ChangeSelection (!isSelected);
	}
}
