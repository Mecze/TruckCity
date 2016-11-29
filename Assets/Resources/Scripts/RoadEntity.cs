using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PicaVoxel;
using System;
using System.Linq;

public enum RoadRotationType { Green=0, Blue=1, Purple=2, Gold=3 }



//public enum RoadPositionPurple { N=0,E=1,W=2,S=3}


public class RoadEntity : MonoBehaviour, IFreezable {
    #region Atributos

    bool freeze = false;

    //GraphicQualitySettings GlobalQS;

    [SerializeField]
    bool PermanentVisuals = false;

    [SerializeField]
    Volume myRoad;
    [SerializeField]
    SpriteRenderer myRoadLow;

    /// <summary>
    /// Posición del tipo "Vector3Int"
    /// </summary>
    [HideInInspector]
    public Vector3Int position;
    #endregion

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
    public bool HasDirection(CardinalPoint direction)
    {
        bool r = false;
        switch (direction)
        {
            case CardinalPoint.N:
                r = HasN;
                break;
            case CardinalPoint.E:
                r = HasE;
                break;
            case CardinalPoint.W:
                r = HasW;
                break;
            case CardinalPoint.S:
                r = HasS;
                break;
            case CardinalPoint.None:
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
            if (_direction == value) return;
            _direction = value;
            if (direction == RoadDirection.EW)
            {
         
               //Debug.Log("ASdf");
            }
            ChangeVisuals(_direction);

        }
    }

    


    

    /// <summary>
    /// Los sprites de los puntos cardinales
    /// </summary>
    [SerializeField]
    GameObject[] sprites;
    [SerializeField]
    GameObject ClickableSprite;

    /// <summary>
    /// 
    /// </summary>
    [HideInInspector]
    int numberOfTruckOnTop = 0;
    public int NumberOfTruckOnTop
    {
        get
        {
            return numberOfTruckOnTop;
        }

        set
        {
            bool b = (value == numberOfTruckOnTop);
            numberOfTruckOnTop = value;
            if (b) return;
            if (possibleGoldRotations.Count <= 1) return;    
            if (value > 0) ChangeSpritesColor(enumColor.Red);
            if (value <= 0) ChangeSpritesColor(enumColor.Black);
            

        }
    }
    int numberOfComingTrucks = 0;

    [Header("Clickable Animation?")]
    [SerializeField]
    public bool isClickable = false;
    //[HideInInspector]
    public Animator myAnimator;
    //[HideInInspector]
    public RoadRotationType TypeOfRotation;
    [Header("Green")]
    //[HideInInspector]
    public RoadRotationGreen StartingGreenRotation = RoadRotationGreen.NE;
    [Header("Blue")]
    //[HideInInspector]
    public RoadRotationBlue StartingBlueRotation = RoadRotationBlue.EW;

    /// <summary>
    /// Posibles rotaciones de esta carretera
    /// </summary>   

    [Header("Purple")]
    //[HideInInspector]
    //public int StartingGoldIndex = 0;
    [SerializeField]
    //[HideInInspector]
    public RoadPositionPurple roadAnchorPurple = RoadPositionPurple.N;
    [SerializeField]
//    [HideInInspector]
    public RoadPositionPurple roadStartingPurple = RoadPositionPurple.E;
    //List<CardinalPoint> roadArrayPurple;
    [Header("Gold")]
    [SerializeField]
    //[HideInInspector]
    public List<RoadDirection> possibleGoldRotations;
    int purpleDirection = 1;

#endregion
    
    #region Freeze

    public void Freeze()
    {
        freeze = true;
    }
    public void Unfreeze()
    {
        freeze = false;
    }


    #endregion

    #region AWAKE()
    void Awake()
    {
    #region animator reference
        if (isClickable && myAnimator == null)
        {
            if (GetComponentInChildren<Animator>() == null)
            {
                Debug.LogError("Reference not made on the Road: " + this.name + " ||| Missing myAnimator and no animator found on childrens");
            }
            else
            {
                myAnimator = GetComponentInChildren<Animator>();
            }
        }
    #endregion
        //RecalculateSprites();
        RecordPosition();
        InitializeAnimator();
        ClickableSpriteInitialization();
    }
#endregion

