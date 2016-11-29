using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RoadEnt))]
public class GreenRotationRoad : MonoBehaviour {
    #region Attributes
    [Header("Parameter")]
    [SerializeField]
    RoadRotationGreen StartingRotation;
    [SerializeField]
    int MaximunNumberOfRotationsWithTrucks = 1;
    [SerializeField]
    Color Green;
    [SerializeField]
    Color Red;

    [Header("References")]
    [SerializeField]
    SpriteRenderer ClickSprite;
    [SerializeField]
    Animator RotateAnimator;
    [SerializeField]
    Transform RotateTransform;

    int OnTopTrucksCounter = 0;
    int _RotationsWithTrucks = 0;
    int NumberOfClicks = 0;
    
    

    #endregion

    #region Properties
    RoadDirection _myDirection;
    private RoadDirection myDirection
    {
        get
        {
            return myRoadEnt.myDirection;
        }

        set
        {
            myRoadEnt.myDirection = value;
        }
    }

    RoadEnt _myRoadEnt;
    private RoadEnt myRoadEnt
    {
        get
        {
            if (_myRoadEnt == null) _myRoadEnt = GetComponent<RoadEnt>();
            return _myRoadEnt;
        }        
    }

    private List<TruckEnt> TruckOnTop
    {
        get
        {
            return myRoadEnt.OnTopTrucks;
        }
    }

    public int RotationsWithTrucks
    {
        get
        {
            return _RotationsWithTrucks;
        }

        set
        {
            Debug.Log("Rotation With Trucks: " + _RotationsWithTrucks.ToString() + " -> " + value.ToString());
            _RotationsWithTrucks = value;
        }
    }

    #endregion

    #region Initialization
    void Awake()
    {
        InitializeAnimator();
        SetSpriteGreen();
    }

    /// <summary>
    /// Animator initialization
    /// </summary>
    void InitializeAnimator()
    {
        GoToRotation(StartingRotation, true);    
    }
    #endregion

    #region Main rotation method
    /// <summary>
    /// Main Rotation Method. It tells animator to rotate towards the specified rotation
    /// Clockwise.
    /// </summary>
    /// <param name="toRotation">target rotation</param>
    /// <param name="NoAnim">if TRUE, the rotation is instant</param>
    public void GoToRotation(RoadRotationGreen toRotation, bool NoAnim = false)
    {
        RoadDirection newRD = toRotation.toRoadDirection();
        if (newRD == myDirection) return;
        string s = "";
        switch (toRotation)
        {
            case RoadRotationGreen.NE:
                s = "GoToNE";
                break;
            case RoadRotationGreen.SE:
                s = "GoToSE";
                break;
            case RoadRotationGreen.SW:
                s = "GoToSW";
                break;
            case RoadRotationGreen.NW:
                s = "GoToNW";
                break;
            default:
                break;
        }
        RotateAnimator.SetBool("NoAnim", NoAnim);
        RotateAnimator.SetBool(s, true);
        myDirection = newRD;

    }

    #endregion

    #region Onclick

    //This is Called from NGUI event System.
    void OnPress(bool isPressed)
    {
        //Failure states.
        if (!isPressed) return; //Required by NGUI

        //Maximun number of stacked clicks (with or without trucss (This number will reset to 0 when the animation happens)
        if (NumberOfClicks > 3) return; 

        //Maximun number of clicks with a Truck standing on it.
        //It will reset when the SAME amount of trucks LEAVE this road.
        if (RotationsWithTrucks >= MaximunNumberOfRotationsWithTrucks) return;

        //upon clicking we force all trucks withing the vecinity to 
        //Check their colliders, because prestablized save routes might change
        MapController.s.GreenTileClicked(myRoadEnt.position);

        //StartingRotation is a missfortuned name. Also serves as "CurrentRotation" once the game has started.
        // .Netx() will turn this 90� degrees
        StartingRotation = StartingRotation.Next();

        //We attend each standing Truck on this road (truckontop comes from RoadEnt)
        foreach (TruckEnt te in TruckOnTop)
        {          
            //In case of multiple clicks we want to store the first "oldDirection" of each truck
            //For that, oldDirection will go back to "None" when the process is complete, so we only change when it's "None"  
            if (te.oldDirection == CardinalPoint.None) te.oldDirection = te.Direction;
            //Once current direction is saved onto oldDirection we are free to Stop the truck on it's tracks
            te.Direction = CardinalPoint.None;
            //this counter, on the truck, will store the current amount of 90� turns the truck suffers
            //Do to this GreenRoad clicking
            te.NumberOfGreenRotations += 1;            
            //We reduce the speed of the truck
            te.CurrentSpeed = 0.1f;
        }
        //Counts for the maximun number of stacked clicks. to avoid Animation overlapping
        NumberOfClicks += 1;

        //ONLY if we have any number of trucks over this Road we add to RotationWithTrucks
        if (myRoadEnt.OnTopTrucks.Count > 0) RotationsWithTrucks += 1;        

        //Now we feed the Animator with the current NEW GreenRoadRotation.
        GoToRotation(StartingRotation);

        //Only if we have reach the maximun number of clicks with trucks on top
        //We turn our sprite RED (for now) and Save the Number of trucks on top we had when this happened (OnTopTrucksCounter)
        //Later on we will count back this number using the here.
        if (RotationsWithTrucks >= MaximunNumberOfRotationsWithTrucks && RotationsWithTrucks > 0)
        {
            //Turns the Road red
            SetSpriteRed();
            //Save number of trucks over this road NOW
            OnTopTrucksCounter = myRoadEnt.OnTopTrucks.Count;           
        }




    }

