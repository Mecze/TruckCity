using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class sMenuButton : MonoBehaviour {
    public GameObject tick;
    public Text buttonText;
    public Button button;
    bool parented = false;
    public int levelIndex;
    [SerializeField]
    GameObject StarAnchor;
    [SerializeField]
    GameObject StarPrefab;
    [SerializeField]
    Sprite StarIMG;
    [SerializeField]
    GameObject StarsToUnlockPanel;
    [SerializeField]
    Text StarstoUnlockText;

    public ProfileLevels myProfileLevel;
    List<GameObject> myStars;

    
    public GameObject UnlockPanel;    
    public Animator UnlockAnimator;
        



    public void Set(ProfileLevels PL)
    {
        myProfileLevel = PL;
        UpdateGUI();
    }
    public void UpdateGUI() { 
        if (!myProfileLevel.locked)
        {

            buttonText.text = "Level " + (myProfileLevel.index + 1).ToString();
            tick.SetActive(myProfileLevel.beated);
            levelIndex = myProfileLevel.index;

            if (myStars == null) myStars = new List<GameObject>();
            foreach (GameObject go in myStars) DestroyImmediate(go);
            myStars.Clear();

            for (int i = 1; i <= myProfileLevel.stars; i++)
            {
                GameObject go = GameObject.Instantiate(StarPrefab);
                go.transform.SetParent(StarAnchor.transform);                
                go.GetComponent<Image>().sprite = StarIMG;
                //go.transform.localPosition = Vector3.one;
                myStars.Add(go);
            }
            int e = myProfileLevel.maxStars - myProfileLevel.stars;
            for (int i = 1; i <= e; i++)
            {
                GameObject go = GameObject.Instantiate(StarPrefab);
                go.transform.SetParent(StarAnchor.transform);
                //go.transform.localPosition = Vector3.one;
                myStars.Add(go);
            }
            foreach (GameObject gostar in myStars)
            {
                if (gostar.transform.localScale != Vector3.one)
                {
                    gostar.transform.localScale = Vector3.one;
                }

            }

            StarsToUnlockPanel.SetActive(false);


        }
        else
        {
            levelIndex = myProfileLevel.index;
            buttonText.text = "Locked";
            StarsToUnlockPanel.SetActive(true);
            int i = myProfileLevel.starsToUnlock - sProfileManager.ProfileSingleton.stars;
            if (i < 0) i = 0;
            StarstoUnlockText.text = i.ToString() + " more";
            tick.SetActive(false);
        }
        button.interactable = !myProfileLevel.locked;
    }

    void Update()
    {
        CheckParented();
    }

    void CheckParented()
    {
        if (parented == false)
        {
            transform.SetParent(GameObject.Find("ButtonAnchor").transform);
            transform.localScale = Vector3.one;
            if (transform.parent) parented = true;
        }

    }
    public void UnlockThisLevelAnim()
    {
        UnlockPanel.SetActive(true);
        UnlockAnimator.SetBool("Start", true);
    }
    public void OnClick()
    {        
        sMenu.singleton.OnLevelButtonClick(levelIndex);
    }
	
}