    #region Begin Sequence - InitializeAnimator()

    /// <summary>
    /// Initializes the starting position of the animator if the road is Clickable
    /// and other things
    /// </summary>
    void InitializeAnimator()
    {
        TurnOffAllSprites();
        if (!isClickable || myAnimator == null) return;        
        switch (TypeOfRotation)
        {
            case RoadRotationType.Green:
                _direction = StartingGreenRotation.toRoadDirection();
                ChangeVisuals(RoadDirection.NE);
                if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("90_"+greenDirtoString(StartingGreenRotation)+"_state")) { break; }                
                myAnimator.SetBool("NoAnim", true);
                myAnimator.SetBool("GoTo"+greenDirtoString(StartingGreenRotation),true);
                  
                
                break;
            case RoadRotationType.Blue:                
                _direction = StartingBlueRotation.toRoadDirection();
                StartingGreenRotation = StartingBlueRotation.BlueToGreen();
                ChangeVisuals(RoadDirection.EW);
                if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("90_" + greenDirtoString(StartingBlueRotation.BlueToGreen()) + "_State")) { break; }
                myAnimator.SetBool("NoAnim", true);
                myAnimator.SetBool("GoTo" + greenDirtoString(StartingBlueRotation.BlueToGreen()), true);
                break;
            case RoadRotationType.Purple:
                //This is "not" animator, but it's an initializatin needed for Purple type of clickable roads
                if (roadAnchorPurple == roadStartingPurple) roadStartingPurple = roadStartingPurple.NextPurpleCardinalPoint(roadAnchorPurple, ref purpleDirection);
                direction = RoadRotationPurpleExtensions.ComposeRoadDirection(roadAnchorPurple, roadStartingPurple);
                RecalculateSprites(RoadRotationPurpleExtensions.ComposeRoadDirection(roadAnchorPurple, roadStartingPurple.NextPurpleCardinalPoint(roadAnchorPurple, ref purpleDirection)));
                break;
            case RoadRotationType.Gold:
                
