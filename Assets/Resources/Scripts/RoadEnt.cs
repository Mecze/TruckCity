using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public delegate void NoTrucksOnTopEventHandler();
public delegate void TruckLeftTheRoadEventHandler(int TrucksOnTop);

public class RoadEnt : MonoBehaviour {
    #region Attributes
    [Header("Visuals")]
    [SerializeField]
    bool PermanentVisuals= false;
    [SerializeField]
    PicaVoxel.Volume myRoad;
    [SerializeField]
    SpriteRenderer myRoadLow;
    [SerializeField]
    GameObject TruckOverlay;


    [Header("Direction")]
    public RoadDirection myDirection;

    [HideInInspector]
    public List<TruckEnt> OnTopTrucks;
    /// <summary>
    /// Posici�n del tipo "Vector3Int"
    /// </summary>
    [HideInInspector]
    public Vector3Int position;
    //[HideInInspector]
    //public List<RoadEnt> Neightbours;
    RoadColl[] myColliders;

    #endregion

    #region Properties
    private NoTrucksOnTopEventHandler _onNoTrucksOnTop;
    public event NoTrucksOnTopEventHandler OnNoTrucksOnTop {
        add
        {
            if (_onNoTrucksOnTop == null || !_onNoTrucksOnTop.GetInvocationList().Contains(value))
            {
                _onNoTrucksOnTop += value;
            }            
        }
        remove
        {
            _onNoTrucksOnTop -= value;
        }
    }

    private TruckLeftTheRoadEventHandler _onTruckLeftTheRoad;
    public event TruckLeftTheRoadEventHandler OnTruckLeftTheRoad
    {
        add
        {
            if (_onTruckLeftTheRoad == null || !_onTruckLeftTheRoad.GetInvocationList().Contains(value))
            {
                _onTruckLeftTheRoad += value;
            }
        }
        remove
        {
            _onTruckLeftTheRoad -= value;
        }
    }

    #endregion

    #region Initialization

    void Awake()
    {
        RecordPosition();
        myColliders = GetComponentsInChildren<RoadColl>();
    }

    /// <summary>
    /// Records its own position on the MapController in order to be
    /// accessed later.
    /// </summary>
    void RecordPosition()
    {
        position = new Vector3Int();
        position.x = Mathf.RoundToInt(transform.position.x);
        position.y = Mathf.RoundToInt(transform.position.y);
        position.z = Mathf.RoundToInt(transform.position.z);
        MapController.s.mapGO.Add(position, gameObject);
        MapController.s.mapRoad.Add(position, this);
    }
    #endregion
    
    #region Visuals and Materials
    /// <summary>
    /// Cambia el material de la carretera
    /// </summary>
    /// <param name="dir"></param>
    public void ChangeVisuals(RoadDirection dir, GraphicQualitySettings GQS = GraphicQualitySettings.None, string LowIMGPath = "")
    {

       // if (isClickable && TypeOfRotation == RoadRotationType.Green) dir = RoadDirection.NE;
        //if (isClickable && TypeOfRotation == RoadRotationType.Blue) dir = RoadDirection.EW;
        if (LowIMGPath == "")
        {
            if (GameConfig.s == null) LowIMGPath = "IMG\\LowIMGs\\";
            if (GameConfig.s != null) LowIMGPath = GameConfig.s.LowIMGPath;
        }
        if (GQS == GraphicQualitySettings.None)
        {
            if (sProfileManager.ProfileSingleton == null)
            {//we are probably at editor time, we change all roads
                ChangeVisuals(dir, GraphicQualitySettings.High, LowIMGPath);
                ChangeVisuals(dir, GraphicQualitySettings.Low, LowIMGPath);
                //Debug.Log("ChangingVisuals: EditorTime");
                return;
            }
            else
            {
                //Debug.Log("ChangingVisuals: PlayTime");
                GQS = sProfileManager.ProfileSingleton.GlobalGraphicQualitySettings;
            }
        }
        else
        {
            //Debug.Log("ChangingVisuals: PlayTime 2 "+ dir.ToString());
        }

        if (PermanentVisuals) return;
        switch (GQS)
        {
            case GraphicQualitySettings.Low:
                //Low Quality using old Materials                
                Sprite sprite = (Sprite)Resources.Load<Sprite>(LowIMGPath + "sprite_" + dir.ToString(0));
                if (myRoadLow != null) myRoadLow.sprite = sprite;


                break;
            case GraphicQualitySettings.Medium:
                Sprite sprite1 = (Sprite)Resources.Load<Sprite>(LowIMGPath + "sprite_" + dir.ToString(0));
                if (myRoadLow != null) myRoadLow.sprite = sprite1;
                break;
            case GraphicQualitySettings.High:
                //High Quality using PicaVoxel
                if (myRoad != null) myRoad.SetFrame((int)dir);

                break;
            default:
                break;
        }
    }

