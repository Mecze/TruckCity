using UnityEngine;
using System.Collections.Generic;

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
    /// If True, will create a new game when the Main Scene Loads (SET newLevelIndex!)
    /// </summary>
    public bool startNewLevel;

    /// <summary>
    /// Points to the level to create new
    /// </summary>
    public int newLevelIndex;


    public string version;



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



}