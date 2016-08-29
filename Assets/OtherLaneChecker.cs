using UnityEngine;
using System.Collections;

public class OtherLaneChecker : MonoBehaviour {
    [SerializeField]
    TruckEntity myTruckEntity;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TruckCollider")
        {
            if (other.transform.parent.tag != "Truck") return;
            if (other.transform.parent.GetComponent<TruckEntity>() == myTruckEntity) return;
            myTruckEntity.otherLaneTrucks++;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "TruckCollider")
        {
            if (other.transform.parent.tag != "Truck") return;
            if (other.transform.parent.GetComponent<TruckEntity>() == myTruckEntity) return;
            myTruckEntity.otherLaneTrucks--;
            if (myTruckEntity.otherLaneTrucks < 0) myTruckEntity.otherLaneTrucks = 0;
        }
    }


}