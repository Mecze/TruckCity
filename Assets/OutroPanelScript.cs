using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class OutroPanelScript : MonoBehaviour {

    [SerializeField]
	TweenScale myScale;
    [SerializeField]
    UILabel Title;
#pragma warning disable 0414
    int Stars;
    [SerializeField]
    List<GameObject> starGOs;
    [SerializeField]
    List<TweenScale> starTweens;
    [Header("Anim waitTimes")]
    [SerializeField]
    float beforeStars = 1f;
    [SerializeField]
    float betweenStars = 0.3f;

    public void ShowPanel(int levelIndex, int stars)
    {
        

        
        Stars = stars;
        Title.text = "Level " + (levelIndex+1).ToString();
        myScale.PlayForward();
        StartCoroutine(StarAnimations(stars));
#pragma warning restore 0414

    }

    IEnumerator StarAnimations(int stars)
    {
        yield return new WaitForSeconds(beforeStars);
        for (int i = 0; i < starGOs.Count; i++)
        {
            int num = i + 1;
            if (num <= stars)
            {
                starGOs[i].SetActive(true);
                starTweens[i].PlayForward();
                yield return new WaitForSeconds(starTweens[i].duration + betweenStars);
            }
            else
            {
                starGOs[i].SetActive(false);
            }
            
        }
    }
}
