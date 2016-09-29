using UnityEngine;
using System.Collections;

public class DockButtonBouncing : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.GetComponent <Animator> ().SetTrigger ("Bounce");}

	public void StopBouncing () {
		gameObject.GetComponent <Animator> ().SetTrigger ("StopBounce");
	}
}
