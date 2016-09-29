using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// Esto es un script Satelite. 
/// Se encarga de cambiar el color de las Flechas.
/// CargoBuilding se encarga de llamar a SetColor.
//////////////////////////////

public class ArrowCargo : MonoBehaviour {

    [SerializeField]
    List<Image> images;



    public void SetColor(Color color)
    {
        foreach(Image img in images)
        {
            img.color = color;
        }
    }


}
