using UnityEngine;
using System.Collections;

public enum SoundType { Music= 0,Sound=1,MuffledSound =2}

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
        float volume=0f;
        switch (settings.soundType)
        {
            case SoundType.Music:
                volume = GameConfig.s.MusicVolume;
                break;
            case SoundType.Sound:
                volume = GameConfig.s.SoundVolume;
                break;
            case SoundType.MuffledSound:
                volume = GameConfig.s.MuffledSoundVolume;
                break;
            default:
                break;
        }


        SoundStore.s.PlaySoundByAlias(Alias, settings.Delay, volume, settings.FadeIn.Enabled, settings.FadeIn.Time, settings.FadeOut.Enabled, settings.FadeOut.Time);

    }
	


}
[System.Serializable]
public class PlaySettings
{
    public float Delay = 0f;
    public SoundType soundType = SoundType.Sound;
    public FadeSettings FadeIn;
    public FadeSettings FadeOut;

}
[System.Serializable]
public class FadeSettings
{
    public bool Enabled = false;
    public float Time = 0.1f;
}
