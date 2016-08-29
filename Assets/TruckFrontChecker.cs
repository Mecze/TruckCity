using UnityEngine;
using System.Collections;

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