using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public enum TweensFrom { Mix = 0, GetComponent = 1, ListOfTweens = 2 }

public class TweenAnimationController : MonoBehaviour {
    public TweensFrom From;

    [SerializeField]
    List<UITweener> ListOfTweens;

    List<UITweener> AllTweens;

    

    void Awake()
    {
        AllTweens = new List<UITweener>();

        if (From == TweensFrom.GetComponent ||  From == TweensFrom.Mix)
        {
            List<UITweener> GC = new List<UITweener>();
            GC = GetComponents<UITweener>().ToList<UITweener>();
            GC.ForEach(x => AllTweens.Add(x));
            //AllTweens.Concat(GC);
        }
        if (From == TweensFrom.ListOfTweens || From == TweensFrom.Mix)
        {
            ListOfTweens.ForEach(x => AllTweens.Add(x));
        }

    }

    public void PlayAnimations(bool loop = false)
    {
        for (int i = 0; i < AllTweens.Count; i++)
        {
            if (loop) AllTweens[i].style = UITweener.Style.Loop;
            if (!loop) AllTweens[i].style = UITweener.Style.Once;
            AllTweens[i].PlayForward();
        }
    }
    public void StopAnimations()
    {
        float longestDuration = AllTweens.Max(x => x.duration);


        for (int i = 0; i < AllTweens.Count; i++)
        {
            AllTweens[i].style = UITweener.Style.Once;
        }
        StartCoroutine(Reset(longestDuration));
    }
    IEnumerator Reset(float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < AllTweens.Count; i++)
        {
            AllTweens[i].ResetToBeginning();
        }
    }

}
