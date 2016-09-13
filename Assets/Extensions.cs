using UnityEngine;
using System.Collections.Generic;

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


}
