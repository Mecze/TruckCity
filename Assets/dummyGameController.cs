using UnityEngine;
using System.Collections;

public class dummyGameController : MonoBehaviour {
    [SerializeField]
    GraphicQualitySettings Quality;
    [SerializeField]
    string LowImgPath;

	
    void Awake()
    {
        SetQuality();
        
    }


    void SetQuality()
    {
        if (Quality == GraphicQualitySettings.None) return;
        if (LowImgPath == "") LowImgPath = "IMG\\LowIMGs\\";

        QualitySelector[] Qss = GameObject.FindObjectsOfType<QualitySelector>();
        QualitySelector.instances = Qss.Length;
        QualitySelector.finishedinstances = 0;
        if (Qss.Length < 1) return; //Failsafe

        for (int i = 0; i < Qss.Length; i++)
        {
            Qss[i].Set(Quality, LowImgPath);
        }



    }



}
