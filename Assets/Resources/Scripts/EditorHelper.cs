using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EditorHelper : MonoBehaviour {
    [SerializeField]
    string LowSpritePath;


	void SetAll(GraphicQualitySettings GQS)
    {
        QualitySelector[] QSs = GameObject.FindObjectsOfType<QualitySelector>();

        for (int i = 0; i < QSs.Length; i++)
        {
            QSs[i].Set(GQS, LowSpritePath, true);
        }


    }

    public void SetAllLow()
    {
        SetAll(GraphicQualitySettings.Low);
    }
    public void SetAllMedium()
    {
        SetAll(GraphicQualitySettings.Medium);
    }

    public void SetAllHigh()
    {
        SetAll(GraphicQualitySettings.High);
    }

    public void SetRoads()
    {
        QualitySelector[] QSs = GameObject.FindObjectsOfType<QualitySelector>();

        for (int i = 0; i < QSs.Length; i++)
        {
            QSs[i].SetRoad(LowSpritePath);
        }
    }

    public void DisableAllBuildings()
    {
        QualitySelector[] QSs = GameObject.FindObjectsOfType<QualitySelector>();

        for (int i = 0; i < QSs.Length; i++)
        {
            QSs[i].DisableBuilding();
        }
    }


}
