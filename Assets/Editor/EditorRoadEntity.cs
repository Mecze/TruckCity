using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RoadEntity))]
public class EditorRoadEntity : Editor
{



    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);

        RoadEntity myScript = (RoadEntity)target;
        if (GUILayout.Button("Update Road"))
        {
            myScript.UpdateMaterial("Materials\\");
        }

    }

}