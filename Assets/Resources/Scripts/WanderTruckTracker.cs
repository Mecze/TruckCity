using UnityEngine;
using System.Collections;

public class WanderTruckTracker : MonoBehaviour {
#pragma warning disable 0649
    [SerializeField]
    Wander myAIWander;
#pragma warning restore 0649


    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Truck") myAIWander.nearbyTruck.Add(col.transform);
    }
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Truck") myAIWander.nearbyTruck.Remove(col.transform);
    }
}