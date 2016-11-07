using UnityEngine;
using System.Collections;

public class TutorialPanel : MonoBehaviour {
    [SerializeField]
    public GameObject PressToContinue;
    [SerializeField]
    public GameObject[] TutorialArray;

    [SerializeField]
    public bool ExtraJustOnce = true;
    [SerializeField]
    public GameObject[] TutorialExtraArray;

    public void AnimationFinished()
    {
        TutorialController.s.SetReadyToClick();
    }

}
