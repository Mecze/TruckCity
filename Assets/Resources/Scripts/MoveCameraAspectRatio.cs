using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MoveCameraAspectRatio : MonoBehaviour {
    [System.Serializable]
    protected class TransformAR
    {
        public bool EditorChosen = false;
        public string AspectRatio;
        public Transform transform;
        public float OrtoSize;
    }

    [SerializeField]
    List<TransformAR> AspectRatioPositions;

    void Awake()
    {
        MoveCam();
    }

    public void MoveCam(bool editortime=false)
    {
        if (AspectRatioPositions.Count == 0) return;
        TransformAR tar;
        if (!editortime) { tar = AspectRatioPositions.Find(x => x.AspectRatio == GameConfig.s.currentAspectRatio.ResolutionName); }
        else { tar = AspectRatioPositions.FindAll(x => x.EditorChosen)[0];  }
        if (tar == null) return;
        if (tar.transform != null) transform.position = tar.transform.position;
        if (tar.OrtoSize != 0f) OrtoSize = tar.OrtoSize;

    }

    public float OrtoSize
    {
        get
        {
            return GetComponent<Camera>().orthographicSize;
        }
        set
        {
            GetComponent<Camera>().orthographicSize = value;
            GetComponentsInChildren<Camera>().ToList<Camera>().ForEach(x => x.orthographicSize = value);            
        }
    }

}
