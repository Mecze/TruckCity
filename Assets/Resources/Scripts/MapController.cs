using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public delegate void TileClickedEventHandler(Vector3Int position, bool CheckSelf);


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

    #region Events
    private TileClickedEventHandler _onPurpleTileClicked;
    public event TileClickedEventHandler OnPurpleTileClicked
    {
        add
        {
            if (_onPurpleTileClicked == null || !_onPurpleTileClicked.GetInvocationList().Contains(value))
            {
                _onPurpleTileClicked += value;
            }
        }
        remove
        {
            _onPurpleTileClicked -= value;
        }
    }


    private TileClickedEventHandler _onGreenTileClicked;
    public event TileClickedEventHandler OnGreenTileCLicked
    {
        add
        {
            if (_onGreenTileClicked == null || !_onGreenTileClicked.GetInvocationList().Contains(value))
            {
                _onGreenTileClicked += value;
            }
        }
        remove
        {
            _onGreenTileClicked -= value;
        }
    }
    /// <summary>
    /// Fire a Green Tile Event Clicked on the position "pos"
    /// </summary>
    /// <param name="pos"></param>
    public void GreenTileClicked(Vector3Int pos)
    {
        if (_onGreenTileClicked != null) _onGreenTileClicked(pos, true);
    }


    /// <summary>
    /// Fire a Purple Tile Event Clicked on the position "pos"
    /// </summary>
    /// <param name="pos"></param>
    public void PurpleTileClicked(Vector3Int pos)
    {
        if (_onPurpleTileClicked != null) _onPurpleTileClicked(pos, false);
    }


    #endregion



    public Dictionary<Vector3Int, GameObject> mapGO;
    public Dictionary<Vector3Int, RoadEnt> mapRoad;
 

    void Awake()
    {
        mapGO = new Dictionary<Vector3Int, GameObject>();
        mapRoad = new Dictionary<Vector3Int, RoadEnt>();
    }


    public bool CheckNextTile(Vector3Int pos, CardinalPoint dir, out RoadEnt roadID)
    {
        roadID = null;
        Vector3Int des = new Vector3Int(pos.x,pos.y,pos.z);
        switch (dir)
        {
            case CardinalPoint.N:
                des.z += 1;
                break;
            case CardinalPoint.E:
                des.x += 1;
                break;
            case CardinalPoint.W:
                des.x -= 1;
                break;
            case CardinalPoint.S:
                des.z -= 1;
                break;
            case CardinalPoint.None:
                break;
            default:
                break;
        }
        bool b = mapGO.ContainsKey(des);
        if (b) roadID = mapRoad[des] ;
        return b;


    }

	
}