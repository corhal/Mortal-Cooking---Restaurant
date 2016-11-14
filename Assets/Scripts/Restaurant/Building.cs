using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {
	
	bool isBuilt;
	public bool IsBuilt { get { return isBuilt; } }

	public int InitialCost;
	int cost;
	public int Cost { get { return cost; } }

	public int InitialPrestige;
	int prestige;
	public int Prestige { get { return prestige; } }

	public int typeId;
	public int TypeId { get { return typeId; } }

	public int[] InitialCountLimitByPrestigeLevel;
	int[] countLimitByPrestigeLevel;
	public int[] CountLimitByPrestigeLevel { get { return countLimitByPrestigeLevel; } }

	SpriteRenderer spriteRenderer;
	Storage storage;

	void Awake() {
		storage = Player.instance.gameObject.GetComponent<Storage> ();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
	}

	void Start () {
		cost = InitialCost;
		prestige = InitialPrestige;
		gameObject.GetComponent<BoxCollider2D> ().size = spriteRenderer.sprite.bounds.size;
		typeId = System.Array.IndexOf (storage.FurnitureSprites, spriteRenderer.sprite);
		countLimitByPrestigeLevel = InitialCountLimitByPrestigeLevel;
	}

	public void InitializeFromData (BuildingData data) {		
		gameObject.GetComponent<BoxCollider2D> ().size = gameObject.GetComponent<SpriteRenderer> ().sprite.bounds.size;
		transform.position = new Vector3 (data.x, data.y, data.z);

		isBuilt = data.IsBuilt;
	}

	public void Build() {
		isBuilt = true;
	}
}
