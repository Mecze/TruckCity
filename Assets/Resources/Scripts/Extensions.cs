using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ExtensionsToThings
{
    public static int RandomIndex<T>(this List<T> myList, List<int> NonRepeat = null)
    {

        if (myList.Count == 1) return 0;

        int candidate = UnityEngine.Random.Range(0, myList.Count);

        if (NonRepeat == null)
        {
            return candidate;
        }else
        {

            if (NonRepeat.Count >= myList.Count) return candidate;
            if (NonRepeat.Count == 0) return candidate;

            bool fail = false;
            
            do {
                fail = false;
                candidate = UnityEngine.Random.Range(0, myList.Count);            
                foreach(int i in NonRepeat)
                {
                    if (i == candidate) fail = true;
                }
            } while (fail);
            return candidate;

        }

    }
    public static int GetClosestIndex(this List<float> doublelist, float targetvalue)
    {
        return doublelist.IndexOf(doublelist.OrderBy(d => Mathf.Abs(d - targetvalue)).ElementAt(0));
    }
    /// <summary>
    /// Add (or substract) an amount to this number.
    /// The result will be kept between or equal to the boundaries (min & max) values
    /// the result will loop around the boundaries in case it's too big, and keep substracting from the top (or bottom) of the boundary
    /// example: 4+2 in a 1 to 4 boundary will be 2 (loops once). 18-11 in a 15 to 20 boundary will be 19 (it loops twice)
    /// </summary>
    /// <param name="thisInt"></param>
    /// <param name="amount"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int AddwithBoundaries(this int thisInt, int amount, int min, int max)
    {
        bool done = false;
        int candidate;
        candidate = thisInt + amount;
        while (!done)
        {
            if (candidate > max)
            {
                int delta = (candidate - max) - 1;
                candidate = min + delta;
            }
            if (candidate < min)
            {
                int delta = (min - candidate) - 1;
                candidate = max - delta;
            }
            if (candidate >= min && candidate <= max) done = true;
        }
        return candidate;

    }

}
