using UnityEngine;
using System.Collections;

public class TruckFrontChecker : MonoBehaviour {
    
    public TruckEntity mytruck;



    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Truck")
        {
            mytruck.Colliding = true;
            Debug.Log("Tocamientos");
        }

    }
        void OnTriggerExit(Collider other)
    {
            if (other.tag == "Truck")
            {
                mytruck.Colliding = false;
            }

        }
    }