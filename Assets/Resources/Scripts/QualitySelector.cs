using UnityEngine;
using System.Collections;
using System;

public enum GraphicQualitySettings {None=0, Low = 2, Medium = 3, High = 5 }

[ExecuteInEditMode]
public class QualitySelector : MonoBehaviour {
    [SerializeField]
    bool isARoad = false;
    

    [SerializeField]
    GameObject[] LowGO;
    [SerializeField]
    GameObject[] MediumGO;
    [SerializeField]
    GameObject[] HighGO;

    public static int instances = 0;
    public static int finishedinstances = 0;

    static void InstaceFinished()
    {
        finishedinstances++;
        if (finishedinstances >= instances) GC.Collect();
    }
    
    public void SetRoad(string LowSpritePath)
    {
        if (!isARoad) return;
        if (GetComponent<RoadEnt>() == null) return;
        if (LowSpritePath == "") LowSpritePath = GameConfig.s.LowIMGPath;
        GetComponent<RoadEnt>().UpdateMaterial(LowSpritePath);
    }

    public void Set(GraphicQualitySettings QS, string LowSpritePath, bool CalledFromEditor = false)
    {
        if (LowSpritePath == "")
        {
            if (GameConfig.s == null) return;
            LowSpritePath = GameConfig.s.LowIMGPath;
        }
        switch (QS)
        {
            case GraphicQualitySettings.Low:
                ActiveQuality(LowGO);
                DisableQuality(MediumGO, LowGO, CalledFromEditor);
                DisableQuality(HighGO, LowGO, CalledFromEditor);
                break;
            case GraphicQualitySettings.Medium:
                ActiveQuality(MediumGO);
                DisableQuality(LowGO, MediumGO, CalledFromEditor);                
                DisableQuality(HighGO, MediumGO, CalledFromEditor);
                break;
            case GraphicQualitySettings.High:
                ActiveQuality(HighGO);
                DisableQuality(LowGO, HighGO, CalledFromEditor);
                DisableQuality(MediumGO, HighGO, CalledFromEditor);
                break;
            default:
                break;
        }
        if (isARoad) GetComponent<RoadEnt>().ChangeVisuals(GetComponent<RoadEnt>().myDirection, QS, LowSpritePath);
        QualitySelector.InstaceFinished();


    }
    /// <summary>
    /// EdidorOnly: Disables all building objects
    /// </summary>
    public void DisableBuilding()
    {
        if (isARoad) return;
        for (int i = 0; i < LowGO.Length; i++)
        {
            if (LowGO[i].name != "RoadMat")LowGO[i].SetActive(false);
        }
        for (int i = 0; i < MediumGO.Length; i++)
        {
            if (MediumGO[i].name != "RoadMat") MediumGO[i].SetActive(false);
        }
        for (int i = 0; i < HighGO.Length; i++)
        {
            if (HighGO[i].name != "RoadMat") HighGO[i].SetActive(false);
        }



    }

    /// <summary>
    /// Activa Objetos de esta calidad
    /// </summary>
    /// <param name="gos"></param>
    void ActiveQuality(GameObject[] gos)
    {
        for (int i = 0; i < gos.Length; i++)
        {
            gos[i].SetActive(true);
        }
    }

    /// <summary>
    /// Desactiva Objectos de esta calidad
    /// Ademas, intenta destruirlos tambien, salvo que esten en la lista de
    /// Objetos usados (ActiveGOs), o se est� llamando desde el editor
    /// </summary>
    /// <param name="gos"></param>
    /// <param name="ActiveGOs"></param>
    void DisableQuality(GameObject[] gos, GameObject[] ActiveGOs, bool CalledFromEditor = false)
    {
        for (int i = 0; i < gos.Length; i++)
        {
            if (!SelectiveObject(gos[i], ActiveGOs))
            {
                if (gos[i] != null) gos[i].SetActive(false);
                if (!CalledFromEditor)GameObject.DestroyImmediate(gos[i]);
            }
            
            //Resources.UnloadAsset(gos[i]);
        }

    }

    /// <summary>
    /// Intenta destruir este Objeto. Pero antes se asegura que no
    /// est� en la lista de Objetos usados
    /// </summary>
    /// <param name="go"></param>
    /// <param name="ActiveGOs"></param>
    bool SelectiveObject(GameObject go, GameObject[] ActiveGOs)
    {
        bool found = false;
        for (int i = 0; i < ActiveGOs.Length; i++)
        {
            if (go == ActiveGOs[i]) found = true;
        }
        return found;
    }


}

