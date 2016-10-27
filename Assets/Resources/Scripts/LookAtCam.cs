using UnityEngine;
using System.Collections;

public class LookAtCam : MonoBehaviour {

    [SerializeField]
    Transform myCamera;


	void OnEnable()
    {
        if (myCamera == null)
        {
            LookAtMainCamera();
        }else
        {
            LookAtCamera(myCamera);
        }
    }

    public void LookAtMainCamera()
    {
        LookAtCamera(Camera.main.transform);
    }

    public void LookAtCamera(Transform Cam)
    {
        Vector3 newRotation = Cam.rotation.eulerAngles;

        this.transform.eulerAngles = newRotation;

       // this.transform.rotation = quat;
    }

}
