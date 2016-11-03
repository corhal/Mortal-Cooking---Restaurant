using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Builder : MonoBehaviour {

	GameObject buildingPrefab;
	public Building selectedBuilding;

	public delegate void BuiltEventHandler (Building building);
	public event BuiltEventHandler OnBuildingBuilt;

	void Awake() {
		Drag.OnDraggableClicked += Drag_OnDraggableClicked;
		buildingPrefab = Restaurant.instance.BuildingPrefab;
	}

	void Drag_OnDraggableClicked (Drag drag) {
		if (!IsPointerOverUIObject () && !drag.selected && drag.gameObject.GetComponent<Building>() != null) {
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
			DeselectBuilding ();
		} else {
			if (!selectedBuilding.IsBuilt) {
				Destroy (selectedBuilding.gameObject);
				selectedBuilding = null;
			}
		}
	}

	public void DeselectBuilding() {
		if (selectedBuilding != null) {
			if (!selectedBuilding.IsBuilt) {
				BuildBuilding (selectedBuilding);
			}
			selectedBuilding.gameObject.GetComponent<Drag> ().ToggleSelect();
			selectedBuilding.gameObject.GetComponent<Drag> ().OnButtonClicked -= SelectedDrag_OnButtonClicked;
			selectedBuilding = null;
		}
	}

	public void DeployBuilding(int buildingIndex) {
		GameObject newBuildingObject = Instantiate (buildingPrefab, Restaurant.RandomScreenPoint(), buildingPrefab.transform.rotation) as GameObject;
		Building newBuilding = newBuildingObject.GetComponent<Building> ();
		SpriteRenderer buildingSprite = newBuildingObject.GetComponent<SpriteRenderer> ();
		Storage storage = Player.instance.gameObject.GetComponent<Storage> ();
		buildingSprite.sprite = storage.FurnitureSprites [buildingIndex];
		newBuilding.SpriteIndex = buildingIndex;
		newBuildingObject.GetComponent<BoxCollider2D> ().size = buildingSprite.sprite.bounds.size;
		SelectBuilding (newBuilding);
	}

	public void BuildBuilding(Building building) {
		OnBuildingBuilt (building);
	}

	public bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}
