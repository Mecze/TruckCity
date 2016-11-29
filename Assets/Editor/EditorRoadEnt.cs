using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RoadEnt))]
public class EditorRoadEnt : Editor
{
    // enum displayFieldType { DisplayAsAutomaticFields =0, DisplayAsCustomizableGUIFields=1 }
    //displayFieldType DisplayFieldType;

    RoadEnt t;
    //SerializedObject GetTarget;
    //SerializedProperty ThisList;
    int ListSize;

    private RoadRotationGreen roadRotationGreen;


    void OnEnable()
    {
        //roadRotationGreen = RoadRotationGreen.NE;
        t = (RoadEnt)target;
        //GetTarget = new SerializedObject(t);
        //ThisList = GetTarget.FindProperty("possibleGoldRotations");
    }

    public override void OnInspectorGUI()
    {
     //   serializedObject.Update();        
        DrawDefaultInspector();
        /*
        if (t.isClickable)
        {            
            t.TypeOfRotation = (RoadRotationType)EditorGUILayout.EnumPopup(new GUIContent("Type of Rotation"), t.TypeOfRotation);
            
            if (t.TypeOfRotation == RoadRotationType.Green)
            {
                t.myAnimator = (Animator)EditorGUILayout.ObjectField(new GUIContent("The Animator"), t.myAnimator, typeof(Animator), true);
                t.StartingGreenRotation = (RoadRotationGreen)EditorGUILayout.EnumPopup(new GUIContent("Starting Rotation"), t.StartingGreenRotation);
                //if (!Application.isPlaying) if (t.direction != t.StartingGreenRotation.toRoadDirection()) t.direction = t.StartingGreenRotation.toRoadDirection();

            }
            if (t.TypeOfRotation == RoadRotationType.Blue)
            {
                t.myAnimator = (Animator)EditorGUILayout.ObjectField(new GUIContent("The Animator"), t.myAnimator, typeof(Animator), true);
                t.StartingBlueRotation = (RoadRotationBlue)EditorGUILayout.EnumPopup(new GUIContent("Starting Rotation"), t.StartingBlueRotation);
                //if (!Application.isPlaying) if (t.direction != t.StartingBlueRotation.toRoadDirection()) t.direction = t.StartingBlueRotation.toRoadDirection();
            }
            if (t.TypeOfRotation == RoadRotationType.Purple)
            {
                t.roadAnchorPurple = (RoadPositionPurple)EditorGUILayout.EnumPopup(new GUIContent("Anchored Position"), t.roadAnchorPurple);
                t.roadStartingPurple = (RoadPositionPurple)EditorGUILayout.EnumPopup(new GUIContent("Starting Position"), t.roadStartingPurple);
                EditorGUILayout.LabelField("(Have to be different)");
                int dummyint = 1;
                if (t.roadAnchorPurple == t.roadStartingPurple) t.roadStartingPurple = t.roadStartingPurple.NextPurpleCardinalPoint(t.roadAnchorPurple, ref dummyint);
                RoadDirection rd = RoadDirectionExtensions.ComposeRoadDirection(t.roadAnchorPurple, t.roadStartingPurple);
                //if (!Application.isPlaying) if (t.direction != rd) t.direction = rd;
            }


            if (t.TypeOfRotation == RoadRotationType.Gold)
            {
                //t.StartingPurpleIndex = (int)EditorGUILayout.IntField()
                EditorGUILayout.LabelField("List of Rotations", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("(Element 0 is the starting one)");
                //t.StartingGoldIndex = (int)EditorGUILayout.IntField(new GUIContent("Starting Index"), t.StartingGoldIndex);
                //Update our list

                GetTarget.Update();
                ListSize = ThisList.arraySize;
                GUILayout.BeginHorizontal();
                ListSize = EditorGUILayout.IntField("List Size", ListSize);

                if (ListSize != ThisList.arraySize)
                {
                    while (ListSize > ThisList.arraySize)
                    {
                        ThisList.InsertArrayElementAtIndex(ThisList.arraySize);
                    }
                    while (ListSize < ThisList.arraySize)
                    {
                        ThisList.DeleteArrayElementAtIndex(ThisList.arraySize - 1);
                    }
                }
               
                if (GUILayout.Button("Add New"))
                {
                    t.possibleGoldRotations.Add(new RoadDirection());
                }
                GUILayout.EndHorizontal();
               
                for (int i = 0; i < ThisList.arraySize; i++)
                {
                    SerializedProperty MyListRef = ThisList.GetArrayElementAtIndex(i);
                 
                    GUILayout.BeginHorizontal();
                    
                    EditorGUILayout.PropertyField(MyListRef);
                    if (GUILayout.Button("X", GUILayout.Width(30f)))
                    {
                        ThisList.DeleteArrayElementAtIndex(i);
                        break;
                    }
                    GUILayout.EndHorizontal();                                       
                }

                //f (t.StartingGoldIndex >= ThisList.arraySize) t.StartingGoldIndex = ThisList.arraySize - 1;
                //if (t.StartingGoldIndex < 0) t.StartingGoldIndex = 0;
                if (!Application.isPlaying) if (t.possibleGoldRotations.Count != 0) t.direction = t.possibleGoldRotations[0];
                GetTarget.ApplyModifiedProperties();
            }
        }else
        {
            if (!Application.isPlaying)
            {
                Debug.Log("LOL");
            }// t.direction = (RoadDirection)EditorGUILayout.EnumPopup(new GUIContent("Road Direction"), t.direction);
        }
        EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);



        */
        
        if (GUILayout.Button("Update Road"))
        {
            t.UpdateMaterial("IMG\\LowIMGs\\", true);
        }

    }

}