using UnityEngine;
using System.Collections;

public class GasStation : MonoBehaviour {

	

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Truck")
        {
            other.GetComponent<TruckEntity>().RefilGas();
            GameController.s.FloatingTextSpawn(this.transform.position.x, this.transform.position.z, "Refill", enumColor.Green);
        }

    }


}