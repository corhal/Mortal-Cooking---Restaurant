using UnityEngine;
using System.Collections;

public class TooltipShower : MonoBehaviour {

	public GameObject Tooltip;

	void OnMouseDown() {
		Tooltip.SetActive (true);
	}

	void OnMouseUp() {
		Tooltip.SetActive (false);
	}
}
