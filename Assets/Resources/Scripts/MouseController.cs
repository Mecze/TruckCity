using UnityEngine;
using System.Collections;

public delegate void MouseClickEventHandler(GameObject go);

public class MouseController : MonoBehaviour {

    public static event MouseClickEventHandler OnClick;


    void Update()
    {
        //Lanzarrayos!
        if (Input.GetMouseButtonDown(0))
        {
            //Solo en la Layer "Clickable", para objetos en esa capa
            int layer_mask = LayerMask.GetMask("Clickable");

            //Rayo
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Para el "out"
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 100, layer_mask))
            {//Si golpeamos algo en esa capa lanzamos "OnClick"
                if (OnClick != null) OnClick(hit.transform.gameObject);
            }


        }
    }

	
}