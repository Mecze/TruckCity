using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(MoveCameraAspectRatio))]
public class MoveCameraAspectRatioEditor : Editor {

    MoveCameraAspectRatio t;

    void OnEnable()
    {        
        t = (MoveCameraAspectRatio)target;        
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Move Camera Now"))
        {
            t.MoveCam(true);
            //FindObjectsOfType<MoveUIToWorldPos>().ToList<MoveUIToWorldPos>().ForEach(x => x.Reposition());
        }
    }
}
