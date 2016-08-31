using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class sMenuButton : MonoBehaviour {
    public GameObject tick;
    public Text buttonText;
    public Button button;
    bool parented = false;
    public int levelIndex;

    public void Set(bool locked, bool beated, int levelindex)
    {
        if (!locked)
        {
            buttonText.text = "Level " + (levelindex+1).ToString();
            tick.SetActive(beated);
            levelIndex = levelindex;
        }
        else
        {
            levelIndex = levelindex;
            buttonText.text = "Locked";
            button.interactable = false;
            tick.SetActive(false);
        }
    }

    void Update()
    {
        CheckParented();
    }

    void CheckParented()
    {
        if (parented == false)
        {
            transform.SetParent(GameObject.Find("ButtonAnchor").transform);
            transform.localScale = Vector3.one;
            if (transform.parent) parented = true;
        }

    }

    public void OnClick()
    {
        
        sMenu.singleton.OnLevelButtonClick(levelIndex);
    }
	
}