using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlyingText : MonoBehaviour {
	
	public Text myText;
	public float SavedScore;
	public float Speed;
	public Vector3 Direction;
	float alpha;

	void Awake() {		
		myText = GetComponentInChildren<Text> ();
	}

	void Start() {
		alpha = 1.0f;
	}

	void Update() {		
		transform.position = transform.position + Direction * Speed;
		myText.color = new Color (1.0f, 1.0f, 1.0f, alpha);
		alpha -= Speed;
		if (alpha <= 0.0f) {
			Destroy (gameObject);
		}
	}
}
