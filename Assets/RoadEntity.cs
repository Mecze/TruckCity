using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RoadDirection { EW, NE, NEWS, NS, NW, SE, SW  }

public class RoadEntity : MonoBehaviour {
    #region Atributos
    

    /// <summary>
    /// Posición del tipo "Vector3Int"
    /// </summary>
    public Vector3Int position;

    #region Propiedades HAS<Direction>
    //estas propiedades indican si una carretera tiene una salida hacia
    //el lugar indicado
    //Nota: NO TIENE "set{}" se calcula al hacer get.

    bool _HasW;
    public bool HasW
    {
        get
        {
            switch (direction)
            {
                case RoadDirection.EW:
                    _HasW = true;
                    break;
                
                case RoadDirection.NEWS:
                    _HasW = true;
                    break;
                
                case RoadDirection.NW:
                    _HasW = true;
                    break;
               
                case RoadDirection.SW:
                    _HasW = true;
                    break;
                default:
                    _HasW = false;
                    break;
            }
            return _HasW;
        }
    }


    bool _HasE;
    public bool HasE
    {
        get
        {
            switch (direction)
            {
                case RoadDirection.EW:
                    _HasE = true;
                    break;
                case RoadDirection.NE:
                    _HasE = true;
                    break;
                case RoadDirection.NEWS:
                    _HasE = true;
                    break;                
                case RoadDirection.SE:
                    _HasE = true;
                    break;                
                default:
                    _HasE = false;
                    break;
            }
            return _HasE;
        }
    }


    bool _HasS;
    public bool HasS
    {
        get
        {
            switch (direction)
            {
                
                case RoadDirection.NEWS:
                    _HasS = true;
                    break;
                case RoadDirection.NS:
                    _HasS = true;
                    break;                
                case RoadDirection.SE:
                    _HasS = true;
                    break;
                case RoadDirection.SW:
                    _HasS = true;
                    break;
                default:
                    _HasS = false;
                    break;
            }
            return _HasS;
        }
    }



    bool _HasN;
    public bool HasN
    {
        get
        {
            switch (direction)
            {               
                case RoadDirection.NE:
                    _HasN = true;
                    break;
                case RoadDirection.NEWS:
                    _HasN = true;
                    break;
                case RoadDirection.NS:
                    _HasN = true;
                    break;
                case RoadDirection.NW:
                    _HasN = true;
                    break;               
                default:
                    _HasN = false;
                    break;
            }
            return _HasN;
        }
    }


    /// <summary>
    /// Consulta si esta carretera tiene la salida indicada
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool HasDirection(TruckDirection direction)
    {
        bool r = false;
        switch (direction)
        {
            case TruckDirection.N:
                r = HasN;
                break;
            case TruckDirection.E:
                r = HasE;
                break;
            case TruckDirection.W:
                r = HasW;
                break;
            case TruckDirection.S:
                r = HasS;
                break;
            case TruckDirection.None:
                r = true;
                break;
            default:
                r = false;
                break;
        }
        return r;
    }
    #endregion

    #region Propiedades y atributos de rotación

    /// <summary>
    /// La dirección ACTUAL de este RoadID (EW, NE, NW...)
    /// </summary>
    [Header("Rotaciones")]
    [SerializeField]
    RoadDirection _direction;
    public RoadDirection direction
    {
        get
        {
            return _direction;
        }

        set
        {
            _direction = value;
            ChangeMaterial(_direction);
        }
    }


    /// <summary>
    /// Posibles rotaciones de esta carretera
    /// </summary>    
    [SerializeField]
    List<RoadDirection> possibleRotations;

    /// <summary>
    /// Se trata del componente que contiene el material del HIJO de este objeto
    /// Es el que muestra la imagen de la carretera (material)
    /// </summary>
    [SerializeField]
    Renderer roadRenderer;


    #endregion
    #endregion



    #region RecordPosition
    void Awake()
    {
        RecordPosition();
    }
    
    /// <summary>
    /// Al iniciar el juego registra la posición de este bloque en el mapa.
    /// </summary>
    void RecordPosition()
    {
        if (gameObject.name == "CubeRoadSE" || gameObject.name == "CubeRoadNS (2)")
        {
            Debug.Log("asdf");
        }
        position = new Vector3Int();
        position.x = Mathf.RoundToInt(transform.position.x);
        position.y = Mathf.RoundToInt(transform.position.y);
        position.z = Mathf.RoundToInt(transform.position.z);

        MapController.s.mapGO.Add(position, gameObject);
        MapController.s.mapRoad.Add(position, this);

        //Debug.Log("Registrado: " + transform.position.x + ", " + transform.position.z + " en " + pos.x + ", " + pos.z);


    }
    #endregion

