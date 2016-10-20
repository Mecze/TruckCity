using UnityEngine;
using System.Collections;

public enum GraphicQualitySettings {None=0, Low = 1, High = 2 }


public class QualitySelector : MonoBehaviour {
    [SerializeField]
    bool isARoad = false;


    [SerializeField]
    GameObject LowGO;
    [SerializeField]
    GameObject HighGO;

    


    public void Set(GraphicQualitySettings QS)
    {
        switch (QS)
        {
            case GraphicQualitySettings.Low:
                LowGO.SetActive(true);
                HighGO.SetActive(false);
                break;
            case GraphicQualitySettings.High:
                LowGO.SetActive(false);
                HighGO.SetActive(true);
                break;
            default:
                break;
        }
        if (isARoad) GetComponent<RoadEntity>().ChangeMaterial(GetComponent<RoadEntity>().direction);


    }


	

}

