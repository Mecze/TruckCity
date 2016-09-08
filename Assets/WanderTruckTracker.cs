using UnityEngine;
using System.Collections;

public class WanderTruckTracker : MonoBehaviour {

    [SerializeField]
    Wander myAIWander;



    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Truck") myAIWander.nearbyTruck.Add(col.transform);
    }
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Truck") myAIWander.nearbyTruck.Remove(col.transform);
    }
}