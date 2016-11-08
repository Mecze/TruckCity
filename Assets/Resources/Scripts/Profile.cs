using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Profile { 


    /// <summary>
    /// The list of current Levels
    /// </summary>
    public List<ProfileLevels> profileLevels;
    
    /// <summary>
    /// At game start, this bool will decide if load game or go to Menu
    /// </summary>
    public bool currentlyPlayingLevel;

    /// <summary>
    /// If True, we HAVE to change level after loading screen sets over screen
    /// </summary>
    public bool ChangingLevel;

    /// <summary>
    /// Points to the level to create new
    /// </summary>
    public int newLevelIndex;
    
    /// <summary>
    /// String of the Versi�n of this Build
    /// </summary>
    public string version;

    /// <summary>
    /// The build number. Newer builds should have greater numbers;
    /// </summary>
    public int buildNumber = 0;

    /// <summary>
    /// IF a build is released with this to Truth, Player's progression upon update
    /// will be deleted.
    /// </summary>
    public bool forceNewProfileOnUpdate = false;


    public int stars
    {
        get
        {
            return profileLevels.Sum(e => e.stars);                
        }
    }

    /// <summary>
    /// State of the Music
    /// </summary>
    public bool MusicState;

    /// <summary>
    /// State of the Sound
    /// </summary>
    public bool SoundState;
    
    /// <summary>
    /// Current Language Selected
    /// </summary>
    public string LanguageSelected;

    /// <summary>
    /// Current QualitySettings detected
    /// </summary>
    public GraphicQualitySettings GlobalGraphicQualitySettings;

    /// <summary>
    /// Device's graphicMemory;
    /// </summary>
    public float GraphicMemory;


}



[System.Serializable]
public class ProfileLevels
{
    /// <summary>
    /// Index of the Level. This correspond to "Levels" InGame
    /// </summary>
    public int index;

    /// <summary>
    /// If the Level is currently locked
    /// </summary>
    public bool locked;

    /// <summary>
    /// if the Level is currently beated by this profile
    /// </summary>
    public bool beated;

    /// <summary>
    /// Numero de estrellas que tiene el jugador en este nivel
    /// </summary>
    public int stars = 0;

    /// <summary>
    /// Maximo de strellas
    /// </summary>
    public int maxStars = 3;

    public int starsToUnlock = 1;

    public bool TutorialDone = false;




}