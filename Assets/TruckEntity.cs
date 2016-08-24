using UnityEngine;
using System.Collections;
using System;

public enum TruckDirection { None=0,N=1,E=2,W=3,S=4}

public enum Turn { Close, Wide, Reverse }

public class TruckEntity : MonoBehaviour
{

    public TruckDirection direction;
    public bool Moving = true;




    [Header("Curves Offset Position")]
    [SerializeField]
    float rotateOffsetClose = 0.4f;
    [SerializeField]
    float rotateOffsetWide = 0.4f;
    [SerializeField]
    float rotateOffsetReverse = 0.3f;

    [Header("Speed")]
    [SerializeField]
    float speed = 1f;
    [SerializeField]
    float acceleration = 0.2f;
    [SerializeField]
    float turnPenalty = 0.5f;
    [HideInInspector]
    public bool Colliding = false;
    //[HideInInspector]
    [SerializeField]
    float _currentSpeed = 0f;

    [Header("Collision with other Trucks")]
    [SerializeField]
    float ignoreCollisionWhenTurningTime = 0.2f;
    [HideInInspector]
    public bool ignoringCollisionsBecauseOfTurning = false;
    float ignoreCollisionTimer = 0f;
    [HideInInspector]
    public int otherLaneTrucks = 0;
    bool waitingToOtherLaneToClearToReverse = false;
    


    public float currentSpeed
    {
        get
        {
            return _currentSpeed;
        }

        set
        {
            if (value < 0f) {
                _currentSpeed = 0f;
                return;
            }
            if (value > speed)
            {
                _currentSpeed = speed;
                return;
            }

            _currentSpeed = value;
        }
    }

    #region Constantes de rotación
    [HideInInspector]
    public Vector3 RotateEast = new Vector3(0, 90, 0);
    [HideInInspector]
    public Vector3 RotateWest = new Vector3(0, 270, 0);
    [HideInInspector]
    public Vector3 RotateSouth = new Vector3(0, 180, 0);
    [HideInInspector]
    public Vector3 RotateNorth = new Vector3(0, 0, 0);

   
    #endregion

    void Awake()
    {
        currentSpeed = 0f;
    }

    void Update()
    {
        Movement();


        

    }

