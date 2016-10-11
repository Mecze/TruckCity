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
    
    public TruckEntity mytruck;



    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TruckCollider")
        {
            if (other.transform.parent.tag != "Truck") return;
            bool b = other.transform.parent.GetComponent<TruckEntity>().ignoringCollisionsBecauseOfTurning;
            if (!b)
            {
                mytruck.Colliding = true;
                Debug.Log("Tocamientos");
            }
        }

    }
        void OnTriggerExit(Collider other)
    {
            if (other.tag == "TruckCollider")
            {
                mytruck.Colliding = false;
            }

        }
    }