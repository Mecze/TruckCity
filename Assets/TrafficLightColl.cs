using UnityEngine;
using System.Collections;

public class TrafficLightColl : MonoBehaviour {

    [SerializeField]
    public CardinalPoint Position;

    //[HideInInspector]
    public bool Green = true;
    	

    void Awake()
    {
        Green = true;
    }

}
