using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public delegate void TruckEntChangedDirectionEventHander(TruckEnt theTruck, bool RaisedFromRoad, bool RaisedFromTruck);

public class TruckEnt : MonoBehaviour {
    #region Atributtes
    [SerializeField]
    float StartingSpeed = 0.1f;
    [SerializeField]
    float MaxSpeed = 1f;
    [SerializeField]
    float Acceleration = 100f;
    [SerializeField]
    float turnPenalty = 0.4f;
    [SerializeField]
    float fullTurnPenalty = 0.7f;
    [SerializeField]
    float minimunTurningSpeed = 0.2f;

    [SerializeField]
    public Animator MoveAnimator;

    [HideInInspector]
    public CardinalPoint oldDirection = CardinalPoint.None;
    [HideInInspector]
    public int NumberOfGreenRotations = 0;

    #endregion

    #region Properties
    /// <summary>
    /// Get whenever the truck is moving
    /// </summary>
    public bool Moving { get { return !(_direction == CardinalPoint.None); } }

    
    [HideInInspector]
    public CardinalPoint lastDirection;
    [SerializeField]
    CardinalPoint _direction;
    /// <summary>
    /// Gets/Sets current truck direction
    /// Change the Truck direction to make ir turn.
    /// Same as "Point" but fires OnTruckDirectionChanged
    /// </summary>
    public CardinalPoint Direction
    {
        get
        {
            return _direction;
        }

        set
        {
            //We only Commit if the newDirection (value) is different
            if (_direction != value)
            {
                Point = value;
                if (_onTruckDirectionChanged != null) _onTruckDirectionChanged(this, false, true);
            }
        }
    }

    /// <summary>
    /// Gets/Sets current truck direction
    /// Change the Truck direction to make ir turn.
    /// </summary>
    public CardinalPoint Point
    {
        get
        {
            return _direction;
        }
        set
        {
            if (_direction != value)
            {
                //We Save Last Direction for later use
                lastDirection = _direction;

                //Commit the change:
                _direction = value;

                //Commit the change to the animator (Direction Integer in Animator will never be 0)
                //Instead, if CardinalPoint.None (0) Moving will be false.
                MoveAnimator.SetBool("Moving", (_direction != CardinalPoint.None));

                //the Commit to direction happens here
                if (_direction != CardinalPoint.None) MoveAnimator.SetInteger("Direction", (int)_direction);

                //WHY? it we will try to keep the former direction on the animator when stoppping

                //Debug
                Debug.Log("Direction Changed: " + lastDirection.ToString(false) + " -> " + Direction.ToString(false));

                //In Case the truck is stopped, the animator usually "Cuts through"
                //other animations. Therefore the "Direction" Integer on the Animator
                //and the REAL physic direction of the truck are different
                //Next if tries to solve that.
                if (_direction == CardinalPoint.None)
                {
                    CardinalPoint CP = GuessRealDirection();
                    if (CP != CardinalPoint.None) MoveAnimator.SetInteger("Direction", (int)CP);
                }
            }          
        }
    }
       
    /// <summary>
    /// Gets current Speed from the animator itself
    /// </summary>
    public float CurrentSpeed
    {
        get
        {
            return MoveAnimator.GetFloat("Speed");
        }

        set
        {
            MoveAnimator.SetFloat("Speed", value);                
        }
    }

    private RoadEnt standingRoad;
    /// <summary>
    /// A Reference to the road the truck is currently standing on.
    /// </summary>
    public RoadEnt StandingRoad
    {
        get
        {
            return standingRoad;
        }

        set
        {
            if (standingRoad != value)
            {
                //We try to register ourSelves on the new Road
                if (value != null) value.AddOnTopTruck(this);
                //We try to un-register ourSelves on the old Road
                if (standingRoad != null) standingRoad.RemoveOnTopTruck(this);
            }
            //Update value
            standingRoad = value;
        }
    }

    /// <summary>
    /// This is used internally to keep track of the road position where the truck is standing
    /// it's used to discover whenever the truck has to update his standing road
    /// </summary>
    Vector2 myRoadPos;


