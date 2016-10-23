using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// Este Script se usa para configurar desde Inspector 
/// edificios que ACEPTAN Cargo.
/// Este es el scripts que genera las instancias de 
/// AcceptsCargo y ProduceCargo que manejan internamente
/// la carga.
/// Ademas, este script representa un edificio que maneja
/// carga y genera la parte UI para las dos clases internas
/// mencionadas mas arriba.
//////////////////////////////

public delegate void OnTruckLoadStationDelegate(CardinalPoint cardinal, Cargo Cargo, CargoBuilding building);
public delegate void OnDeliveranceDelegate();
public delegate void OnProducedDelegate();
public delegate void OnLoadCargoDelegate();
#pragma warning disable 0169
public class CargoBuilding : MonoBehaviour {


    [Header("Spawn? (Do Manual References Below!)")]
    [SerializeField]
    bool dontSpawnAnything = false;
    [Header("Config")]
    [SerializeField]
    Color buildingColor;
    [SerializeField]
    bool Forbidable = true;

    [SerializeField]
    public List<ProducesCargo> producesCargo;

    [SerializeField]
    public List<AcceptsCargo> acceptsCargo;




    #region References
    [Header("CargoSprite References")]
    [SerializeField]
    GameObject cargoSpritePrefab;
    [SerializeField]
    List<Transform> midBoxes;

    [Header("Triggers Reference")]
    [SerializeField]
    public List<Transform> TriggersTransform;

    [Header("ArrowsReferences")]
    [SerializeField]
    GameObject arrowPrefab;
    [SerializeField]
    GameObject flippedArrowPrefab;
    [SerializeField]
    List<Transform> centerPanelArrows;
    [SerializeField]
    List<Transform> outterPanelArrows;
    [Header("Highlights")]
    [SerializeField]
    List<GameObject> highlightsGo;
    [SerializeField]
    List<Image> highlights;

    [SerializeField]
    GameObject ForbidSprite;

    

    #endregion

    #region Properties

    public Transform myTransform
    {
        get
        {
            return transform;
        }
    }
    bool _Forbidded = false;
    public bool Forbidded
    {
        get
        {
            return _Forbidded;
        }

        set
        {
            _Forbidded = value;
            ForbidSprite.SetActive(value);
        }
    }

    #endregion

    public event OnTruckLoadStationDelegate OnTruckLoadStation;
    public event OnTruckLoadStationDelegate OnTruckUnloaded;

    void Awake()
    {
        //SetUp phase
        highlights[0].color = buildingColor;
        foreach (ProducesCargo pc in producesCargo)
        {
            SpawnAndConfigCargoSpriteProduces(pc);
        }
        foreach (AcceptsCargo ac in acceptsCargo)
        {
            SpawnAndConfigCargoSpriteRecives(ac);
        }
        /*
        foreach (List<CargoSprite> cslist in acceptsCargo.Select(x => x.myCargoSpriteReference))
        {
            foreach (CargoSprite cs in cslist)
            {
                cs.Adjust();
            }
        }
        */
        foreach (Transform go in midBoxes)
        {
            go.gameObject.GetComponent<AutoGrid>().Adjust();
        }

    }


    /// <summary>
    /// SpawnsArrows
    /// </summary>
    /// <param name="cardinalPoint">Cardinal Point where to Spawn. CANT BE 0!</param>
    /// <param name="inner">On the inner Circle or Outer</param>
    void SpawnArrow(CardinalPoint cardinalPoint, Color color, bool inner = true, bool flipped = false)
    {
        GameObject prefab = arrowPrefab;
        if (flipped) prefab = flippedArrowPrefab;

        List<Transform> list = centerPanelArrows;        
        if (!inner) list = outterPanelArrows;
        GameObject GO = (GameObject)GameObject.Instantiate(prefab, list[(int)cardinalPoint],false);
        //GO.transform.SetParent(list[(int)cardinalPoint]);
        GO.transform.localScale = Vector3.one;
        //GO.transform.rotation = Quaternion.
        GO.GetComponent<ArrowCargo>().SetColor(color);

        if (!flipped)
        {
           // SpawnArrow(cardinalPoint, color, !inner, true);
        }



    }

    /// <summary>
    /// Spawns and Configs a CargoSprite/s depending on ProducesCargo
    /// Only used for ProducesCargo Sprites
    /// </summary>
    /// <param name="pc"></param>
    void SpawnAndConfigCargoSpriteProduces(ProducesCargo pc)
    {
        pc.amountOfItems = pc.startingAmount;

        if (!dontSpawnAnything)
        {
            foreach (CardinalPoint cp in pc.direction)
            {
                GameObject GO = SpawnCargoSprite(cp);
                CargoSprite cs = GO.GetComponent<CargoSprite>();
                SpawnArrow(cp, GameConfig.s.cargoColors[(int)pc.CargoType], false);
                pc.myCargoSpriteReference.Add(cs);
            }
            
        }

        pc.myBuilding = this;
        OnTruckLoadStation += pc.TruckOnPointListener;
        if (pc.needsIncome) OnTruckUnloaded += pc.TruckOnPointListenerINCOME;        
        pc.UpdateMyCargoSprites();
        pc.Initialize();


        
        //cs.Initialize();
    }

    /// <summary>
    /// Spawns and Configs a CargoSprite depending on AcceptsCargo
    /// Only used for AcceptsCargo Sprites
    /// </summary>
    /// <param name="ac"></param>
    void SpawnAndConfigCargoSpriteRecives(AcceptsCargo ac)
    {
        if (!dontSpawnAnything)
        {
            foreach (CardinalPoint cp in ac.direction)
            {
                GameObject GO = SpawnCargoSprite(cp);
                CargoSprite cs = GO.GetComponent<CargoSprite>();
                SpawnArrow(cp, GameConfig.s.cargoColors[(int)ac.CargoType],false,true);
                ac.myCargoSpriteReference.Add(cs);
            }
        }
        ac.myBuilding = this;        
        ac.UpdateMyCargoSprites();
        OnTruckLoadStation += ac.TruckOnPointListener;

        
        
    }



    /// <summary>
    /// Instantiate CargoSprites
    /// </summary>
    /// <param name="cardinalPoint">MidBox to Spawn (it CAN be 0)</param>
    GameObject SpawnCargoSprite(CardinalPoint cardinalPoint)
    {
        
        GameObject GO = (GameObject)GameObject.Instantiate(cargoSpritePrefab, midBoxes[(int)cardinalPoint],false);
        
        GO.transform.localScale = Vector3.one;
        return GO;
    }

    /// <summary>
    /// Fires Event OnTruckStation
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="cargo"></param>
    public void TruckOnStation(CardinalPoint cp, Cargo cargo)
    {
        if (Forbidded) return;
        if (OnTruckLoadStation != null) OnTruckLoadStation(cp, cargo, this);
    }
    /// <summary>
    /// Fires Event OnTruckUnloaded
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="cargo"></param>
    public void TruckGotUnloaded(CardinalPoint cp, Cargo cargo)
    {
        if (OnTruckUnloaded != null) OnTruckUnloaded(cp, cargo, this);
    }

    void OnClick()
    {
        Debug.Log("asdf");
        if (Forbidable) Forbidded = !Forbidded;
    }



}












