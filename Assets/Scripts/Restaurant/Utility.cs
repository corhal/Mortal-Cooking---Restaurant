using UnityEngine;
using System.Collections;

public static class Utility {

	public static float SnapNumberToFactor(float number, float factor) {
		int multiple =  Mathf.RoundToInt(number/factor);

		return multiple * factor;
	}

	static Vector3 mousePoint;
	static Vector2 mousePoint2D;
	public static GameObject CastRayToMouse() {
		mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePoint2D = new Vector2 (mousePoint.x, mousePoint.y);

		RaycastHit2D hit = Physics2D.Raycast (mousePoint2D, mousePoint2D);

		GameObject hitObject = hit.collider.gameObject;

		return hitObject;
	}
}
