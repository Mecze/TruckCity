using UnityEngine;
using System.Collections;

public class GasStation : MonoBehaviour {

	

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Truck")
        {
            other.GetComponent<TruckEntity>().RefilGas();
            GameController.s.FloatingTextSpawn(this.transform, "Refill", enumColor.Green, "Gas_Refill",Color.black);
        }

    }


}