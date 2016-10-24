using UnityEngine;
using System.Collections;
using System;

//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// Truck Entity
//////////////////////////////
/// "Truck" es un agente autonomo que se mueve por el mundo
///  gracias a sus scripts. Este es el script principal de "Truck"
//////////////////////////////


public enum CardinalPoint { None=0,N=1,E=2,W=3,S=4}
public enum HonkType { None=0,Single=1,Double=2 }
public enum Turn { Close, Wide, Reverse }
#pragma warning disable 0649
public class TruckEntity : MonoBehaviour, IFreezable
{

    public CardinalPoint direction;
    public bool Moving = true;
    bool freeze = false;



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
    private bool _colliding = false;
    public bool Colliding
    {
        get
        {
            return _colliding;
        }

        set
        {
            _colliding = value;
            if (value) PlayHonk();
        }
    }

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

    [Header("Gas")]
    [SerializeField]
    float currentGas;
    [SerializeField]
    float maxGas;
    [SerializeField]
    float gasConsumption;
    [SerializeField]
    float depletedGasThreshold;
    [SerializeField]
    GasMeter gasMeter;

    [Header("Cargo")]
    public Cargo myCargo;
#pragma warning disable 0414
    int Debug = 0;
#pragma warning restore 0414

    [Header("Sounds")]
    [SerializeField]
    string HonkAlias = "HonkA";
    [SerializeField]
    HonkType HonkType = HonkType.Single;
    [SerializeField]
    float waitBetweenHonk = 0.2f;
#pragma warning restore 0649

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

    void Awake()
    {
        currentSpeed = 0f;
    }

