using UnityEngine;
using System.Collections;

public class ThemeButtonBegging : MonoBehaviour {
	public float period = 1.0f;
	public float delay = 1.0f;
	private float nextBeg;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > nextBeg) {
			if (gameObject.GetComponent <UIButton> ().isEnabled) {
				gameObject.GetComponent <Animator> ().SetTrigger ("Beg");
			}

			nextBeg = Time.time + period;
		}
	}

	void OnEnable () {
		nextBeg = Time.time + delay;
	}
}
