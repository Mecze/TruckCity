using UnityEngine;
using System.Collections.Generic;

public class OutroPanelScript : MonoBehaviour {

    [SerializeField]
	TweenScale myScale;
    [SerializeField]
    UILabel Title;
    int Stars;
    [SerializeField]
    List<GameObject> starGOs;

    public void ShowPanel(int levelIndex, int stars)
    {
        for (int i = 0; i < starGOs.Count; i++)
        {
            int num = i + 1;
            if (num <= stars)
            {
                starGOs[i].SetActive(true);
            }else{
                starGOs[i].SetActive(false);
            }
            //starGOs[i].SetActive(true);
        }

        
        Stars = stars;
        Title.text = "Level " + (levelIndex+1).ToString();
        myScale.PlayForward();

    }
}
