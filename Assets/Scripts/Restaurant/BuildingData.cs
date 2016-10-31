﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class BuildingData {

	public bool IsBuilt;

	public float x;
	public float y;
	public float z;

	public void InitializeFromBuilding(Building building) {
		x = building.transform.position.x;
		y = building.transform.position.y;
		z = building.transform.position.z;
		IsBuilt = building.IsBuilt;
	}
}
