using UnityEngine;
using System.Collections;

public class FloorController : MonoBehaviour {

	public int[] Floors;
	public int CurrentFloor;
	Storage storage;

	bool shouldDisableFloorMode;
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
			if (Input.GetMouseButton(0) && !Utility.IsPointerOverUIObject()) {
				Debug.Log (Utility.CastRayToMouse (layerMask));
				if (Utility.CastRayToMouse (layerMask) != null) {					
					GameObject tile = Utility.CastRayToMouse (layerMask);
					foreach (var tileSprite in storage.TileSprites) {
						if (tileSprite == tile.GetComponent<SpriteRenderer>().sprite) {							
							if (Restaurant.instance.SpendGold(FloorCost)) {
								tile.GetComponent<Tile> ().BuildTile (1);
							} else {
								shouldDisableFloorMode = true;
							}
						}
					}
				}
			}
			if (Input.GetMouseButtonUp(0) && shouldDisableFloorMode) {
				shouldDisableFloorMode = false;
				isInFloorMode = false;
			}
		}
	}
}
