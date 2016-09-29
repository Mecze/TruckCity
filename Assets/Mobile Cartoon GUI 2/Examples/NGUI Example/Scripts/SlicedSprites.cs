using UnityEngine;
using System.Collections;

public class SlicedSprites : MonoBehaviour {
	private Vector3 windowPosition;
	private int dimensionsX;
	private int dimensionsY;

	// Use this for initialization
	void Start () {
		windowPosition = transform.localPosition;
		dimensionsX = gameObject.GetComponent <UISprite> ().width;
		dimensionsY = gameObject.GetComponent <UISprite> ().height;
	}

	public void ResetWindow () {
		transform.localPosition = windowPosition;
		gameObject.GetComponent <UISprite> ().width = dimensionsX;
		gameObject.GetComponent <UISprite> ().height = dimensionsY;
	} 
}
