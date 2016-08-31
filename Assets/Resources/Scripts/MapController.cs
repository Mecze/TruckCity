using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MapController : MonoBehaviour {
    #region Singleton
    private static MapController s_singleton = null;

    public static MapController singleton
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<MapController>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("No Existe Singleton MapController");
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }
    public static MapController s
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<MapController>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("No Existe Singleton MapController");
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }
    #endregion


    public Dictionary<Vector3Int, GameObject> mapGO;
    public Dictionary<Vector3Int, RoadEntity> mapRoad;
 

    void Awake()
    {
        mapGO = new Dictionary<Vector3Int, GameObject>();
        mapRoad = new Dictionary<Vector3Int, RoadEntity>();
    }


    public bool CheckNextTile(Vector3Int pos, TruckDirection dir, out RoadEntity roadID)
    {
        roadID = null;
        Vector3Int des = new Vector3Int(pos.x,pos.y,pos.z);
        switch (dir)
        {
            case TruckDirection.N:
                des.z += 1;
                break;
            case TruckDirection.E:
                des.x += 1;
                break;
            case TruckDirection.W:
                des.x -= 1;
                break;
            case TruckDirection.S:
                des.z -= 1;
                break;
            case TruckDirection.None:
                break;
            default:
                break;
        }
        bool b = mapGO.ContainsKey(des);
        if (b) roadID = mapRoad[des] ;
        return b;


    }

	
}