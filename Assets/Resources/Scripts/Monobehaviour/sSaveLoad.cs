
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class sSaveLoad : MonoBehaviour

{
    /*
    public static Game savedGame = new Game();
    public static Profile savedProfile = new Profile();

    public static Action ReloadCallBack;
    
    #region Profile Save/Load
    public static Profile LoadProfile()
    {
        if(File.Exists(Application.persistentDataPath + "/savedprofile.gd"))
        {
            
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedprofile.gd", FileMode.Open);
            sSaveLoad.savedProfile = (Profile)bf.Deserialize(file);
            file.Close();

            return sSaveLoad.savedProfile;
            
        }
        return null;


    }
    public static void SaveProfile()
    {        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedprofile.gd");
        bf.Serialize(file, sSaveLoad.savedProfile);
        file.Close();

    }

    #endregion
    
    #region Game Save/Load
    public static void Save(bool newgame = false)
    {
        
        if (!newgame) sSaveLoad.savedGame = sGameManager.singleton.currentGame;
        BinaryFormatter bf = new BinaryFormatter();        
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
        bf.Serialize(file, sSaveLoad.savedGame);
        file.Close();
        
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            sGameManager.gameEnabled = false;
            sGameManager.gameLoaded = false;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            sSaveLoad.savedGame = (Game)bf.Deserialize(file);
            file.Close();
            sGameManager.singleton.currentGame = sSaveLoad.savedGame;
            sGameManager.gameLoaded = true;
            sGameManager.singleton.currentGame.FixLoadedGame();
            GameObject.FindObjectOfType<sGameManager>().GameStarted();
            if (ReloadCallBack != null)
            {
                ReloadCallBack();
            }
        }
    }
    public static void NewGame(int levelIndex)
    {
        sGameManager.gameEnabled = false;
        sGameManager.gameLoaded = false;

        //StartCoroutine(Wait(1f));

        DeleteSavedGame();
        Game g = new Game();
        g.GameInitialization(sLevels.singleton.gameLevels[0]);//Initialize Level 0
        g.timeFrozen = false;

        sSaveLoad.savedGame = g;

        sSaveLoad.Save(true);
        sGameManager.singleton.currentGame = savedGame;
        sGameManager.gameLoaded = true;
        GameObject.FindObjectOfType<sGameManager>().GameStarted();
        if (ReloadCallBack != null)
        {
            ReloadCallBack();
        }


    }

    #endregion

    #region Utils

    

    public static bool CheckIfSavedGame()
    {
            return File.Exists(Application.persistentDataPath + "/savedGames.gd");
    }

    public static bool DeleteSavedGame()
    {
        if (CheckIfSavedGame())
        {
            File.Delete(Application.persistentDataPath + "/savedGames.gd");
            return true; //Borrado con exito
        }
        else
        {
            return false;
        }
    }

    
    

    IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

    }
    #endregion
    */
}