    void Update()
    {
        if (freeze) return;
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
            
            if (gasMeter.currentGas <= 0f && currentSpeed > depletedGasThreshold) currentSpeed = depletedGasThreshold;            
            //We Check if we are going to pass



            this.transform.Translate(Vector3.Lerp(this.transform.position,Vector3.forward * Time.deltaTime * currentSpeed,1f));
            ConsumeGas(currentSpeed);

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
    void ConsumeGas(float thisspeed)
    {
        if (thisspeed < depletedGasThreshold) return;
        currentGas -= Time.deltaTime * (gasConsumption);
        if (currentGas <= 0) currentGas = 0;
        gasMeter.currentGas = currentGas / maxGas;

    }

    public void RefilGas()
    {
        currentGas = maxGas;
    }



    #region change direction

    public void CheckDirection(RoadDirection newroaddirection, Turn checkTurn)
    {
        CardinalPoint newtruckdirection = direction;
        switch (newroaddirection)
        {
            #region LineasRectas
            //LineaRecta E <-> W
            case RoadDirection.EW:
                if (direction == CardinalPoint.W)
                {
                    newtruckdirection = CardinalPoint.W;
                }
                if (direction == CardinalPoint.E)
                {
                    newtruckdirection = CardinalPoint.E;
                }
                break;
            //LineaRecta N <-> S
            case RoadDirection.NS:
                if (direction == CardinalPoint.S)
                {
                    newtruckdirection = CardinalPoint.S;
                }
                if (direction == CardinalPoint.N)
                {
                    newtruckdirection = CardinalPoint.N;
                }
                break;
            #endregion

            case RoadDirection.NEWS:
                //Todavia no hay!
                break;

            #region Curvas
            case RoadDirection.NE:
                if (direction == CardinalPoint.S)
                {
                    if (checkTurn != Turn.Wide) return;                    
                    newtruckdirection = CardinalPoint.E;
                }
                if (direction == CardinalPoint.W)
                {
                    if (checkTurn != Turn.Close) return;
                    newtruckdirection = CardinalPoint.N;
                }
                break;
            case RoadDirection.NW:
                if (direction == CardinalPoint.S)
                {
                    if (checkTurn != Turn.Close) return;
                    newtruckdirection = CardinalPoint.W;
                }
                if (direction == CardinalPoint.E)
                {
                    if (checkTurn != Turn.Wide) return;
                    newtruckdirection = CardinalPoint.N;
                }
                break;
            case RoadDirection.SE:
                if (direction == CardinalPoint.N)
                {
                    if (checkTurn != Turn.Close) return;
                    newtruckdirection = CardinalPoint.E;
                }
                if (direction == CardinalPoint.W)
                {
                    if (checkTurn != Turn.Wide) return;
                    newtruckdirection = CardinalPoint.S;
                }
                break;
            case RoadDirection.SW:
                if (direction == CardinalPoint.N)
                {
                    if (checkTurn != Turn.Wide) return;
                    newtruckdirection = CardinalPoint.W;
                }
                if (direction == CardinalPoint.E)
                {
                    if (checkTurn != Turn.Close) return;
                    newtruckdirection = CardinalPoint.S;
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


    public void ChangeDirection(CardinalPoint newdirection, Turn turn, float additionOffset = 0f)
    {
        if (turn == Turn.Reverse && otherLaneTrucks > 0)
        {
            waitingToOtherLaneToClearToReverse = true;
            Moving = false;
            return;       
        }
        if (turn == Turn.Reverse)
        {
            //UnityEngine.Debug.Log("DebugReverse");
        }

        /*
        if (turn == Turn.Reverse && newdirection == TruckDirection.W && Debug == 1)
        {
            return;
        }
        if (turn == Turn.Reverse && newdirection == TruckDirection.W) Debug = 1;
        */
        float rotateOffset = 0f;
        if (turn == Turn.Close) rotateOffset = rotateOffsetClose;
        if (turn == Turn.Wide) rotateOffset = rotateOffsetWide;
        if (turn == Turn.Reverse) rotateOffset = rotateOffsetReverse;
        Vector3 r = transform.rotation.eulerAngles;
        Vector3 offset = new Vector3(0, 0, 0);
        switch (newdirection)
        {
            case CardinalPoint.N:
                r = RotateNorth;
                gasMeter.reverse = false;
                ClampAxis(true);
                break;
            case CardinalPoint.E:
                r = RotateEast;
                gasMeter.reverse = true;     
                ClampAxis(false);
                break;
            case CardinalPoint.W:
                gasMeter.reverse = false;
                r = RotateWest;                
                ClampAxis(false);
                break;
            case CardinalPoint.S:
                r = RotateSouth;
                gasMeter.reverse = true;
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
        offset = transform.forward * (rotateOffset + additionOffset);
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
            if (direction == CardinalPoint.W || direction == CardinalPoint.N)
            {
                newPos.z = Mathf.Round(Mathf.Abs(newPos.z));
            }
            else if (direction == CardinalPoint.E || direction == CardinalPoint.S)
            {
                newPos.z = Mathf.Round(Mathf.Abs(newPos.z));
            }
            if (neg) newPos.z = -newPos.z;
        }
        else
        {
            if (newPos.x < 0) neg = true;
            if (direction == CardinalPoint.W || direction == CardinalPoint.N)
            {
                newPos.x = Mathf.Round(Mathf.Abs(newPos.x));
            }
            else if (direction == CardinalPoint.E || direction == CardinalPoint.S)
            {
                newPos.x = Mathf.Round(Mathf.Abs(newPos.x));
            }
            if (neg) newPos.x = -newPos.x;
        }

        transform.position = newPos;

    }
    #endregion

    #region Sounds

    public void PlayHonk()
    {
        switch (HonkType)
        {
            case HonkType.None:
                return;
                
            case HonkType.Single:
                SoundStore.s.PlaySoundByAlias(HonkAlias, 0f, GameConfig.s.MuffledSoundVolume);
                break;
            case HonkType.Double:
                SoundStore.s.PlaySoundByAlias(HonkAlias, 0f, GameConfig.s.MuffledSoundVolume,false,0.1f,false,0.1f,() => { SoundStore.s.PlaySoundByAlias(HonkAlias, waitBetweenHonk, GameConfig.s.MuffledSoundVolume); });
                break;
            default:
                break;
        }
        
    }

    #endregion





}


public interface IFreezable
{
    void Freeze();
    void Unfreeze();
}