    private TruckEntChangedDirectionEventHander _onTruckDirectionChanged;
    /// <summary>
    /// This event is fired whenever the truck turns, at the begin of the turning.
    /// </summary>    
    public event TruckEntChangedDirectionEventHander OnTruckDirectionChanged
    {
        add
        {
            if (_onTruckDirectionChanged == null || !_onTruckDirectionChanged.GetInvocationList().Contains(value))
            {
                _onTruckDirectionChanged += value;
            }    
        }
        remove
        {
            _onTruckDirectionChanged -= value;            
        }
    }
    

    #endregion

    #region Initialization
    void Awake()
    {
        UpdateMyStandingRoad(true);
        Direction = GuessRealDirection();
        InitializeAnimator();
        EventRegistration();
    }
    //This guesses the starting Direction of the truck
    public CardinalPoint GuessRealDirection()
    {
        CardinalPoint CP = CardinalPoint.None;
        float y = this.gameObject.transform.rotation.eulerAngles.y;
        if (y > 150f && y < 210f) CP = CardinalPoint.S;
        if (y > 60f && y < 130f) CP = CardinalPoint.E;        
        if (y > -30f && y < 30f) CP = CardinalPoint.N;
        if (y > 330f && y < 390f) CP = CardinalPoint.N;
        if (y > 240f && y < 300f) CP = CardinalPoint.W;
        if (y > -120f && y < -60f) CP = CardinalPoint.W;
        return CP;
    }
    //initializes de animator!
    void InitializeAnimator()
    {        
        CardinalPoint temp = _direction;
        Direction = CardinalPoint.None;
        Direction = temp;
        lastDirection = Direction.Reverse();
        //Pass all the floats to the Animator. The animator is the responsible
        //for acceleration and deceleration.
        MoveAnimator.SetFloat("MinimunTurningSpeed", minimunTurningSpeed);
        MoveAnimator.SetFloat("Speed", StartingSpeed);
        MoveAnimator.SetFloat("MaxSpeed", MaxSpeed);
        MoveAnimator.SetFloat("TurnPenalty", turnPenalty);
        MoveAnimator.SetFloat("FullTurnPenalty", fullTurnPenalty);
        MoveAnimator.SetFloat("Acceleration", Acceleration);


    }
    /// <summary>
    /// Register this truck on the corresponding events
    /// </summary>
    void EventRegistration()
    {
        MapController.s.OnGreenTileCLicked += CheckRotationsAgainListener;
        MapController.s.OnPurpleTileClicked += CheckRotationsAgainListener;

    }

    void OnDisable()
    {
        MapController.s.OnGreenTileCLicked -= CheckRotationsAgainListener;
        MapController.s.OnPurpleTileClicked -= CheckRotationsAgainListener;
    }

    void CheckRotationsAgainListener(Vector3Int position, bool CheckSelf)
    {
        CardinalPoint where = CardinalPoint.None;
        if (!position.CheckAdjacencyWith(StandingRoad.position, out where)) return;
        if (position == standingRoad.position && !CheckSelf) return;

        if (_onTruckDirectionChanged != null) _onTruckDirectionChanged(this, true, false);       
    }


    #endregion

    #region UpdateStandingRoad
    void Update()
    {
        UpdateMyStandingRoad();
        //ChangeSpeed();
    }
    /// <summary>
    /// Updates which road is this truck standing when it gets too far from it
    /// </summary>
    /// <param name="force"></param>
    void UpdateMyStandingRoad(bool force = false)
    {
        if (!force)
        {
            bool pass = false;
            float DeltaX = Mathf.Abs(this.transform.position.x - myRoadPos.x);
            float DeltaZ = Mathf.Abs(this.transform.position.z - myRoadPos.y);
            if (DeltaX > 0.501f || DeltaZ > 0.501f) pass = true;
            if (!pass) return;
        }
        //We disable the No turn effect when changing road
        MoveAnimator.SetBool("OutSideChange", false);

        //We find the new Road where we are not standing on
        //We do it by finding it on the array of all road and comparing the positions (LINQ)
        RoadEnt newRoad = GameObject.FindObjectsOfType<RoadEnt>().ToList<RoadEnt>().Find(x => Mathf.RoundToInt(x.gameObject.transform.position.x) == Mathf.RoundToInt(transform.position.x) && Mathf.RoundToInt(x.gameObject.transform.position.z) == Mathf.RoundToInt(transform.position.z));
        
        //force here, means that this method was called from "AWAKE" instead of "UPDATE".
        //So this will destroy any truck that is not standing on a road at the begining of the game
        if (newRoad == null && force) DestroyImmediate(this.gameObject);
        
        //we asign our new Standing road (see StandingRoad Property for more)
        StandingRoad = newRoad;

        //We record the position of our new road for future purposes
        myRoadPos = new Vector2(StandingRoad.transform.position.x, StandingRoad.transform.position.z);
    }

