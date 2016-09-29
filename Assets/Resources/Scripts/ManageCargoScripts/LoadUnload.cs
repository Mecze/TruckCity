using UnityEngine;
using System.Collections;
using System.Linq;
using System;

//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// Esto es un script Satelite. Se encarga
/// de detectar si el camion (la carga del camion)
/// Est� dentro. Si es as� llama a CargoBuilding para
/// que se dispare el evento correspondiente.
//////////////////////////////

public class LoadUnload : MonoBehaviour {
    /// <summary>
    /// This is MY Building, assigned by Inspector
    /// </summary>
    [SerializeField]
    CargoBuilding myBuilding;
    [SerializeField]
    CardinalPoint cardinalPoint;
   


    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Cargo") return;
        Cargo cargo = other.GetComponent<Cargo>();
        myBuilding.TruckOnStation(cardinalPoint, cargo);    
    }
   



}