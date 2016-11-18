using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum enumColor { Red = 0, Green = 1, Yellow = 2, Blue = 3, Black = 4 }
public enum CargoType { None = 0, Pink = 1, Brown = 2, Orange = 3 }

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

    [Header("Paths")]
    public string materialsPath;
    public string IMGPath;
    public string LowIMGPath;

    [Header("Native Resolution")]
    public float NativeWidth = 1920f;
    public float NativeHeight = 1080f;


    [Header("Public Colors")]
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

    [Header("Clickable Roads Colors")]
    public Color[] clickableRoadColors;

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
            SetMusicButton(false);
            sProfileManager.ProfileSingleton.MusicState = value;
            sSaveLoad.SaveProfile();
            if (GameController.s == null) { PlayDefaultMusic(value); SetMusicButton(true); return; }
            if (GameController.s.MenuVersion || GameController.s.MusicPlaying) { PlayDefaultMusic(value); SetMusicButton(true); return; }
            SetMusicButton(true);

        }
    }

    void SetMusicButton(bool b)
    {
        if (MusicButton.s != null) MusicButton.s.Clickable = b;
    }
    void PlayDefaultMusic(bool value)
    {
        if (value)
        {
            if (SoundSystem.s.AudioSources[0].isPlaying == false)
            {
                SoundSystem.s.AudioSources[0].Stop();
            }
            if (GameController.s != null)
            {
                GameController.s.PlayLevelMusic(false);
            }
            else
            {
                MusicStore.s.PlayMusicByAlias("Menu", 1.5f, GameConfig.s.MusicVolume, true, 5f, true, 0.1f, null, () => { if (MusicButton.s != null) MusicButton.s.Clickable = true; });
                //SoundSystem.s.FadeInMusic(1f, () => { if (MusicButton.s != null) MusicButton.s.Clickable = true; });
            }

            //SoundSystem.s.FadeInMusic(1f, () => { if (MusicButton.s != null) MusicButton.s.Clickable = true; });


        }
        else
        {
            SoundSystem.s.FadeOutMusic(1f, () => { if (MusicButton.s != null) MusicButton.s.Clickable = true; });
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


    [Header("Menu Levels Font and Shadow Colors")]
    public FontShadowColors[] MenuLevelFontShadowColors;

    [Header("BETA URL configs")]
    public List<LanguageLinks> languageLinks;

    /// <summary>
    /// Gets Correct URL's for chosen language
    /// </summary>
    /// <param name="Language">(optional) the language. If empty, will chose system's current language</param>
    /// <returns>A class with correct URLs</returns>
    public LanguageLinks GetBETAURLs(string Language = "")
    {
        if (Language == "") Language = Localization.language;
        return languageLinks.Find(x => x.Language == Language);
    }
    [HideInInspector]
    public AspectRatioOptions currentAspectRatio;

    
}

[System.Serializable]
public class LanguageLinks
{
    public string Language;
    public string BugURL;
    public string SuggestionURL;
    public string SurveyURL;
}


[System.Serializable]
public class FontShadowColors
{
    public Color fontColor;
    public Color OutlineColor;
    public Color shadowColor;
    
}
