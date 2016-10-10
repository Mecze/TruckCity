using UnityEngine;
using System.Collections;

public class PlayAliasSound : MonoBehaviour {
    [Header("Alias To Play")]
    [SerializeField]
    string Alias;
    [Header("Optional Settings")]
    [SerializeField]
    PlaySettings settings;

    /// <summary>
    /// Use this as Listener
    /// </summary>
    public void Play()
    {

        SoundStore.s.PlaySoundByAlias(Alias, settings.Delay, settings.Volume, settings.FadeIn.Enabled, settings.FadeIn.Time, settings.FadeOut.Enabled, settings.FadeOut.Time);

    }
	


}
[System.Serializable]
public class PlaySettings
{
    public float Delay = 0f;
    public float Volume = 1f;
    public FadeSettings FadeIn;
    public FadeSettings FadeOut;

}
[System.Serializable]
public class FadeSettings
{
    public bool Enabled = false;
    public float Time = 0.1f;
}
