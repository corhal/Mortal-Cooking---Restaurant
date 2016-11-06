using UnityEngine;
using System.Collections;

public class Selectable : MonoBehaviour {

	public GameObject Selection;

	bool isSelected;
	public bool IsSelected { get { return isSelected; } }

	bool moveable;
	public bool Moveable { get { return moveable; } }

	public bool StartingMoveableTemp;

	void Awake() {
		moveable = StartingMoveableTemp;
	}

	void ChangeSelection(bool shouldSelect) {
		isSelected = shouldSelect;
		if (Selection != null) {
			Selection.SetActive (shouldSelect);
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
