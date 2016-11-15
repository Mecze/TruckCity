using UnityEngine;
using System.Collections;

public class DebugResolution : MonoBehaviour {

	void Awake()
    {
        UILabel myLabel = GetComponent<UILabel>();
        if (myLabel == null) return;
        myLabel.text = Screen.currentResolution.width.ToString() + "x" + Screen.currentResolution.height.ToString();
    }
}
