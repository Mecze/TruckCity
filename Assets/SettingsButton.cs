using UnityEngine;
using System.Collections;

public class SettingsButton : MonoBehaviour {
    bool Clickable = true;
    bool state = false;

    [SerializeField]
    TweenPosition ConfigPanelTween;

    






    public void Click()
    {
        if (!Clickable) return;
        if (state) {
            Clickable = false;
            state = false;
            ConfigPanelTween.PlayReverse();
        }else
        {
            state = true;
            Clickable = false;
            ConfigPanelTween.PlayForward();
            
        }

    }


    public void FinishPlaying()
    {
        Clickable = true;
    }



}
