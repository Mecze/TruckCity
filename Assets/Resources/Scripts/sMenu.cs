using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class sMenu : MonoBehaviour {
    #region singleton
    static sMenu _smenu;
    public static sMenu singleton
    {
        get
        {
            if (_smenu == null)
            {
                _smenu = FindObjectOfType<sMenu>();
            }
            if (_smenu == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("NO EXITE SINGLETON!");
            }
            return _smenu;
        }
        set
        {
            _smenu = value;
        }
    }
    #endregion
    public GameObject buttonPrefab;
    //public GameObject currentGamePanel;
    //public GameObject surePanel;
    int lastSelectedIndex;
    bool promptIsUp = false;



    void Start()
    {
        Profile profile = sProfileManager.ProfileSingleton;

        foreach (ProfileLevels PL in profile.profileLevels)
        {
            GameObject go = (GameObject)GameObject.Instantiate(buttonPrefab);
            go.GetComponent<sMenuButton>().Set(PL.locked,PL.beated, PL.index);


        }        

    }

    public void OnDeleteClick()
    {

    }

    public void OnContinueClick()
    {

    }

    public void OnLevelButtonClick(int levelIndex)
    {
        if (promptIsUp) return; //FAILSAFE, Si hay un prompt, los botones no hacen nada

        
            NewGameLoadGame(levelIndex);
       





        
    }   


   

    public void ContinueGame()
    {
        SceneManager.LoadScene(1);
    }

    void NewGameLoadGame(int levelIndex)
    {
        sProfileManager.ProfileSingleton.newLevelIndex = levelIndex;
        sProfileManager.ProfileSingleton.startNewLevel = true;
        SceneManager.LoadScene(levelIndex+2);
    }

}