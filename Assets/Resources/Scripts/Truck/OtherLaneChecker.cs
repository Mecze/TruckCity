using UnityEngine;
using System.Collections;

//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// Truck Entity
//////////////////////////////
/// "Truck" es un agente autonomo que se mueve por el mundo
///  gracias a sus scripts. 
///  Maneja los triggers que comprueban que hay camiones en el otro sentido.
//////////////////////////////

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