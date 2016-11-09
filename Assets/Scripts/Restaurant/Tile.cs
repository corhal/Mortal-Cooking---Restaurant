using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public SpriteRenderer TileSprite;
	public int TileType;
	public int TileSpriteIndex;

	void Awake() {
		TileSprite = gameObject.GetComponent<SpriteRenderer> ();	
	}

	void Start () {			
		for (int i = 0; i < Player.instance.GetComponent<Storage>().TileSprites.Length; i++) {
			if (TileSprite.sprite == Player.instance.GetComponent<Storage>().TileSprites[i]) {
				TileSpriteIndex = i;
			}
		}
	}

	public void BuildTile(int tileType) {
		ChangeTile (tileType);
		Restaurant.instance.AddPrestige (1); // потом поменять на что-то нормальное
	}

	public void ChangeTile (int tileType) {
		TileType = tileType;
		if (tileType == 1) {
			TileSprite.sprite = Player.instance.GetComponent<Storage>().FancyTileSprites [TileSpriteIndex];
		}
	}
}
