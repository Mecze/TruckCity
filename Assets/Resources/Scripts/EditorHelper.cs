using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EditorHelper : MonoBehaviour {
    [SerializeField]
    string materialsPath;


	void SetAll(GraphicQualitySettings GQS)
    {
        QualitySelector[] QSs = GameObject.FindObjectsOfType<QualitySelector>();

        for (int i = 0; i < QSs.Length; i++)
        {
            QSs[i].Set(GQS,materialsPath, true);
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
            QSs[i].SetRoad(materialsPath);
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
