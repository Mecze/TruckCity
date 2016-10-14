using UnityEngine;
using System.Collections;

public enum enumColor { Red = 0, Green = 1, Yellow = 2, Blue = 3, Black = 4 }
public enum CargoType { None = 0, Pink = 1, Brown = 2 }

public class GameConfig : Singleton<GameConfig> {
    /*Old Singleton
    #region Singleton
    private static GameConfig s_singleton = null;


    public static GameConfig singleton
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<GameConfig>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("No Existe Singleton MapController");
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }
    public static GameConfig s
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<GameConfig>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("No Existe Singleton MapController");
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }
    #endregion
    */


    public string materialsPath;
    public string IMGPath;


    public Color[] publicColors;

    #region cargoFILE Path configuration
    [Header("Cargo: filename of the Materials Config")]
    public string cargoMaterialFileName;
    [Header("Cargo: filename of the Sprite Config")]
    public string cargoSpriteCommonFileName;
    public string[] cargoSpriteFileName;

    public Color[] cargoColors;

    public Color[] cargoTextColors;

    #endregion

    #region strings for Quest Slate
    public string[] playerOrdersQuestSlate;


    #endregion

    #region Sound Config
    [Header("Sound & Music Volumes")]
    [SerializeField]
    float _musicVolume = 1f;
    public float MusicVolume
    {
        get
        {
            if (MusicState)return _musicVolume;
            return 0f;
        }
    }

    [SerializeField]
    float _soundVolume = 0.5f;
    public float SoundVolume
    {
        get
        {
            if (SoundState)return _soundVolume;
            return 0f;
        }
    }

    [SerializeField]
    float _muffledSoundVolume = 0.3f;
    public float MuffledSoundVolume
    {
        get
        {
            if (SoundState) return _muffledSoundVolume;
            return 0f;
        }
    }

    [HideInInspector]
    private bool _musicState = true;
    public bool MusicState
    {
        get
        {
            return _musicState;
        }

        set
        {
            _musicState = value;
            if (MusicButton.s != null)MusicButton.s.Clickable = false;
            sProfileManager.ProfileSingleton.MusicState = value;
            sSaveLoad.SaveProfile();
            if (value)
            {
                if (SoundSystem.s.AudioSources[0].isPlaying == false)
                {
                    SoundSystem.s.AudioSources[0].Stop();
                }
                if (GameController.s != null)
                {
                    GameController.s.PlayLevelMusic(false);
                }else
                {
                    MusicStore.s.PlayMusicByAlias("Menu", 1.5f, GameConfig.s.MusicVolume, true,5f,true,0.1f,null,()=> { if (MusicButton.s != null) MusicButton.s.Clickable = true; });
                    //SoundSystem.s.FadeInMusic(1f, () => { if (MusicButton.s != null) MusicButton.s.Clickable = true; });
                }
                
                //SoundSystem.s.FadeInMusic(1f, () => { if (MusicButton.s != null) MusicButton.s.Clickable = true; });
                
                
            }else
            {
                SoundSystem.s.FadeOutMusic(1f, () => { if (MusicButton.s != null) MusicButton.s.Clickable = true; });
            }
        }
    }

    

    [HideInInspector]
    private bool soundState = true;
    public bool SoundState
    {
        get
        {
            return soundState;
        }

        set
        {
            soundState = value;
            sProfileManager.ProfileSingleton.SoundState = value;
            sSaveLoad.SaveProfile();
        }
    }

    public string SoundSprite;
    public string NoSoundSprite;
    public string MusicSprite;
    public string NoMusicSprite;

    




    #endregion
}