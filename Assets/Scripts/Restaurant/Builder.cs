using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Builder : MonoBehaviour {

	public GameObject[] BuildingPrefabs;
	public static GameObject[] SBuildingPrefabs;

	public delegate void BuiltEventHandler (Building building);
	public event BuiltEventHandler OnBuildingBuilt;

	SelectionController selectionController;

	void Awake() {				
		selectionController = gameObject.GetComponent<SelectionController> ();
		SelectionController.OnSelectionChanged += SelectionController_OnSelectionChanged;
		SBuildingPrefabs = BuildingPrefabs;
	}

	void SelectionController_OnSelectionChanged (Selectable selection, bool selected) {
		if (!selected) {			
			Building building = selection.gameObject.GetComponent<Building> ();
			if (building != null && !building.IsBuilt) {				
				BuildBuilding (building);
			}
		}
	}

	public void DeployBuilding(int buildingIndex) {
		GameObject newBuildingObject = Instantiate (SBuildingPrefabs[buildingIndex], Camera.main.transform.position, SBuildingPrefabs[buildingIndex].transform.rotation) as GameObject;
		selectionController.Select(newBuildingObject.GetComponent<Selectable>()); // выглядит грязновато
	}

	public void BuildBuilding(Building building) {
		OnBuildingBuilt (building);
	}

	void OnDestroy() {
		SelectionController.OnSelectionChanged -= SelectionController_OnSelectionChanged;
	}
}
