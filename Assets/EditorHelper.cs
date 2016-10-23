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
            QSs[i].Set(GQS,materialsPath);
        }


    }

    public void SetAllLow()
    {
        SetAll(GraphicQualitySettings.Low);
    }
    public void SetAllHigh()
    {
        SetAll(GraphicQualitySettings.High);
    }


}
