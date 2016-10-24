using UnityEngine;
//using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    //int lastSelectedIndex;
    bool promptIsUp = false;

    [SerializeField]
    UILabel StarsText;


    [SerializeField]
    GameObject anchor;

    [SerializeField]
    GameObject languageAnchor;

    [SerializeField]
    GameObject languagePrefab;

    bool Loading = false;

    void Start()
    {

        Profile profile = sProfileManager.ProfileSingleton;
        foreach (ProfileLevels PL in profile.profileLevels)
        {
            if (profile.stars >= PL.starsToUnlock && PL.locked == true)
            {
                PL.locked = false;
            }
        }


        StarsText.text = profile.stars.ToString();
        foreach (ProfileLevels PL in profile.profileLevels)
        {
            GameObject go = (GameObject)GameObject.Instantiate(buttonPrefab);
            go.GetComponent<MenuLevel>().myProfileLevel = PL;
            go.transform.SetParent(anchor.transform);
            go.transform.localScale = Vector3.one;
            

        }
        anchor.GetComponent<UIGrid>().Reposition();


        string[] flags = Localization.dictionary["Flag"];
        for (int i = 0; i < Localization.knownLanguages.Length; i++)
        {
            GameObject go = (GameObject)GameObject.Instantiate(languagePrefab);
            FlagSprite fs = go.GetComponent<FlagSprite>();
            fs.myLanguage = Localization.knownLanguages[i];
            fs.myImage = flags[i];
            if (Localization.language == Localization.knownLanguages[i])
            {
                fs.Set = true;
            }else
            {
                fs.Set = false;
            }


            go.transform.SetParent(languageAnchor.transform);
            go.transform.localScale = Vector3.one;
        }
        languageAnchor.GetComponent<UIGrid>().Reposition();
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
        if (Loading == false)
        {
            Loading = true;
            sProfileManager.instance.ChangeLevel(levelIndex);
        }
    }
       

}