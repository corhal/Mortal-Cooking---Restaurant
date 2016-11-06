﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class Utility {

	public static float SnapNumberToFactor(float number, float factor) {
		int multiple =  Mathf.RoundToInt(number/factor);

		return multiple * factor;
	}

	public static GameObject CastRayToMouse() {
		Vector3 mousePoint;
		Vector2 mousePoint2D;

		mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePoint2D = new Vector2 (mousePoint.x, mousePoint.y);

		RaycastHit2D hit = Physics2D.Raycast (mousePoint2D, mousePoint2D);

		GameObject hitObject = null;

		if (hit.collider != null) {
			hitObject = hit.collider.gameObject;
		}

		return hitObject;
	}

	public static bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		for (int i = 0; i < results.Count; i++) {
			if (results[i].gameObject.GetComponent<Text> () != null) {
				results.RemoveAt (i); // Довольно сомнительное решение, которое я сейчас обосновываю тем, что текстовые лейблы, как правило, некликабельны
			}
		}
		return results.Count > 0;
	}
}
