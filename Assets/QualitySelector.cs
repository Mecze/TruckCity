using UnityEngine;
using System.Collections;

public enum GraphicQualitySettings {None=0, Low = 2, High = 5 }

[ExecuteInEditMode]
public class QualitySelector : MonoBehaviour {
    [SerializeField]
    bool isARoad = false;
    

    [SerializeField]
    GameObject LowGO;
    [SerializeField]
    GameObject HighGO;


    


    public void Set(GraphicQualitySettings QS, string materialPath)
    {
        if (materialPath == "") materialPath = GameConfig.s.materialsPath;
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
        if (isARoad) GetComponent<RoadEntity>().ChangeMaterial(GetComponent<RoadEntity>().direction, QS, materialPath);


    }


	

}

