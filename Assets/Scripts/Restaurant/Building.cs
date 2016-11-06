using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour, ISelectable {

	public int Id;
	public bool IsBuilt;
	public int Cost;
	public int Prestige;
	public int SpriteIndex;

	void Awake () {
		
	}

	public void InitializeFromData (BuildingData data) {
		Id = data.Id;
		SpriteIndex = data.SpriteIndex;
		Storage storage = Player.instance.gameObject.GetComponent<Storage> ();
		gameObject.GetComponent<SpriteRenderer> ().sprite = storage.FurnitureSprites [SpriteIndex];
		gameObject.GetComponent<BoxCollider2D> ().size = gameObject.GetComponent<SpriteRenderer> ().sprite.bounds.size;
		transform.position = new Vector3 (data.x, data.y, data.z);
		IsBuilt = data.IsBuilt;
	}

	public void Select () {

	}

	public void Deselect () {

	}

	public void ToggleSelect () {

	}
}