    void Movement()
    {
        if (waitingToOtherLaneToClearToReverse && otherLaneTrucks == 0)
        {
            waitingToOtherLaneToClearToReverse = false;
            Moving = true;
            ChangeDirection(RoadEntity.ReverseDirection(direction), Turn.Reverse);
        }

        if (Moving && Colliding == false)
        {
            currentSpeed += acceleration * Time.deltaTime;            
            this.transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed);

        }
        if (Colliding == true || Moving == false)
        {
            currentSpeed = 0f;
        }
        if (ignoringCollisionsBecauseOfTurning || ignoreCollisionTimer > 0f)
        {
            ignoreCollisionTimer -= Time.deltaTime;
            if (ignoreCollisionTimer <= 0f)
            {
                ignoreCollisionTimer = 0f;
                ignoringCollisionsBecauseOfTurning = false;
            }
        }
    }

    #region change direction

    public void CheckDirection(RoadDirection newroaddirection, Turn checkTurn)
    {
        TruckDirection newtruckdirection = direction;
        switch (newroaddirection)
        {
            #region LineasRectas
            //LineaRecta E <-> W
            case RoadDirection.EW:
                if (direction == TruckDirection.W)
                {
                    newtruckdirection = TruckDirection.W;
                }
                if (direction == TruckDirection.E)
                {
                    newtruckdirection = TruckDirection.E;
                }
                break;
            //LineaRecta N <-> S
            case RoadDirection.NS:
                if (direction == TruckDirection.S)
                {
                    newtruckdirection = TruckDirection.S;
                }
                if (direction == TruckDirection.N)
                {
                    newtruckdirection = TruckDirection.N;
                }
                break;
            #endregion

            case RoadDirection.NEWS:
                //Todavia no hay!
                break;

            #region Curvas
            case RoadDirection.NE:
                if (direction == TruckDirection.S)
                {
                    if (checkTurn != Turn.Wide) return;                    
                    newtruckdirection = TruckDirection.E;
                }
                if (direction == TruckDirection.W)
                {
                    if (checkTurn != Turn.Close) return;
                    newtruckdirection = TruckDirection.N;
                }
                break;
            case RoadDirection.NW:
                if (direction == TruckDirection.S)
                {
                    if (checkTurn != Turn.Close) return;
                    newtruckdirection = TruckDirection.W;
                }
                if (direction == TruckDirection.E)
                {
                    if (checkTurn != Turn.Wide) return;
                    newtruckdirection = TruckDirection.N;
                }
                break;
            case RoadDirection.SE:
                if (direction == TruckDirection.N)
                {
                    if (checkTurn != Turn.Close) return;
                    newtruckdirection = TruckDirection.E;
                }
                if (direction == TruckDirection.W)
                {
                    if (checkTurn != Turn.Wide) return;
                    newtruckdirection = TruckDirection.S;
                }
                break;
            case RoadDirection.SW:
                if (direction == TruckDirection.N)
                {
                    if (checkTurn != Turn.Wide) return;
                    newtruckdirection = TruckDirection.W;
                }
                if (direction == TruckDirection.E)
                {
                    if (checkTurn != Turn.Close) return;
                    newtruckdirection = TruckDirection.S;
                }
                break;
            #endregion
            default:
                break;
        }


        
        if (direction != newtruckdirection)
        {
            ChangeDirection(newtruckdirection, checkTurn);
        }
        direction = newtruckdirection;




    }


    public void ChangeDirection(TruckDirection newdirection, Turn turn)
    {
        if (turn == Turn.Reverse && otherLaneTrucks > 0)
        {
            waitingToOtherLaneToClearToReverse = true;
            Moving = false;
            return;       
        }

        float rotateOffset = 0f;
        if (turn == Turn.Close) rotateOffset = rotateOffsetClose;
        if (turn == Turn.Wide) rotateOffset = rotateOffsetWide;
        if (turn == Turn.Reverse) rotateOffset = rotateOffsetReverse;
        Vector3 r = transform.rotation.eulerAngles;
        Vector3 offset = new Vector3(0, 0, 0);
        switch (newdirection)
        {
            case TruckDirection.N:
                r = RotateNorth;                                
                ClampAxis(true);
                break;
            case TruckDirection.E:
                r = RotateEast;                
                ClampAxis(false);
                break;
            case TruckDirection.W:
                r = RotateWest;                
                ClampAxis(false);
                break;
            case TruckDirection.S:
                r = RotateSouth;
                ClampAxis(true);
                break;
            default:

                break;
        }
        ignoringCollisionsBecauseOfTurning = true;
        ignoreCollisionTimer = ignoreCollisionWhenTurningTime;
        direction = newdirection;
        currentSpeed -= turnPenalty;
        transform.rotation = Quaternion.Euler(r);
        offset = transform.forward * rotateOffset;
        transform.position = transform.position + offset;
        
        



    }
    void ClampAxis(bool verticalAxis = false)
    {
        Vector3 newPos = new Vector3();
        bool neg = false;
        newPos = transform.position;
        if (!verticalAxis)
        {
            if (newPos.z < 0) neg = true;
            if (direction == TruckDirection.W || direction == TruckDirection.N)
            {
                newPos.z = Mathf.Round(Mathf.Abs(newPos.z));
            }
            else if (direction == TruckDirection.E || direction == TruckDirection.S)
            {
                newPos.z = Mathf.Round(Mathf.Abs(newPos.z));
            }
            if (neg) newPos.z = -newPos.z;
        }
        else
        {
            if (newPos.x < 0) neg = true;
            if (direction == TruckDirection.W || direction == TruckDirection.N)
            {
                newPos.x = Mathf.Round(Mathf.Abs(newPos.x));
            }
            else if (direction == TruckDirection.E || direction == TruckDirection.S)
            {
                newPos.x = Mathf.Round(Mathf.Abs(newPos.x));
            }
            if (neg) newPos.x = -newPos.x;
        }

        transform.position = newPos;

    }
    #endregion







}

