using UnityEngine;
using System.Collections;

public class CameraDrag : MonoBehaviour
{
	public float dragSpeed = 2;
	private Vector3 dragOrigin;

	public bool cameraDragging = true;

	public float outerLeft = -10f;
	public float outerRight = 10f;


	void Update() {		
		//Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

		//float left = Screen.width * 0.2f;
		//float right = Screen.width - (Screen.width * 0.2f);

		//if(mousePosition.x < left) {
			cameraDragging = true;
		//}
		//else if(mousePosition.x > right) {
			//cameraDragging = true;
		//}

		if (Restaurant.instance.GetComponent<Builder>().selectedBuilding != null) {
			cameraDragging = false;
		}

		if (cameraDragging) {
			if (Input.GetMouseButtonDown(0)) {
				dragOrigin = Input.mousePosition;
				return;
			}

			if (!Input.GetMouseButton (0)) {
				if(this.transform.position.x > outerRight) {
					transform.position = new Vector3 (outerRight, transform.position.y, transform.position.z);
				}
				if(this.transform.position.x < outerLeft) {
					transform.position = new Vector3 (outerLeft, transform.position.y, transform.position.z);
				}
				return;
			}

			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
			Vector3 move = new Vector3(-pos.x * dragSpeed, 0, 0);

			if (move.x > 0f) {
				if(this.transform.position.x < outerRight) {
					transform.Translate(move, Space.World);
				}
			}
			else {
				if(this.transform.position.x > outerLeft) {
					transform.Translate(move, Space.World);
				}
			}
		}
	}
}