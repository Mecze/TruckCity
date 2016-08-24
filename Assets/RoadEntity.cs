using UnityEngine;
using System.Collections;
using System.Collections.Generic;



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

    /// <summary>
    /// Los sprites de los puntos cardinales
    /// </summary>
    [SerializeField]
    GameObject[] sprites;


    #endregion
    #endregion



    #region RecordPosition
    void Awake()
    {
        RecalculateSprites();
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
            RecalculateSprites();
        }
    }

    #endregion


    #region Change ROAD material and Sprites
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

    /// <summary>
    /// Selecciona que Sprites deben estar activados en función de si la carretera se puede mover.
    /// </summary>
    void RecalculateSprites() {
        if (possibleRotations.Count <= 1)
        {
            TurnOffAllSprites();
            return;
        }
        TurnOffAllSprites();
        RoadDirection currentState = direction;
        RoadDirection nextState = possibleRotations[0];
        TruckDirection[] differences;
        TruckDirection[] equals = currentState.Compare(nextState, out differences, true);
        List<TruckDirection> changed = new List<TruckDirection>();
        foreach (TruckDirection dir in differences)
        {
            if (dir != TruckDirection.None)
            {
                TurnOnArrow(dir);
                changed.Add(dir);
            }
        }
        foreach (TruckDirection dir1 in equals)
        {
            if (dir1 != TruckDirection.None)
            {
                TurnOnEquals(dir1);
                changed.Add(dir1);
            }
        }
        if (possibleRotations.Count <= 2) return;
        RoadDirection AfterState = possibleRotations[1];
        equals = nextState.Compare(AfterState, out differences, true);
        foreach (TruckDirection dir in differences)
        {
            if (dir != TruckDirection.None && (changed.Contains(dir) == false))
            {
                TurnOnArrowTrans(dir);
                changed.Add(dir);
            }
        }

    }

    void TurnOffAllSprites()
    {
        foreach (GameObject go in sprites)
        {
            go.GetComponent<SpriteRenderer>().enabled = false;
            go.GetComponent<Animator>().SetBool("Moving", false);
        }
    }
    void TurnOffSprite(TruckDirection dir)
    {
        GameObject go = sprites[(int)dir-1];
        go.GetComponent<SpriteRenderer>().enabled = false;
        go.GetComponent<Animator>().SetBool("Moving", false);
    }
    void TurnOnArrow(TruckDirection dir)
    {
        GameObject go = sprites[(int)dir-1];
        SpriteRenderer ren = go.GetComponent<SpriteRenderer>();
        ren.enabled = true;
        ren.sprite = (Sprite)Resources.Load(GameConfig.s.IMGPath + "Arrow", typeof(Sprite));
        go.GetComponent<Animator>().SetBool("Moving", true);
    }
    void TurnOnArrowTrans(TruckDirection dir)
    {
        GameObject go = sprites[(int)dir - 1];
        SpriteRenderer ren = go.GetComponent<SpriteRenderer>();
        ren.enabled = true;
        ren.sprite = (Sprite)Resources.Load(GameConfig.s.IMGPath + "ArrowTrans", typeof(Sprite));
        go.GetComponent<Animator>().SetBool("Moving", false);
    }
    void TurnOnEquals(TruckDirection dir)
    {
        GameObject go = sprites[(int)dir - 1];
        SpriteRenderer ren = go.GetComponent<SpriteRenderer>();
        ren.enabled = true;
        ren.sprite = (Sprite)Resources.Load(GameConfig.s.IMGPath + "Equals",typeof(Sprite));
        go.GetComponent<Animator>().SetBool("Moving", false);
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



public enum RoadDirection { EW, NE, NEWS, NS, NW, SE, SW }
#region extensions



public static class RoadDirectionExtensions
{
    /// <summary>
    /// Compara dos "RoadDirection" Devuelve las similitudes en array de Truckdirection
    /// mediante el RETURN.
    /// Devuelve las diferencias en array de TruckDirection mediante out.
    /// Ambas arrays varian entre 0 y 2 elementos
    /// </summary>
    /// <param name="thisRoad">selfroad</param>
    /// <param name="otherRoad">La otra carretera a comparar</param>
    /// <param name="diferences">OUT de array de TruckDirection</param>
    /// <param name="focusDifferencesOnOtherRoad">Si es false, el out "differences" devuelve=> ¿Que tiene esta carretera que no tenga "otherRoad"?. Si es true => ¿Que tiene la otra carretera que no tenga esta?</param>
    /// <returns>array de TruckDirection</returns>
    public static TruckDirection[] Compare (this RoadDirection thisRoad, RoadDirection otherRoad, out TruckDirection[] diferences, bool focusDifferencesOnOtherRoad = false)
    {
        bool b = focusDifferencesOnOtherRoad; //alias
        //Arrays temporales a llenar
        TruckDirection[] diff = new TruckDirection[2];
        TruckDirection[] result = new TruckDirection[2];
        //contadores para las arrays
        int i = 0; //result
        int e = 0; //diff
        
        //MainLoop de 1 a 4. el ENUM TruckDirection puede ser casteado a INT (y viceversa)
        //Nota: Enum Truckdirection. None = 0, N = 1, E = 2, W = 3, S = 4.
        for (int index = 1; index <= 4; index++)
        {
            //Sacamos la dirección que vamos a comprobar en este loop:
            TruckDirection dir = (TruckDirection)index; //Cast de int a TruckDirection

            //La comprobamos!

            //Si ambos tienen la direción, guaramos la similitud y se acaba el loop
            if (thisRoad.HasDirection(dir) && otherRoad.HasDirection(dir))
            {
                result[i] = dir;
                i++;
            }
            else //Si no comprobamos si alguno de los dos tiene esa dirección
            {
                //Nota: b es el focus. 
                //Sobre Que carretera queremos devolver las diferencias
                // !b = esta carretera
                // b = la otra carretera
                if (thisRoad.HasDirection(dir) && !b)
                {
                    diff[e] = dir;
                    e++;
                }
                if (otherRoad.HasDirection(dir) && b)
                {
                    diff[e] = dir;
                    e++;
                }
            }
        }
        //End of Main Loop

        //reconstruimos ambas arrays y devolvemos.
        TruckDirection[] resultd = new TruckDirection[result.Length];
        for (int x = 0; x < result.Length; x++)
        {
            resultd[x] = result[x];
        }

        diferences = new TruckDirection[diff.Length];
        for (int y = 0; y < diff.Length; y++)
        {
            diferences[y] = diff[y];
        }
        return resultd;
    }


    /// <summary>
    /// Consulta si esta carretera tiene la salida indicada
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static bool HasDirection(this RoadDirection roadDirection, TruckDirection direction)
    {
        bool r = false;
        switch (direction)
        {
            case TruckDirection.N:
                r = roadDirection.HasN();
            break;
            case TruckDirection.E:
                r = roadDirection.HasE();
                break;
            case TruckDirection.W:
                r = roadDirection.HasW();
                break;
            case TruckDirection.S:
                r = roadDirection.HasS();
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
    
    public static bool HasN(this RoadDirection roadDirection)
    {
        bool r = false;
        switch (roadDirection)
        {        
            case RoadDirection.NE:
                    r = true;
                break;
            case RoadDirection.NEWS:
                    r = true;
                    break;
            case RoadDirection.NS:
                    r = true;
                    break;
            case RoadDirection.NW:
                    r = true;
                    break;
        
            default:
                    r = false;
                    break;
        }
        return r;

    }
    public static bool HasW(this RoadDirection roadDirection)
    {
        bool r = false;
        switch (roadDirection)
        {
            case RoadDirection.EW:
                r = true;
                break;
            
            case RoadDirection.NEWS:
                r = true;
                break;
           
            case RoadDirection.NW:
                r = true;
                break;
            
            case RoadDirection.SW:
                r = true;
                break;

            default:
                r = false;
                break;
        }
        return r;
    }
    public static bool HasE(this RoadDirection roadDirection)
    {
        bool r = false;

        switch (roadDirection)
        {
            case RoadDirection.EW:
                    r = true;
                break;
            case RoadDirection.NE:
                r = true;

                break;
            case RoadDirection.NEWS:
                r = true;
                break;
            
            case RoadDirection.SE:
                r = true;
                break;
            
            default:
                r = false;
                break;
        }
        return r;
    }
    public static bool HasS(this RoadDirection roadDirection)
    {
        bool r = false;

        switch (roadDirection)
        {  
            case RoadDirection.NEWS:
                r = true;
                break;
            case RoadDirection.NS:
                r = true;
                break;           
            case RoadDirection.SE:
                r = true;
                break;
            case RoadDirection.SW:
                r = true;
                break;
            default:
                r = false;
                break;
        }
        return r;
    }



}
#endregion
