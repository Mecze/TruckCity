using UnityEngine;
using System.Collections;
using PicaVoxel;

public class TriggerInvisible : MonoBehaviour {
    [SerializeField]
    Volume myVolume;
    


    int _numberOfTrucks;

    public int numberOfTrucks
    {
        get
        {
            return _numberOfTrucks;
        }

        set
        {
            _numberOfTrucks = value;
            if (_numberOfTrucks < 0) _numberOfTrucks = 0;
            ChangeVolumeMaterial(_numberOfTrucks);

        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Truck")
        {
            numberOfTrucks++;
        }


    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Truck")
        {
            numberOfTrucks--;
        }
    }


    void ChangeVolumeMaterial(int trucks)
    {
        if (trucks == 0)
        {
            myVolume.SetFrame(0);
        }else
        {
            myVolume.SetFrame(1);
        }


    }

	



}