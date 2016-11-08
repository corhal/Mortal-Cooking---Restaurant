using UnityEngine;
using System.Collections;

public class FloorController : MonoBehaviour {

	public int[] Floors;
	public int CurrentFloor;
	Storage storage;

	public static bool isInFloorMode;
	public int layerMask;
	public int FloorCost;

	void Start() {
		layerMask = LayerMask.GetMask ("Floor");
		storage = Player.instance.gameObject.GetComponent<Storage> ();
	}

	public void ToggleMode() {
		isInFloorMode = !isInFloorMode;
	}

	void Update() {
		if (isInFloorMode) {
			if (Input.GetMouseButton(0)) {
				Debug.Log (Utility.CastRayToMouse (layerMask));
				if (Utility.CastRayToMouse (layerMask) != null) {					
					GameObject tile = Utility.CastRayToMouse (layerMask);
					foreach (var tileSprite in storage.TileSprites) {
						if (tileSprite == tile.GetComponent<SpriteRenderer>().sprite) {
							//int index = System.Array.IndexOf (storage.TileSprites, tileSprite);
							if (Restaurant.instance.SpendGold(FloorCost)) {
								tile.GetComponent<Tile> ().ChangeTile (1);
								//tile.GetComponent<SpriteRenderer> ().sprite = storage.FancyTileSprites [index];
							} else {
								isInFloorMode = false;
							}
						}
					}
				}
			}
		}
	}
}
