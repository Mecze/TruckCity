using UnityEngine;
using System.Collections;

public class CountDownSprite : MonoBehaviour {
    [SerializeField]
    UISprite mySprite;
    [SerializeField]
    int step = 3;
    [SerializeField]
    TweenScale myTweenScale;
    [SerializeField]
    TweenAlpha myTweenAlpha;


    public void StartCountDown()
    {
        myTweenScale.PlayForward();
        myTweenAlpha.PlayForward();
    }

    public void changeSprite()
    {
        step--;
        string s = "Countdown" + step.ToString();
        if (step == 0) s = "CountdownGO";
        
        
        if (step > -1)
        {
            myTweenAlpha.ResetToBeginning();
            myTweenScale.ResetToBeginning();
            mySprite.spriteName = s;
            myTweenScale.PlayForward();
            myTweenAlpha.PlayForward();

        }
        else
        {
            if (step == -1)
            {
                step = 3;
                mySprite.spriteName = "Countdown3";
                myTweenAlpha.ResetToBeginning();
                myTweenScale.ResetToBeginning();
                GameController.s.StartGame();
            }

        }

    }
	


}
