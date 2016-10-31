using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	Drag drag;
	public bool IsBuilt;
	public int Cost;
	public int Prestige;

	void Awake() {
		drag = GetComponent<Drag> ();
	}

	public void InitializeFromData(BuildingData data) {
		transform.position = new Vector3 (data.x, data.y, data.z);
		IsBuilt = data.IsBuilt;
	}
}
