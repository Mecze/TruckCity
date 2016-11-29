using UnityEngine;
using System.Collections;

public class RoadColl : MonoBehaviour {
    
    [SerializeField]
    RoadEnt myRoadEnt;

    [SerializeField]
    CardinalPoint myCardinalPoint;

    [HideInInspector]
    bool firstCheck;

    void Awake()
    {
        firstCheck = true;
    }

    void OnTriggerEnter(Collider other)
    {       
        #region Truck Checker

        if (other.gameObject.tag == "Truck")//Es un Camion:
        {
            TruckEnt otherTruck = other.GetComponent<TruckEnt>();
            //We try to check now!
            //Thanks to 1st line in CheckTruck, we only care if truck is our way
            CheckTruck(otherTruck, false, false);

            //if (otherTruck.Direction != myCardinalPoint)
            //{
                //Also, in case the truck TURNS and the trigger has already been triggered:
                //we register checktruck on the EVENT that fires when truck turns.
                //otherTruck.OnTruckDirectionChanged += CheckTruck;
            //}
        }
        #endregion
    }
    /*
    void OnTriggerExit(Collider other)
    {

        #region Truck Checker disable
        if (other.tag == "Truck")
        {
            TruckEnt otherTruck = other.GetComponent<TruckEnt>();
            //we forget the to check this truck when it turns!
            //otherTruck.OnTruckDirectionChanged -= CheckTruck;
        }
        #endregion
    }
    */
    #region Main Check for TRUCK!
    //This is, also, a listener
    void CheckTruck(TruckEnt theTruck, bool ComesFromAdjacentRoadChange, bool ComesFromTruckDirectionChange)
    {
        bool pass = false;
        if (theTruck.Direction == CardinalPoint.None)
        {
            if (ComesFromAdjacentRoadChange && theTruck.lastDirection == myCardinalPoint) pass = true;
        }else
        {
            if (theTruck.Direction == myCardinalPoint) pass = true;
        }
        if (!pass) return;
        //we only check trucks on top of our own road
        if (theTruck.StandingRoad != myRoadEnt) return;

        CardinalPoint truckDirection = theTruck.Direction;        
        if (truckDirection == CardinalPoint.None) { truckDirection = theTruck.lastDirection; }

        //Resolve Rotation Will resolve the Rotation for a 2 exit roads and return
        //the new direction for the truck, depending on current road.
        bool ForceApplyEffect= false; 
        if (ComesFromTruckDirectionChange)
        {
            //This will avoid to Raise this event again
            // .Point is the same as direction but doesnt raise events
            theTruck.Point = myRoadEnt.myDirection.ResolveRotation(truckDirection, theTruck.lastDirection,out ForceApplyEffect);
        }
        else
        {
            //We change direction and but we may raise this event one more tiem.
            theTruck.Direction = myRoadEnt.myDirection.ResolveRotation(truckDirection, theTruck.lastDirection, out ForceApplyEffect);
        }

        if (ForceApplyEffect) theTruck.ApplyNoRotationEffect();

        //If the direcction didnt change in the last sentence we continue
        if (theTruck.Direction != myCardinalPoint) return;


        //Now, the truck will try to enter the next road
        //We look for next road!
        RoadEnt NextRoad = null;
        bool attached = MapController.s.CheckNextTile(myRoadEnt.position, myCardinalPoint, out NextRoad);

        //First we check if the NextRoad Exits
        //That's what attached is for!
        if (!attached)
        {
            //We turn the truck arround, this Road Collider 
            //is not attacehd to any [next] road.
            ForceTurn(theTruck, ComesFromTruckDirectionChange);
        }else
        {
            //Next Road exits!
            
            if (NextRoad == null) return; //FAILSAFE
            
            //Now we check if next road has the direction for the truck to enter            
            if (NextRoad.myDirection.HasDirection(theTruck.Direction.Reverse()))
            {
                //Next Road supports an entrance for the truck

                //We do nothing (for now)
            }else
            {
                //Next road DOES NOT supports an entrance for the truck
                //we turn the truck arround.
                ForceTurn(theTruck, ComesFromTruckDirectionChange);
            }


        }      
    }


    /// <summary>
    /// Turns the truck arround
    /// </summary>
    /// <param name="other"></param>
    void ForceTurn(TruckEnt other, bool tooPoint)
    {
        //We are trying to turn the truck around.
        
        RoadEnt RE = null;
        CardinalPoint direction;
        //This if will check if the OTHER exit of this ROAD has an exit
        if (MapController.s.CheckNextTile(myRoadEnt.position, myRoadEnt.myDirection.GetOther(other.Direction), out RE))
        {//if it has it, we are safe to turn around
            direction = other.Direction.Reverse();
        }else
        {// if it doesnt, we stop the truck
            direction = CardinalPoint.None;
        }
        if (tooPoint)
        {
            other.Point = direction;
        }else
        {
            other.Direction = direction;
        }


       
        
    }
    #endregion

/*
    void OnTriggerStay(Collider other)
    {
        if (firstCheck)
        {
            if (other.gameObject.tag == "Truck")//Es un Camion:
            {
                TruckEnt otherTruck = other.GetComponent<TruckEnt>();
                CheckTruck(otherTruck, false);
                otherTruck.OnTruckDirectionChanged += CheckTruck;
            }
            firstCheck = false;
        }
    }
    */
    #region Register/Unregister our Checks on Trucks Events

    public void RegisterYourCheckOnThisTruck(TruckEnt theTruck)
    {
        theTruck.OnTruckDirectionChanged += CheckTruck;
    }

    public void UnRegisterYourCheckOnThisTruck(TruckEnt theTruck)
    {
        theTruck.OnTruckDirectionChanged -= CheckTruck;
    }
    #endregion

}
