using UnityEngine;
using System.Collections;

public class StoreUpgradeButton : MonoBehaviour {
	private UISlider progressBar;

	// Use this for initialization
	void Start () {
		progressBar = gameObject.GetComponentInChildren <UISlider> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Upgrade () {
		progressBar.value += 0.2f;
		if (progressBar.value == 1.0f) {
			gameObject.GetComponent <UIButton> ().isEnabled = false;
		}
	}
}
