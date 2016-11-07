using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CountryFlagPicker : MonoBehaviour {
    List<GameObject> Flags;


    void OnEnable()
    {
        //Localization.language = "Spanish";
        Flags = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (checkName(transform.GetChild(i).gameObject.name)) Flags.Add(transform.GetChild(i).gameObject);
        }

        for (int e = 0; e < Flags.Count; e++)
        {
            if (Flags[e].name != Localization.language)
            {
                Flags[e].SetActive(false);
            }else
            {
                Flags[e].SetActive(true);
            }

            
        }

    }

    bool checkName(string Name)
    {
        for (int i = 0; i < Localization.knownLanguages.Length; i++)
        {
            if (Localization.knownLanguages[i] == Name) return true;
        }
        return false;
        
    }
    
}


