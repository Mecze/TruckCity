using UnityEngine;
using System.Collections.Generic;

public class resetTypewriter : MonoBehaviour {

    List<GameObject> myLabelObjects;


    List<TypewriterEffect> lastTypeWriter;
    bool listSet = false;

    void OnEnable()
    {
        if (!listSet) ReferenceList();
    }

    void ReferenceList()
    {
        if (lastTypeWriter == null) lastTypeWriter = new List<TypewriterEffect>();
        if (myLabelObjects == null) myLabelObjects = new List<GameObject>();
        myLabelObjects.Clear();
        UILabel[] labels = GetComponentsInChildren<UILabel>();
        for (int i = 0; i < labels.Length; i++)
        {
            myLabelObjects.Add(labels[i].gameObject);
        }
        listSet = true;
    }

    public void Reset(TypewriterEffect ResetLike = null)
    {
        if (!listSet) ReferenceList();
        lastTypeWriter.Clear();
        if (myLabelObjects.Count > 0)
        {
            for (int i = 0; i < myLabelObjects.Count; i++)
            {
                lastTypeWriter.Add(myLabelObjects[i].AddComponent<TypewriterEffect>());                
                if (ResetLike != null)
                {
                    lastTypeWriter[i].initialDelay = ResetLike.initialDelay;
                    lastTypeWriter[i].charsPerSecond = ResetLike.charsPerSecond;
                    lastTypeWriter[i].delayOnNewLine = ResetLike.delayOnNewLine;
                    lastTypeWriter[i].delayOnPeriod = ResetLike.delayOnPeriod;
                    lastTypeWriter[i].fadeInTime = ResetLike.fadeInTime;
                    lastTypeWriter[i].keepFullDimensions = ResetLike.keepFullDimensions;
                    lastTypeWriter[i].onFinished = ResetLike.onFinished;
                    
                }
            }
        }
    }
    public void DisableTypeWriter()
    {
        //if (lastTypeWriter != null) lastTypeWriter.enabled = false;
        for (int i = 0; i < lastTypeWriter.Count; i++)
        {
            if (lastTypeWriter[i] != null) Destroy(lastTypeWriter[i]);
        }
        lastTypeWriter.Clear();
    }

    public void finishTypeWritter()
    {
        for (int i = 0; i < lastTypeWriter.Count; i++)
        {
            if (lastTypeWriter[i] != null) lastTypeWriter[i].Finish();
        }
        
        
    }

}