    public void UpdateMaterial(string LowIMGPath, bool fromEditor = false)
    {
        if (PermanentVisuals) return;
        RoadDirection dir = myDirection;
        bool h = myRoad.gameObject.activeSelf;
        bool l = myRoadLow.gameObject.activeSelf;
        if (fromEditor)
        {
            myRoad.gameObject.SetActive(true);
            myRoadLow.gameObject.SetActive(true);
        }

        if (myRoad != null)
        {
            if (myRoad.gameObject.activeSelf == true || fromEditor)
            {
                myRoad.SetFrame((int)dir);
            }
        }
        if (myRoadLow != null)
        {
            if (myRoadLow.gameObject.activeSelf == true || fromEditor)
            {
                string s = (string)LowIMGPath + "sprite_" + myDirection.ToString(0);
                Sprite sp = Resources.Load<Sprite>(s);
                myRoadLow.sprite = sp;
            }
        }

        if (fromEditor)
        {
            myRoadLow.gameObject.SetActive(l);
            myRoad.gameObject.SetActive(h);
        }



    }

    void OnTopTruckSprite()
    {
        if (OnTopTrucks.Count > 0)
        {
            TruckOverlay.SetActive(true);
        }else
        {
            TruckOverlay.SetActive(false);
        }

    }
    #endregion
        
    #region OnTopTruck Management

    public void AddOnTopTruck(TruckEnt Thetruck)
    {
        //REGISTER COLLIDER EVENTS
        foreach (RoadColl TC in myColliders)
        {
            TC.RegisterYourCheckOnThisTruck(Thetruck);
        }

        //Manage ONTOPTRUCKS ARRAY (and its events)
        OnTopTrucks.Add(Thetruck);
        OnTopTruckSprite();
    }
    public void RemoveOnTopTruck(TruckEnt Thetruck)
    {
        //UNREGISTER COLLIDER EVENTS
        foreach (RoadColl TC in myColliders)
        {
            TC.UnRegisterYourCheckOnThisTruck(Thetruck);
        }


        //Manager ONTOPTRUCKS ARRAY (and its events)
        OnTopTrucks.Remove(Thetruck);
        if (_onTruckLeftTheRoad != null) _onTruckLeftTheRoad(OnTopTrucks.Count);
        if (OnTopTrucks.Count == 0)
        {
            if (_onNoTrucksOnTop != null) _onNoTrucksOnTop();
        }
        OnTopTruckSprite();


    }


    #endregion
/*
    #region Collider Events Management

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Truck") {
            TruckEnt te = other.GetComponent<TruckEnt>();
            foreach (RoadColl TC in myColliders)
            {
                TC.RegisterYourCheckOnThisTruck(te);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Truck")
        {
            TruckEnt te = other.GetComponent<TruckEnt>();
            foreach (RoadColl TC in myColliders)
            {
                TC.UnRegisterYourCheckOnThisTruck(te);
            }
        }
    }


    #endregion
    */
}
