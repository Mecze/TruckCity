using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// ProfileManager [DONTDESTROYONLOAD] Persistente
//////////////////////////////
/// Este script carga o crea nuevo perfil, e inicializa el juego
//////////////////////////////

public class sProfileManager : Singleton<sProfileManager> {
    

  
    [SerializeField]
    bool ForceNewProfile;
    [SerializeField]
    bool DebugVersion = false;
    [SerializeField]
    int targetFrameRateHigh = 60;
    [SerializeField]
    int targetFrameRateLow = 30;
    [SerializeField]
    int RecommendedVideoMemory = 1024;

    static Profile _singleton = null;
    public static Profile ProfileSingleton
    {
        get
        {
            if (_singleton == null)
            {
                //If we have no profile:

                //if there's no instance of the Manager we exit to avoid errors
                if (sProfileManager.instance == null) return null;
                
                //If we are forced to create a new profile (usually used for debuging and testing)
                if (sProfileManager.instance.ForceNewProfile)
                {
                    _singleton = NewProfile();
                }
                else
                {
                    //if we are allowed to load:
                    Profile pf = sSaveLoad.LoadProfile();

                    
                    

                    //Now we compare loaded profile versions with current versi�n (default's profile build numbers)
                    if (pf == null || pf.buildNumber == 0 || pf.buildNumber != sProfileManager.instance.defaultProfile.buildNumber)
                    {
                        
                         

                        _singleton = NewProfile(); //we create a new profile (a copy of Default's profile)

                        //below buildNumber 6 there are no CODENAMES, we will not try to adapt
                        

                        //if new version (default's) is greater than loaded version and we are not forced to clean player progression we proceed to copy it from old profile
                        if (pf != null)
                        {
                            if (pf.buildNumber < 6) { return _singleton; }

                            if (sProfileManager.instance.defaultProfile.buildNumber > pf.buildNumber && !_singleton.forceNewProfileOnUpdate)
                            {
                                //Newer versi�n detected!
                                //we try to copy older ProfileLevels and stars of the player (Progresion)
                                for (int i = 0; i < pf.profileLevels.Count; i++)
                                {
                                    //we do it one by one, on a for to respect newer levels
                                    string code = _singleton.profileLevels[i].code;
                                    ProfileLevels TempPL = pf.profileLevels.Find(x => x.code == code);
                                    if (TempPL != null) _singleton.profileLevels[i] = TempPL;
                                }
                                Debug.Log("Profile adapted");
                            }
                        }
                    }
                    else
                    {
                        //If the version Build number Match, we just load
                        _singleton = pf;
                        sSaveLoad.savedProfile = _singleton;                        
                        Debug.Log("Profile Loaded");
                    }
                }
            }
            return _singleton;


        }
        set
        {
            _singleton = value;
        }
    }



    public Profile defaultProfile;

    public List<LevelConditions> levelconditions;

    public List<AspectRatioOptions> ResolutionDictionary;


    public static Profile NewProfile(bool deleteOld = false)
    {
        if (deleteOld) sSaveLoad.DeleteSavedGame();
        Debug.Log("NEW PROFILE!");
        
        Profile PL = new Profile();
        PL = ObjectCloner.Clone<Profile>(sProfileManager.instance.defaultProfile);
        sSaveLoad.savedProfile = PL;
        sSaveLoad.savedProfile.LanguageSelected = ChooseLanguage();
        sSaveLoad.SaveProfile();
        //sSaveLoad.savedProfile.GlobalGraphicQualitySettings = CheckSystem(out sSaveLoad.savedProfile.GraphicMemory);
        return PL;
    }

    static GraphicQualitySettings CheckSystem(out float graphicMemory)
    {
        float mem = SystemInfo.graphicsMemorySize;
        graphicMemory = mem;
        Debug.Log(mem);
        if (mem >= sProfileManager.s.RecommendedVideoMemory) {            
            return GraphicQualitySettings.High;
        }else
        {
            return GraphicQualitySettings.Low;
        }
    }

    static string ChooseLanguage()
    {
        string s = "English";
        switch (Application.systemLanguage)
        {           
            case SystemLanguage.Spanish:
                s = "Spanish";
                break;           
            default:
                s = "English";
                break;
        }
        return s;
    }


    void Start()
    {

        //Esto solo sera ejecutado cuando se crea este objeto.
        //Gracias a que hereda de Singleton<> y "Is Peristent" es true

        //DEVELOPER!
        //sSaveLoad.DeleteSavedGame(); //
        //---DEVELOPER

        Debug.Log(Application.persistentDataPath.ToString());
        //Inicializa el Perfil   (Se hace en el GET)
#pragma warning disable 0219
        Profile pf = sProfileManager.ProfileSingleton;
#pragma warning restore 0219
        //pf no vale para nada realmente! (Por ahora)

        //Comprueba si hay una partida guardada... estabamos jugando
        //pf.currentlyPlayingLevel = sSaveLoad.CheckIfSavedGame();



        ChoseResolution();
        InitializeGame();

        //if (GameConfig.s.MusicState) MusicStore.s.PlayMusicByAlias("Menu", 1.5f, GameConfig.s.MusicVolume, true, 5f);




    }

