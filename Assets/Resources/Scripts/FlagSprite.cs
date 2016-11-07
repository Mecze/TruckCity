using UnityEngine;
using System.Collections;

public class FlagSprite : MonoBehaviour {

    [SerializeField]
    UISprite myCheckSprite;
    [HideInInspector]
    public string myLanguage;

    bool _set;
    public bool Set
    {
        set
        {
            _set = value;
            SetSprite();
        }
        get
        {
            return _set;
        }
    }

    public string myImage
    {
        set
        {
            GetComponent<UISprite>().spriteName = value;
        }
        get
        {
            return GetComponent<UISprite>().spriteName;
        }
    }



    void SetSprite()
    {
        if (_set)
        {
            myCheckSprite.alpha = 1f;
        }else
        {
            myCheckSprite.alpha = 0f;
        }
    }


    public void Click()
    {
        if (myLanguage == "") return;
        if (Set) return;

        Localization.language = myLanguage;
        //Debug.Log(Localization.Get("Settings"));
        FlagSprite[] array = FindObjectsOfType<FlagSprite>();
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].myLanguage == myLanguage)
            {
                array[i].Set = true;
            }else
            {
                array[i].Set = false;
            }
        }
    }
    



}
