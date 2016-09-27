using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeSpawn : MonoBehaviour {
    [Header("Lists")]
    [SerializeField]
    List<GameObject> TreeGOPrefab;

    [SerializeField]
    List<Transform> Position;

    [Header("Number of Trees")]
    [SerializeField]
    int numberOfTrees;

    [SerializeField]
    Vector3 Scale;

    List<int> nonRepeatIndexList;


    void Awake()
    {
        nonRepeatIndexList = new List<int>();
        for (int i = 1; i <= numberOfTrees; i++)
        {
            CreateTree();
        }




    }



    void CreateTree()
    {
        
        if (TreeGOPrefab.Count == 0 || Position.Count == 0) return;
        int chosenTree = TreeGOPrefab.RandomIndex();
        int chosenPos = Position.RandomIndex(nonRepeatIndexList);
        nonRepeatIndexList.Add(chosenPos);

        GameObject go = GameObject.Instantiate(TreeGOPrefab[chosenTree]);
        go.transform.SetParent(Position[chosenPos]);
        go.transform.localScale = Scale;
        go.transform.localPosition = Vector3.zero;
        //go.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        



    }
   
   



	
}




