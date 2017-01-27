using UnityEngine;
using System.Collections;


//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// Truck Entity
//////////////////////////////
/// "Truck" es un agente autonomo que se mueve por el mundo
///  gracias a sus scripts. 
///  Este script maneja los triggers por delante del camion.
//////////////////////////////

public class TruckFrontChecker : MonoBehaviour {
    
    public TruckEnt mytruck;

    

    void OnTriggerStay(Collider other)
    {
        //bool r = false;
        //bool c = false;

        if (other.tag == "TruckCollider" || other.tag == "FrontTruck")
        {
            if (other.transform.parent.tag != "Truck") return;            
            mytruck.CollidingWithTruck = true;
            Debug.Log("Tocamientos");         
            return;
        
        }
        else if (other.tag == "TrafficLight")
        {
            TrafficLightColl tlc = other.GetComponent<TrafficLightColl>();
            if (!tlc.Green && mytruck.Direction == tlc.Position.Reverse())
            {

                mytruck.myTLC = tlc;
                mytruck.CollidingWithTrafficLight = true;                         
                return;
            }
            if (tlc.Green && mytruck.Direction == tlc.Position.Reverse())
            {                
                mytruck.CollidingWithTrafficLight = false;
                mytruck.myTLC = null;
                return;
            }            
        }
        else
        {
            
        }

        

    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "TruckCollider" || other.tag == "FrontTruck")
        {            
            mytruck.CollidingWithTruck = false;
        }
        if (other.tag == "TraficLight")
        {            
            mytruck.CollidingWithTrafficLight = false;
            mytruck.myTLC = null;
        }

    }
    }