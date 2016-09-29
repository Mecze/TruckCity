using UnityEngine;
using System.Collections;

public class ButtonState : MonoBehaviour {
	public Animator buttonAnimator;
	public TweenColor tweenColor;

	public bool bounce;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (tweenColor != null) {
			if (!gameObject.GetComponent <UIButton> ().isEnabled) {
				tweenColor.PlayForward ();
			}
			else {
				tweenColor.PlayReverse ();
			}
		}
	}

	public void OnButtonPressed () {
		buttonAnimator.SetTrigger ("Pressed");
	}

	public void OnButtonRelease () {
		buttonAnimator.SetTrigger ("Release");
	}

	public void OnButtonClick () {
		buttonAnimator.SetTrigger ("Click");
	}
}