                direction = possibleGoldRotations[0];               
                possibleGoldRotations.RemoveAt(0);
                possibleGoldRotations.Add(direction);
                RecalculateSprites();
                break;
            default:
                break;
        }



    }

    #endregion

    #region Begin Sequence - RecordPosition()
    /// <summary>
    /// Al iniciar el juego registra la posición de este bloque en el mapa.
    /// </summary>
    void RecordPosition()
    {
        if (gameObject.name == "CubeRoadSE" || gameObject.name == "CubeRoadNS (2)")
        {
         //   Debug.Log("asdf");
        }
        position = new Vector3Int();
        position.x = Mathf.RoundToInt(transform.position.x);
        position.y = Mathf.RoundToInt(transform.position.y);
        position.z = Mathf.RoundToInt(transform.position.z);

        MapController.s.mapGO.Add(position, gameObject);
        //MapController.s.mapRoad.Add(position, this);

        //Debug.Log("Registrado: " + transform.position.x + ", " + transform.position.z + " en " + pos.x + ", " + pos.z);


    }
    #endregion

    #region Begin Sequence - ClickableSpriteInitialization()
    void ClickableSpriteInitialization()
    {
        



        if (isClickable) ClickableSprite.SetActive(true);
        SpriteRenderer sr = ClickableSprite.GetComponent<SpriteRenderer>();

        //Colors
        List<Color> colors = new List<Color>();        

        //Path
        string path = "";


        if (GameConfig.s == null)
        {
            path = "IMG\\";
            colors.Add(Color.green);
            colors.Add(Color.blue);
            colors.Add(Color.magenta);
            colors.Add(Color.yellow);
        }else
        {
            colors = GameConfig.s.clickableRoadColors.ToList<Color>();
            path = GameConfig.s.IMGPath;
        }
        if (path == "") return;
        switch (TypeOfRotation)
        {
            case RoadRotationType.Green:
                sr.color = colors[0];
                sr.sprite = (Sprite)Resources.Load<Sprite>(path + "GreenOverlay");
                break;
            case RoadRotationType.Blue:
                sr.color = colors[1];
                sr.sprite = (Sprite)Resources.Load<Sprite>(path + "BlueOverlay");
                

                break;
            case RoadRotationType.Purple:
                sr.color = colors[2];
                sr.sprite = (Sprite)Resources.Load<Sprite>(path + "PurpleOverlay");
                
                
                switch (roadAnchorPurple)
                {
                    case RoadPositionPurple.None:
                        break;
                    case RoadPositionPurple.N:
                        ClickableSprite.transform.rotation = new Quaternion(-3.090862e-08f, 0.7071068f, -0.7071068f, -3.090862e-08f);
                        break;
                    case RoadPositionPurple.E:
                        ClickableSprite.transform.rotation = new Quaternion(0.5f, -0.5f, 0.5f, 0.5f);
                        break;
                    case RoadPositionPurple.S:
                        break;
                    case RoadPositionPurple.W:
                        ClickableSprite.transform.rotation = new Quaternion(0.5f, 0.5f, -0.5f, 0.5f);
                        break;
                    default:
                        break;
                }
                break;
            case RoadRotationType.Gold:
                sr.color = colors[3];
                sr.sprite = (Sprite)Resources.Load<Sprite>(path + "GoldOverlay");
                break;
            default:
                break;
        }

    }
    #endregion

    #region OnClick!

    void OnPress(bool isPressed)
    {
        if (NumberOfTruckOnTop > 0) return;
        if (numberOfComingTrucks > 0) return;
        if (freeze) return;
        if (!isPressed) return;
        if (!isClickable) return;
        
        switch (TypeOfRotation)
        {
            case RoadRotationType.Green:
                myAnimator.SetBool("Next", true);
                StartingGreenRotation = StartingGreenRotation.Next();
                _direction = StartingGreenRotation.toRoadDirection();
                break;
            case RoadRotationType.Blue:
                myAnimator.SetBool("Next",true);
                StartingGreenRotation = StartingGreenRotation.Next();
                StartingBlueRotation = StartingGreenRotation.GreenToBlue();
                _direction = StartingBlueRotation.toRoadDirection();
                break;
            case RoadRotationType.Purple:
                roadStartingPurple = roadStartingPurple.NextPurpleCardinalPoint(roadAnchorPurple,ref purpleDirection);
                direction = RoadRotationPurpleExtensions.ComposeRoadDirection(roadAnchorPurple, roadStartingPurple);
                TurnOffAllSprites();
                if (direction == RoadDirection.EW || direction == RoadDirection.NS)
                {
                    RecalculateSprites(RoadRotationPurpleExtensions.ComposeRoadDirection(roadAnchorPurple, roadStartingPurple.NextPurpleCardinalPoint(roadAnchorPurple, ref purpleDirection)));
                }
                break;
            case RoadRotationType.Gold:
                direction = possibleGoldRotations[0];
                possibleGoldRotations.RemoveAt(0);
                possibleGoldRotations.Add(direction);
                RecalculateSprites();
                break;
            default:
                break;
        }
    }
    #endregion

    #region Change ROAD material and Sprites
    /// <summary>
    /// Cambia el color de todos los sprites de esta carretera.
    /// </summary>
    /// <param name="color">Color publico de "GameConfig"</param>
    void ChangeSpritesColor(enumColor color)
    {
        foreach (GameObject go in sprites)
        {
            //go.GetComponent<SpriteRenderer>().enabled = false;
            go.GetComponent<SpriteRenderer>().color = GameConfig.s.publicColors[(int)color];
        }
    }

    /// <summary>
    /// Cambia el material de la carretera
    /// </summary>
    /// <param name="dir"></param>
    public void ChangeVisuals(RoadDirection dir, GraphicQualitySettings GQS = GraphicQualitySettings.None, string LowIMGPath= "")
    {
        
        if (isClickable && TypeOfRotation == RoadRotationType.Green) dir = RoadDirection.NE;
        if (isClickable && TypeOfRotation == RoadRotationType.Blue) dir = RoadDirection.EW;
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
            }else
            {
                //Debug.Log("ChangingVisuals: PlayTime");
                GQS = sProfileManager.ProfileSingleton.GlobalGraphicQualitySettings;
            }
        }else
        {
            //Debug.Log("ChangingVisuals: PlayTime 2 "+ dir.ToString());
        }
        
        if (PermanentVisuals) return;
        switch (GQS)
        {
            case GraphicQualitySettings.Low:
                //Low Quality using old Materials                
                Sprite sprite = (Sprite)Resources.Load<Sprite>(LowIMGPath + "sprite_"+ roadDirToString(dir));
                if (myRoadLow != null) myRoadLow.sprite = sprite;
                

                break;
            case GraphicQualitySettings.Medium:
                Sprite sprite1 = (Sprite)Resources.Load<Sprite>(LowIMGPath + "sprite_" + roadDirToString(dir));
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

    public void UpdateMaterial(string LowIMGPath)
    {
        if (PermanentVisuals) return;
        RoadDirection dir = _direction;
        if (myRoad != null)
        {
            if (myRoad.gameObject.activeSelf == true)
            {
                myRoad.SetFrame((int)dir);
            }
        }
        if (myRoadLow != null)
        {
            if (myRoadLow.gameObject.activeSelf == true)
            {
                string s = (string)LowIMGPath + "sprite_" + (string)roadDirToString(dir);
                Sprite sp = Resources.Load<Sprite>(s);
                myRoadLow.sprite = sp;
            }
        }



    }


    public void ReEnableSprites()
    {
        ClickableSprite.SetActive(true);
    }

    /// <summary>
    /// Selecciona que Sprites deben estar activados en función de si la carretera se puede mover.
    /// </summary>
    public void RecalculateSprites(RoadDirection RdPurple = RoadDirection.NEWS)
    {
        TurnOffAllSprites();
        if (TypeOfRotation != RoadRotationType.Gold && TypeOfRotation != RoadRotationType.Purple)
        {            
            return;
        }
        RoadDirection currentState = direction;
        RoadDirection nextState = RoadDirection.NEWS;
        if (TypeOfRotation == RoadRotationType.Gold) nextState = possibleGoldRotations[0];
        if (TypeOfRotation == RoadRotationType.Purple)
        {
            if (RdPurple == RoadDirection.NEWS) return;
            nextState = RdPurple;
        }
        if (nextState == RoadDirection.NEWS) return;
        CardinalPoint[] differences;
        CardinalPoint[] equals = currentState.Compare(nextState, out differences, true);
        List<CardinalPoint> changed = new List<CardinalPoint>();
        foreach (CardinalPoint dir in differences)
        {
            if (dir != CardinalPoint.None)
            {
                TurnOnArrow(dir);
                changed.Add(dir);
            }
        }
        if (TypeOfRotation == RoadRotationType.Purple) return;
        foreach (CardinalPoint dir1 in equals)
        {
            if (dir1 != CardinalPoint.None)
            {
                TurnOnEquals(dir1);
                changed.Add(dir1);
            }
        }
        if (possibleGoldRotations.Count <= 2 || TypeOfRotation != RoadRotationType.Gold) return;
        RoadDirection AfterState = possibleGoldRotations[1];
        equals = nextState.Compare(AfterState, out differences, true);
        foreach (CardinalPoint dir in differences)
        {
            if (dir != CardinalPoint.None && (changed.Contains(dir) == false))
            {
                //TurnOnArrowTrans(dir);
                changed.Add(dir);
            }
        }

    }

    void TurnOffAllSprites()
    {
        foreach (GameObject go in sprites)
        {
            if (go.activeSelf && go.transform.parent.gameObject.activeSelf)
            {
                if (GameConfig.s != null)
                {
                    go.GetComponent<SpriteRenderer>().color = GameConfig.s.publicColors[(int)enumColor.Black];
                }
                go.GetComponent<SpriteRenderer>().enabled = false;
                go.GetComponent<Animator>().SetBool("Moving", false);
            }
        }
    }
    void TurnOffSprite(CardinalPoint dir)
    {
        GameObject go = sprites[((int)dir) - 1];
        go.GetComponent<SpriteRenderer>().enabled = false;
        go.GetComponent<Animator>().SetBool("Moving", false);
    }
    void TurnOnArrow(CardinalPoint dir)
    {
        GameObject go = sprites[(int)dir - 1];
        if (go.activeSelf && go.transform.parent.gameObject.activeSelf)
        {
            SpriteRenderer ren = go.GetComponent<SpriteRenderer>();
            ren.enabled = true;
            ren.sprite = (Sprite)Resources.Load(GameConfig.s.IMGPath + "Arrow", typeof(Sprite));
            go.GetComponent<Animator>().SetBool("Moving", true);
        }
    }
    void TurnOnArrowTrans(CardinalPoint dir)
    {
        GameObject go = sprites[(int)dir - 1];
        if (go.activeSelf && go.transform.parent.gameObject.activeSelf)
        {
            SpriteRenderer ren = go.GetComponent<SpriteRenderer>();
            ren.enabled = true;
            ren.sprite = (Sprite)Resources.Load(GameConfig.s.IMGPath + "ArrowTrans", typeof(Sprite));
            go.GetComponent<Animator>().SetBool("Moving", false);
        }
    }
    void TurnOnEquals(CardinalPoint dir)
    {
        GameObject go = sprites[(int)dir - 1];
        if (go.activeSelf && go.transform.parent.gameObject.activeSelf)
        {
            SpriteRenderer ren = go.GetComponent<SpriteRenderer>();
            ren.enabled = true;
            ren.sprite = (Sprite)Resources.Load(GameConfig.s.IMGPath + "ArrowTrans", typeof(Sprite));
            go.GetComponent<Animator>().SetBool("Moving", false);
        }
    }
    

#endregion

    #region Check for trucks on top

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Truck") NumberOfTruckOnTop += 1;        
        if (other.tag == "Truck2") numberOfComingTrucks += 1;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Truck") NumberOfTruckOnTop -= 1;
        if (other.tag == "Truck2") numberOfComingTrucks -= 1; 
        if (NumberOfTruckOnTop < 0) NumberOfTruckOnTop = 0;
        if (numberOfComingTrucks < 0) numberOfComingTrucks = 0;
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
        CardinalPoint from1to2;
        if (!road1.position.CheckAdjacencyWith(road2.position, out from1to2)) return false;
        //Igual en la otra dirección
        CardinalPoint from2to1;
        if (!road2.position.CheckAdjacencyWith(road1.position, out from2to1)) return false; ;

        //Si llegamos hasta aquí, estan adyacentes y ambos "fromXtoX" tienen información

        //CheckAdjacency devuelve "true" y el out en "TruckDirection.None" (en ambos)
        //Si se trata, casualmente, de la misma casilla, devolvemos true automaticamente
        if (from1to2 == CardinalPoint.None || from2to1 == CardinalPoint.None) return true;


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
    public static CardinalPoint ReverseDirection(CardinalPoint direction)
    {
        CardinalPoint result = CardinalPoint.None;
        switch (direction)
        {
            case CardinalPoint.N:
                result = CardinalPoint.S;
                break;
            case CardinalPoint.E:
                result = CardinalPoint.W;
                break;
            case CardinalPoint.W:
                result = CardinalPoint.E;
                break;
            case CardinalPoint.S:
                result = CardinalPoint.N;
                break;
            case CardinalPoint.None:
                result = CardinalPoint.None;
                break;
            default:
                result = CardinalPoint.None;
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
    public static string greenDirtoString(RoadRotationGreen road)
    {
        string s;
        switch (road)
        {
            case RoadRotationGreen.NE:
                s = NE;
                break;
            case RoadRotationGreen.SE:
                s = SE;
                break;
            case RoadRotationGreen.SW:
                s = SW;
                break;
            case RoadRotationGreen.NW:
                s = NW;
                break;
            default:
                s = NE;
                break;
        }
        return s;

    }
    public static string blueDirtoString(RoadRotationBlue road)
    {
        string s;
        switch (road)
        {
            case RoadRotationBlue.EW:
                s = EW;
                break;
            case RoadRotationBlue.NS:
                s = NS;
                break;
            default:
                s = EW;
                break;
        }
        return s;
    }


    #endregion
}


/*
#region extensions

[System.Serializable]
public enum RoadDirection { EW, NE, NEWS, NS, NW, SE, SW }
public static class RoadDirectionExtensions
{


    /// <summary>
    /// Compara dos "RoadDirection" Devuelve las similitudes en array de Truckdirection
    /// mediante el RETURN.
    /// Devuelve las diferencias en array de CardinalPoint mediante out.
    /// Ambas arrays varian entre 0 y 2 elementos
    /// </summary>
    /// <param name="thisRoad">selfroad</param>
    /// <param name="otherRoad">La otra carretera a comparar</param>
    /// <param name="diferences">OUT de array de TruckDirection</param>
    /// <param name="focusDifferencesOnOtherRoad">Si es false, el out "differences" devuelve=> ¿Que tiene esta carretera que no tenga "otherRoad"?. Si es true => ¿Que tiene la otra carretera que no tenga esta?</param>
    /// <returns>array de TruckDirection</returns>
    public static CardinalPoint[] Compare(this RoadDirection thisRoad, RoadDirection otherRoad, out CardinalPoint[] diferences, bool focusDifferencesOnOtherRoad = false)
    {
        bool b = focusDifferencesOnOtherRoad; //alias
        //Arrays temporales a llenar
        CardinalPoint[] diff = new CardinalPoint[2];
        CardinalPoint[] result = new CardinalPoint[2];
        //contadores para las arrays
        int i = 0; //result
        int e = 0; //diff

        //MainLoop de 1 a 4. el ENUM TruckDirection puede ser casteado a INT (y viceversa)
        //Nota: Enum Truckdirection. None = 0, N = 1, E = 2, W = 3, S = 4.
        for (int index = 1; index <= 4; index++)
        {
            //Sacamos la dirección que vamos a comprobar en este loop:
            CardinalPoint dir = (CardinalPoint)index; //Cast de int a TruckDirection

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
        CardinalPoint[] resultd = new CardinalPoint[result.Length];
        for (int x = 0; x < result.Length; x++)
        {
            resultd[x] = result[x];
        }

        diferences = new CardinalPoint[diff.Length];
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
    public static bool HasDirection(this RoadDirection roadDirection, CardinalPoint direction)
    {
        bool r = false;
        switch (direction)
        {
            case CardinalPoint.N:
                r = roadDirection.HasN();
                break;
            case CardinalPoint.E:
                r = roadDirection.HasE();
                break;
            case CardinalPoint.W:
                r = roadDirection.HasW();
                break;
            case CardinalPoint.S:
                r = roadDirection.HasS();
                break;
            case CardinalPoint.None:
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
    public static List<RoadDirection> GetAll(this RoadDirection thisRD)
    {
        List<RoadDirection> rds = ((RoadDirection[])Enum.GetValues(typeof(RoadDirection))).ToList();
        //rds.Remove(RoadDirection.NEWS);
        return rds;
    }

}

public enum RoadRotationGreen { NE = 0, SE = 1, SW = 2, NW = 3 }
public static class RoadRotationGreenExtensions
{
    
    public static RoadDirection toRoadDirection(this RoadRotationGreen greenRoadDirection)
    {
        switch (greenRoadDirection)
        {
            case RoadRotationGreen.NE:
                return RoadDirection.NE;
            case RoadRotationGreen.SE:
                return RoadDirection.SE;
            case RoadRotationGreen.SW:
                return RoadDirection.SW;
            case RoadRotationGreen.NW:
                return RoadDirection.NW;
            default:
                return RoadDirection.NE;
        }
    }
    public static RoadRotationGreen Next(this RoadRotationGreen greenRoadDirection)
    {

        switch (greenRoadDirection)
        {
            case RoadRotationGreen.NE:
                return RoadRotationGreen.SE;
            case RoadRotationGreen.SE:
                return RoadRotationGreen.SW;
            case RoadRotationGreen.SW:
                return RoadRotationGreen.NW;
            case RoadRotationGreen.NW:
                return RoadRotationGreen.NE;
            default:
                return RoadRotationGreen.NE;
        }
    }
    public static RoadRotationBlue GreenToBlue(this RoadRotationGreen greenRoadDirection)
    {
        switch (greenRoadDirection)
        {
            case RoadRotationGreen.NE:
                return RoadRotationBlue.EW;
            case RoadRotationGreen.SE:
                return RoadRotationBlue.NS;
            case RoadRotationGreen.SW:
                return RoadRotationBlue.EW;
            case RoadRotationGreen.NW:
                return RoadRotationBlue.NS;
            default:
                return RoadRotationBlue.EW;
        }
    }
}


public enum RoadRotationBlue { EW = 0, NS = 1 }
public static class RoadRotationBlueExtensions
{

    public static RoadDirection toRoadDirection(this RoadRotationBlue blueRoadDirection)
    {
        switch (blueRoadDirection)
        {
            case RoadRotationBlue.EW:
                return RoadDirection.EW;
            case RoadRotationBlue.NS:
                return RoadDirection.NS;
            default:
                return RoadDirection.EW;
        }
    }
    public static RoadRotationBlue Next(this RoadRotationBlue blueRoadDirection)
    {
        switch (blueRoadDirection)
        {
            case RoadRotationBlue.EW:
                return RoadRotationBlue.NS;

            case RoadRotationBlue.NS:
                return RoadRotationBlue.EW;

            default:
                return RoadRotationBlue.EW;
        }
    }
    public static RoadRotationGreen BlueToGreen(this RoadRotationBlue blueRoadDirection)
    {
        switch (blueRoadDirection)
        {
            case RoadRotationBlue.EW:
                return RoadRotationGreen.SE;

            case RoadRotationBlue.NS:
                return RoadRotationGreen.NW;

            default:
                return RoadRotationGreen.NE;

        }
    }

}


public enum RoadPositionPurple { None = 0, N = 1, E = 2, S = 3, W = 4 }
public static class RoadRotationPurpleExtensions
    { 

    public static RoadPositionPurple NextPurpleCardinalPoint(this RoadPositionPurple purpleRoadcurrent, RoadPositionPurple purpleRoadAnchor, ref int Direction)
    {
        if (Direction != -1 && Direction != 1) return RoadPositionPurple.None;
        int currentPos = (int)purpleRoadcurrent;
        int anchorPos = (int)purpleRoadAnchor;
        currentPos = currentPos.AddwithBoundaries(Direction, 1, 4);
        if (currentPos == anchorPos)
        {
            //We bounce
            Direction = -Direction;
            //Now we advance 2 (on the other direction)
            currentPos = currentPos.AddwithBoundaries(Direction * 2, 1, 4);
        }
        return (RoadPositionPurple)currentPos;
    }
    public static RoadDirection ComposeRoadDirection(this RoadPositionPurple A, RoadPositionPurple B)
    {
        List<RoadDirection> result = new List<RoadDirection>();
        RoadDirection dummy = RoadDirection.NEWS;
        result = dummy.GetAll().FindAll(x => x.HasDirection(A.ToCardinalPoint()) && x.HasDirection(B.ToCardinalPoint()));
        result.Remove(RoadDirection.NEWS);
        return result[0];
    }
    
    public static CardinalPoint ToCardinalPoint(this RoadPositionPurple roadPositionPurple)
    {
        switch (roadPositionPurple)
        {
            case RoadPositionPurple.None:
                return CardinalPoint.None;                
            case RoadPositionPurple.N:
                return CardinalPoint.N;
            case RoadPositionPurple.E:
                return CardinalPoint.E;                
            case RoadPositionPurple.S:
                return CardinalPoint.S;
            case RoadPositionPurple.W:
                return CardinalPoint.W;                
            default:
                return CardinalPoint.None;
                //break;
        }

    }
    
    
}
#endregion
*/