    //These two methods will subscribe and unsub. the method that resets
    //"OnTopTrucksCounter" to 0.
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Truck")
        {
            myRoadEnt.OnTruckLeftTheRoad += CheckResetRotationsListener;
        }
    }
   /* void OnTriggerEexit(Collider other)
    {
        if (other.tag == "Truck")
        {
            //myRoadEnt.OnTruckLeftTheRoad -= CheckResetRotationsListener;
        }
    }
*/
    /// <summary>
    /// This is passed as a listener. It is fired everytime a truck leaves this road.
    /// </summary>
    /// <param name="trucksLeft"></param>
    void CheckResetRotationsListener(int trucksLeft)
    {
        OnTopTrucksCounter -= 1;
        if (OnTopTrucksCounter < 0) OnTopTrucksCounter = 0;
        if (OnTopTrucksCounter == 0)
        {
            SetSpriteGreen();
            RotationsWithTrucks = 0;
            myRoadEnt.OnTruckLeftTheRoad -= CheckResetRotationsListener;
        }

    }
    /// <summary>
    /// This is called from the animator to Parent all the trucks to the ROTATE transform
    /// </summary>
    public void ParentTrucksToMe()
    {
        //This is called from the animator when the road "Starts a transition".
        if (TruckOnTop.Count > 0)
        {
            foreach (TruckEnt te in TruckOnTop)
            {
                //We anchor the trucks on top of the rotation road
                te.transform.SetParent(RotateTransform);
                //We add an effects to the truck that will allow its animator to ignore curves and turns (it last certain time)
                te.ApplyNoRotationEffect();
            }
        }
    }
    /// <summary>
    /// This is called from the animator to unparent all the trucks from the animator
    /// </summary>
    /// <param name="direction"></param>
    public void UnParentTrucksOnMe(string direction)
    {
        //This is called from the animator when the road "Ends a transition".

        //Count back the number of clicks that were given at the begining
        NumberOfClicks -= 1;

        //ONLY if all clicks have been rotated by the animator we give closure 
        //to the whole process
        if (NumberOfClicks > 0) return;

        //Closure:

        //We reset the number of clicks
        NumberOfClicks = 0;        
        //we Unparent the Trucks from our Animator
        foreach (TruckEnt te in TruckOnTop)
        {
            //We ignore Trucks not currently parented to our transform (we did parent them before)
            //This is to avoid bugs. Any other truck "might" drive in in the middle of an animation (shouldnt ever occur)
            if (te.transform.parent == RotateTransform)
            {
                //We unparent them
                te.transform.SetParent(null);
                //we process the drection we saved before. we turn it 90� degrees the same amount of times that clicks were given
                te.oldDirection = te.oldDirection.TurnRight(te.NumberOfGreenRotations);
                //then we reset the int
                te.NumberOfGreenRotations = 0;
                //Now we start the truck by given it it's calculated new direction
                if (te.oldDirection != CardinalPoint.None) te.Direction = te.oldDirection;
                //we reset the oldDirection so the truck can endure this process again
                te.oldDirection = CardinalPoint.None;
                //we call this to Fix rotations on the truck and avoid float and animator minor shifts
                te.FinishRotation();
            }

        }
        //After all. As a road has finally changed we call all adjacent trucks to reconsider their tracks
        MapController.s.GreenTileClicked(myRoadEnt.position);


    }



    #endregion

    #region Sprite Management

    void SetSpriteGreen()
    {
        ClickSprite.color = Green;
    }
    void SetSpriteRed()
    {
        ClickSprite.color = Red;
    }


    #endregion


}
