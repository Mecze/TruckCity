using UnityEngine;
using System.Collections;

public class RoadSprites : MonoBehaviour {

    //This script will just force recalculate sprites from RoadEntity
    //When Sprites were enables anothter way (maybe cinematic)
    void OnEnable()
    {
        GetComponentInParent<RoadEntity>().ReEnableSprites();

    }
}
