using UnityEngine;
using System.Collections;

public class Builder : MonoBehaviour {

	Building selectedBuilding;

	public delegate void BuiltEventHandler (Building building);
	public event BuiltEventHandler OnBuildingBuilt;

	void Awake() {
		Drag.OnDraggableClicked += Drag_OnDraggableClicked;
	}

	void Drag_OnDraggableClicked (Drag drag) {
		if (!drag.selected && drag.gameObject.GetComponent<Building>() != null) {
			SelectBuilding (drag.gameObject.GetComponent<Building> ());
		}
	}

	public void SelectBuilding(Building building) {
		DeselectBuilding ();
		building.gameObject.GetComponent<Drag> ().ToggleSelect();
		building.gameObject.GetComponent<Drag> ().OnButtonClicked += SelectedDrag_OnButtonClicked;
		selectedBuilding = building;
	}

	void SelectedDrag_OnButtonClicked (bool confirm) {
		if (confirm) {
			if (selectedBuilding.IsBuilt) {
				Debug.Log ("Deselecting built building");
				DeselectBuilding ();
			} else {
				BuildBuilding (selectedBuilding);
				DeselectBuilding ();
			}
		} else {
			if (!selectedBuilding.IsBuilt) {
				Destroy (selectedBuilding.gameObject);
				selectedBuilding = null;
			}
		}
	}

	public void DeselectBuilding() {
		if (selectedBuilding != null) {
			selectedBuilding.gameObject.GetComponent<Drag> ().ToggleSelect();
			selectedBuilding.gameObject.GetComponent<Drag> ().OnButtonClicked -= SelectedDrag_OnButtonClicked;
			selectedBuilding = null;
		}
	}

	public void DeployBuilding(GameObject buildingPrefab) {
		GameObject newBuildingObject = Instantiate (buildingPrefab, Restaurant.RandomScreenPoint(), buildingPrefab.transform.rotation) as GameObject;
		Building newBuilding = newBuildingObject.GetComponent<Building> ();
		SelectBuilding (newBuilding);
	}

	public void BuildBuilding(Building building) {
		OnBuildingBuilt (building);
	}
}
