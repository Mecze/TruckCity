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
    int lastSelectedIndex;
    bool promptIsUp = false;
    [SerializeField]
    UILabel StarsText;

    [SerializeField]
    GameObject anchor;

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
        //anchor.GetComponent<UIGrid>().r

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
        sProfileManager.instance.ChangeLevel(levelIndex);
    }

    /*void Unlocks()
    {
        
        Profile profile = sProfileManager.ProfileSingleton;
        

    }
    */

}