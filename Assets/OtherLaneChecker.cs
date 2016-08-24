using UnityEngine;
using System.Collections;

public class OtherLaneChecker : MonoBehaviour {
    [SerializeField]
    TruckEntity myTruckEntity;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Truck")
        {
            myTruckEntity.otherLaneTrucks++;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Truck")
        {
            myTruckEntity.otherLaneTrucks--;
            if (myTruckEntity.otherLaneTrucks < 0) myTruckEntity.otherLaneTrucks = 0;
        }
    }


}