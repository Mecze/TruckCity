using UnityEngine;
using System.Collections;

public class RoadBlock : MonoBehaviour {



    void OnTriggerEnter(Collider other)
    {

        if (other.tag != "Truck") return;
        TruckEntity te = other.GetComponent<TruckEntity>();
        te.ChangeDirection(RoadEntity.ReverseDirection(te.direction), Turn.Reverse);


    }


}
