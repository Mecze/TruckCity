using UnityEngine;
using System.Collections;

public class Clickable : MonoBehaviour {

	void OnEnable()
    {
        MouseController.OnClick += GetClicked;
    }
    void OnDisable()
    {
        MouseController.OnClick -= GetClicked;
    }



    //Listener!
    void GetClicked(GameObject g)
    {
        if (g != gameObject) return;
        BroadcastMessage("OnClick");
        Debug.Log(gameObject.name + " Got Clicked!");


    }

}