    void ChoseResolution() {
        //Get current screen resolution
        Resolution myRes = Screen.currentResolution;
        //we calculate Aspect Ratio coeficient:
        float myAR;
#if UNITY_EDITOR
        Vector2 size = GetMainGameViewSize();
        myAR = size.x / size.y;
#else

        myAR = (float)myRes.width / (float)myRes.height;
#endif
        


        //We find our Aspect Ratio "Option"!
        AspectRatioOptions myARO = null;
        myARO = ResolutionDictionary.Find(x => Mathf.Approximately(x.aspectRatio, myAR));
        
        if (myARO == null)
        {
            //if not found, we save the default AspectRatio as current
            //and return;    
            GameConfig.s.currentAspectRatio = ResolutionDictionary[0];
            return;
        }
        else {
            //We save the AspectRatio we are playing with on the GameConfig Instance
            //and continue
            GameConfig.s.currentAspectRatio = myARO;
        }


        //If the current resolution exceeds the maximum resolution of out Option
        //we change resolution
        if (myRes.height > myARO.height)
        {
            Screen.SetResolution(myARO.width, myARO.height, true, myARO.refreshRate);
        }


    }

    public static Vector2 GetMainGameViewSize()
    {
#if UNITY_EDITOR
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
        return (Vector2)Res;
#endif
        return Vector2.one;
    }

    public void InitializeGame(bool InstantTransition=false)
    {
        Profile pf = sProfileManager.ProfileSingleton;
        Localization.language = pf.LanguageSelected;

        switch (sProfileManager.ProfileSingleton.GlobalGraphicQualitySettings)
        {
            case GraphicQualitySettings.Low:
                Application.targetFrameRate = targetFrameRateLow;
                break;
            case GraphicQualitySettings.Medium:
                Application.targetFrameRate = targetFrameRateHigh;
                break;
            case GraphicQualitySettings.High:
                Application.targetFrameRate = targetFrameRateHigh;
                break;
            default:
                Application.targetFrameRate = targetFrameRateLow;
                break;
        }

        //pplication.targetFrameRate = 5;
        QualitySettings.SetQualityLevel((int)_singleton.GlobalGraphicQualitySettings);
        GameConfig.s.MusicState = pf.MusicState;
        GameConfig.s.SoundState = pf.SoundState;
        if (!DebugVersion)
        {
            if (InstantTransition)
            {
                LoadingScreenManager.LoadScene(1);
            }
            else
            {
                StartCoroutine(ChangeScene(1)); //Vamos al menu
            }
            

        }
    }


    IEnumerator ChangeScene(int i)
    {
        yield return new WaitForSeconds(3f);
        //        SceneManager.LoadScene(i);
        LoadingScreenManager.LoadScene(i);
    }
/*
    public void ChangeLevel(int levelIndex)
    {
        SoundSystem.s.FadeOutMusic(0.5f, () =>
         {
             LevelConditions LC = levelconditions.Find(x => x.BuildSettingOrder == levelIndex);
             if (LC != null) {
                 ProfileLevels PL = ProfileSingleton.profileLevels.Find(x => x.code == LC.Code);
                 if (PL != null)
                 {
                     sProfileManager.ProfileSingleton.newLevelIndex = PL.index;

                 }
            }
             sProfileManager.ProfileSingleton.ChangingLevel = true;
             LoadingScreenManager.LoadScene(levelIndex + 3);
         });
    }
    */
    public void ChangeLevel(string code)
    {
        int levelIndex = 0;
        levelIndex = levelconditions.Find(x => x.Code == code).BuildSettingOrder;
        SoundSystem.s.FadeOutMusic(0.5f, () =>
        {

            sProfileManager.ProfileSingleton.newLevelIndex = ProfileSingleton.profileLevels.Find(x => x.code == code).index;
            sProfileManager.ProfileSingleton.ChangingLevel = true;
            LoadingScreenManager.LoadScene(levelIndex);
        });
    }


    public bool IsNextLevelUnlocked(int thisLevel)
    {
        if (!ProfileSingleton.profileLevels.Exists(x => x.index == thisLevel + 1)) return false;
        if (ProfileSingleton.profileLevels[thisLevel + 1] == null) return false;
        if (ProfileSingleton.profileLevels[thisLevel + 1].starsToUnlock <= ProfileSingleton.stars)
        {
            return true;
        }
        return false;
    }
    


}







[System.Serializable]
public class AspectRatioOptions
{
    /*
    public const float AspectRatio1609 = 1.7777f;
    public const float AspectRatio1610 = 1.6f;
    public const float AspectRatio0403 = 1.3333f;

    public const int max1609height = 1080;
    public const int max1610height = 1200;
    public const int max0403 = 1200;
    */
    
    //Used to differentiate them (developer, serialized onto the inspector)
    public string ResolutionName;
    /// <summary>
    /// Max Height of this AspectRatio
    /// </summary>
    public int height;
    /// <summary>
    /// Max Width of this AspectRatio
    /// </summary>
    public int width; 
    /// <summary>
    /// Refresh Rate, usually 60
    /// </summary>
    public int refreshRate;

#region Constructor    

    public AspectRatioOptions(int width=1920,int height=1080, int RefresRate=60)
    {
        this.height = height;
        this.width = width;
        this.refreshRate = RefresRate;
    }

#endregion

#region TO Resolution
    public Resolution resolution
    {
        get
        {
            Resolution res = new Resolution();
            res.height = height;
            res.width = width;
            res.refreshRate = refreshRate;
            return res;
        }
        set
        {
            height = value.height;
            width = value.width;
            refreshRate = value.refreshRate;
        }
    }
#endregion

#region AspectRatio Transformation
    public float aspectRatio
    {
        get
        {
            return (float)width / (float)height;
        }
    }
#endregion



}





