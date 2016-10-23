using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EditorHelper))]
public class EditorHelperEditor : Editor {

    

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);

        EditorHelper myScript = (EditorHelper)target;
        if (GUILayout.Button("Update all Road directions"))
        {
            myScript.SetRoads();
        }

        EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);

        if (GUILayout.Button("Set All Objects to High"))
        {
            myScript.SetAllHigh();
        }
        if (GUILayout.Button("Set All Objects to Medium"))
        {
            myScript.SetAllMedium();
        }
        if (GUILayout.Button("Set All Objects to Low"))
        {
            myScript.SetAllLow();
        }
    }
    
}
