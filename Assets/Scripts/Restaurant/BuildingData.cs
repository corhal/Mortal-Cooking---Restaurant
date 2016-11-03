using UnityEngine;
using System.Collections;

[System.Serializable]
public class BuildingData {

	public bool IsBuilt;

	public int Id;
	public float x;
	public float y;
	public float z;

	public int SpriteIndex;

	public void InitializeFromBuilding(Building building) {
		Id = building.Id;
		SpriteIndex = building.SpriteIndex;
		x = building.transform.position.x;
		y = building.transform.position.y;
		z = building.transform.position.z;
		IsBuilt = building.IsBuilt;
	}
}
