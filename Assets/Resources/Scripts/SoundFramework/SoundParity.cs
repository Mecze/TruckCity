using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoundParity
{
    public AudioClip clip;
    public string Alias;
    
    public SoundParity()
    {
        clip = null;
        Alias = "none";
    }
    public SoundParity(AudioClip newclip)
    {
        clip = newclip;
        Alias = newclip.name;
    }

}