    public Vector2 DistanceToMyRoad()
    {
        Vector2 r = new Vector2();
        r.x = myRoadPos.x - transform.position.x;
        r.y = myRoadPos.y - transform.position.z;
        return r;
    }
    public float DistanceToMyRoad(bool X)
    {
        if (X)
        {
            return DistanceToMyRoad().x;
        }else
        {
            return DistanceToMyRoad().y;
        }
    }

    #endregion
        
    #region finishing rotation
    /// <summary>
    /// This is called by the animator to Clamp the position and rotation of 
    /// the truck via script (to avoid float positions to slowly shift the truck)
    /// </summary>
    public void FinishRotation()
    {
        //if moved by an outside animator, there could be slight movement from that animator
        //so we guess the direction again.

        //if (FromOutSide) SetStartingDirection();

        //Now we clamp it
        //MoveAnimator.SetBool("OutSideChange", false);
        ClampRotation();        
        bool b = (_direction == CardinalPoint.N || _direction == CardinalPoint.S);
        ClampAxis(b);
    }
    void ClampRotation()
    {
        Vector3 rot = this.transform.rotation.eulerAngles;

        switch (_direction)
        {
            case CardinalPoint.None:
                break;
            case CardinalPoint.N:
                rot.y = 0f;
                break;
            case CardinalPoint.E:
                rot.y = 90f;
                break;
            case CardinalPoint.W:
                rot.y = 270f;
                break;
            case CardinalPoint.S:
                rot.y = 180f;
                break;
            default:
                break;
        }
        this.transform.rotation = Quaternion.Euler(rot);
    }
    void ClampAxis(bool verticalAxis = false)
    {
        Vector3 newPos = new Vector3();
        bool neg = false;
        newPos = transform.position;
        if (!verticalAxis)
        {
            if (newPos.z < 0) neg = true;
            if (_direction == CardinalPoint.W || _direction == CardinalPoint.N)
            {
                newPos.z = Mathf.Round(Mathf.Abs(newPos.z));
            }
            else if (_direction == CardinalPoint.E || _direction == CardinalPoint.S)
            {
                newPos.z = Mathf.Round(Mathf.Abs(newPos.z));
            }
            if (neg) newPos.z = -newPos.z;
        }
        else
        {
            if (newPos.x < 0) neg = true;
            if (_direction == CardinalPoint.W || _direction == CardinalPoint.N)
            {
                newPos.x = Mathf.Round(Mathf.Abs(newPos.x));
            }
            else if (_direction == CardinalPoint.E || _direction == CardinalPoint.S)
            {
                newPos.x = Mathf.Round(Mathf.Abs(newPos.x));
            }
            if (neg) newPos.x = -newPos.x;
        }

        transform.position = newPos;

    }
    #endregion

    #region Outside Rotation Change
    /// <summary>
    /// This will apply an effect to truck that will make it ignore rotation animations
    /// </summary>
    public void ApplyNoRotationEffect()
    {
        //We stop other courutine that disables the effect (extending the duration)
        StopCoroutine("DisableNoRotationEffect");
        //Turn on the effect on the animator
        MoveAnimator.SetBool("OutSideChange", true);
        //Programing courutine to disable de effet
        StartCoroutine("DisableNoRotationEffect");
    }

    IEnumerator DisableNoRotationEffect()
    {
        float time = (0.1f / CurrentSpeed) +0.3f;
        
        yield return new WaitForSeconds(time);
        MoveAnimator.SetBool("OutSideChange", false);
    }



    #endregion
     
}
