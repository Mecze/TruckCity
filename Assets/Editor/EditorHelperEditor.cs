using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EditorHelper))]
public class EditorHelperEditor : Editor {

   public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorHelper myScript = (EditorHelper)target;
            if (GUILayout.Button("Set All Objects to High"))
            {
                myScript.SetAllHigh();
            }
            if (GUILayout.Button("Set All Objects to Low"))
            {
                myScript.SetAllLow();
            }
    }
    
}
