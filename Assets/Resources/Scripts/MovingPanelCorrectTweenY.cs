using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TweenPosition))]
public class MovingPanelCorrectTweenY : MonoBehaviour {
    

    void Awake()
    {
        Adapt();
    }

    public void Adapt()
    {
        TweenPosition TPos = GetComponent<TweenPosition>();
        if (TPos == null) return;

        TPos.to.y = transform.localPosition.y;
        TPos.from.y = transform.localPosition.y;


    }


}
