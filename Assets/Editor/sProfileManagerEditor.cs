using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(sProfileManager))]
public class sProfileManagerEditor : Editor
{

    sProfileManager t;

    void OnEnable()
    {
        t = (sProfileManager)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        if (GUILayout.Button("Reorder Default Profile levels"))
        {
            t.defaultProfile.profileLevels = t.defaultProfile.profileLevels.OrderBy(x => x.index).ToList<ProfileLevels>();

        }
        if (GUILayout.Button("Reorder Level Conditions"))
        {
            t.levelconditions = t.levelconditions.OrderBy(x => x.level).ToList<LevelConditions>();
        }


    }
}