    #region OnClick!

    void OnClick()
    {
        if (possibleRotations.Count > 1)
        {
            direction = possibleRotations[0];
            possibleRotations.RemoveAt(0);
            possibleRotations.Add(direction);
        }
    }

    #endregion


    #region Change ROAD material!
    /// <summary>
    /// Cambia el material de la carretera
    /// </summary>
    /// <param name="dir"></param>
    void ChangeMaterial(RoadDirection dir)
    {
        Material mat = (Material)Resources.Load(GameConfig.s.materialsPath + roadDirToString(dir));
        roadRenderer.material = mat;
        //Material[] currentMats = roadRenderer.materials;

    }

    #endregion

    #region utils

    /// <summary>
    /// Devuelve true si hay conexi�n entre ambas carreteras
    /// Lo usan los vehiculos
    /// No tiene encuenta si realmente est�n adyacentes
    /// </summary>
    /// <param name="road1"></param>
    /// <param name="road2"></param>
    /// <returns></returns>
    public static bool CheckConnection(RoadEntity road1, RoadEntity road2)
    {
        bool r = true; //Presuponemos "true"
        //en el "out" de CheckAdjacencyWith se devuelve una dirección
        //que apunta hacia donde está el otro RoadID.
        //Además devuelve True si es adyacente o false si no lo es
        //Si no están Adyacentes hacemos return false;
        TruckDirection from1to2;
        if (!road1.position.CheckAdjacencyWith(road2.position, out from1to2)) return false;
        //Igual en la otra dirección
        TruckDirection from2to1;
        if (!road2.position.CheckAdjacencyWith(road1.position, out from2to1)) return false; ;

        //Si llegamos hasta aquí, estan adyacentes y ambos "fromXtoX" tienen información

        //CheckAdjacency devuelve "true" y el out en "TruckDirection.None" (en ambos)
        //Si se trata, casualmente, de la misma casilla, devolvemos true automaticamente
        if (from1to2 == TruckDirection.None || from2to1 == TruckDirection.None) return true;

        //Si road1 no posee la dirección hasta road2, se falla el check
        if (!road1.HasDirection(from1to2)) r = false;
        //Si road2 no posee la dirección hasta road1, se falla el check
        if (!road2.HasDirection(from2to1)) r = false;


        return r;
    }



    /// <summary>
    /// Devuelve el punto cardinal contrario (Entra sur, sale norte)
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static TruckDirection ReverseDirection(TruckDirection direction)
    {
        TruckDirection result = TruckDirection.None;
        switch (direction)
        {
            case TruckDirection.N:
                result = TruckDirection.S;
                break;
            case TruckDirection.E:
                result = TruckDirection.W;
                break;
            case TruckDirection.W:
                result = TruckDirection.E;
                break;
            case TruckDirection.S:
                result = TruckDirection.N;
                break;
            case TruckDirection.None:
                result = TruckDirection.None;
                break;
            default:
                result = TruckDirection.None;
                break;
        }
        return result;

    }

    public const string EW = "EW";
    public const string NS = "NS";
    public const string SE = "SE";
    public const string SW = "SW";
    public const string NW = "NW";
    public const string NE = "NE";
    public const string NEWS= "NEWS";

    /// <summary>
    /// Devuelve un string que equivale a la dirección de una carretera
    /// </summary>
    /// <param name="road">Lacarretera</param>
    /// <returns>Un string</returns>
    public static string roadDirToString(RoadDirection road)
    {
        string s;
        switch (road)
        {
            case RoadDirection.EW:
                s = EW;
                break;
            case RoadDirection.NE:
                s = NE;
                break;
            case RoadDirection.NEWS:
                s = NEWS;
                break;
            case RoadDirection.NS:
                s = NS;
                break;
            case RoadDirection.NW:
                s = NW;
                break;
            case RoadDirection.SE:
                s = SE;
                break;
            case RoadDirection.SW:
                s = SW;
                break;
            default:
                s = "";
                break;
        }



        return s;

    }


    #endregion




}