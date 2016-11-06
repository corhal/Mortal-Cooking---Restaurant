using UnityEngine;
using System.Collections;

[System.Serializable]
public class BuildingData {

	public bool IsBuilt;

	public int TypeId;
	public float x;
	public float y;
	public float z;

	public void InitializeFromBuilding(Building building) {	
		TypeId = building.TypeId;
		x = building.transform.position.x;
		y = building.transform.position.y;
		z = building.transform.position.z;
		IsBuilt = building.IsBuilt;
	}
}
