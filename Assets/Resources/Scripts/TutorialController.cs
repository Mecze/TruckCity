using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour {
    #region Singleton
    private static TutorialController s_singleton = null;

    public static TutorialController singleton
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<TutorialController>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }
    public static TutorialController s
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<TutorialController>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }




    #endregion
    [SerializeField]
    public bool hasTutorial = false;
    [SerializeField]
    GameObject GUIPanel;
    [SerializeField]
    GameObject IntroPanel;
    [SerializeField]
    GameObject TutorialOverlayPanel;
    [SerializeField]
    GameObject SkipTutorialButton;
    [SerializeField]
    GameObject RepeatTutorialButton;
    bool readyToClick = false;
    private bool _secondTime = false;
    public bool secondTime
    {
        get
        {
            return _secondTime;
        }

        set
        {
            _secondTime = value;
            if (secondTime) ShowExtras();
        }
    }
    int TutoIndex = 0;
    [SerializeField]
    TypewriterEffect TypewriterEffectTemplate;
    resetTypewriter CurrentTypeWritter;

    #region custom properties
    //All these custom properties tries to find the Tutorial panel
    //And tutorial GameObject arrays which may be different each level

    [SerializeField]
    GameObject _staticFloatingTextPanel;




    GameObject StaticFloatingTextPanel
    {
        get
        {
            if (_staticFloatingTextPanel == null) _staticFloatingTextPanel = GameObject.FindGameObjectWithTag("TutorialPanel");
            return _staticFloatingTextPanel;
        }
    }


    TutorialPanel _tutorialPanel;
    TutorialPanel TutorialPanel
    {
        get
        {
            if (_tutorialPanel == null) _tutorialPanel = StaticFloatingTextPanel.GetComponent<TutorialPanel>();
            return _tutorialPanel;
        }
    }

    GameObject[] _tutorialArray;
    GameObject[] TutorialArray
    {
        get
        {
            if (_tutorialArray == null) _tutorialArray = TutorialPanel.TutorialArray;
            return _tutorialArray;
        }
    }

    GameObject[] _tutorialExtra;
    GameObject[] TutorialExtra
    {
        get
        {
            if (_tutorialExtra == null) _tutorialExtra = TutorialPanel.TutorialExtraArray;
            return _tutorialExtra;
        }
    }

    

    GameObject _pressToContinue;
    public GameObject PressToContinue
    {
        get
        {
            if (_pressToContinue == null) _pressToContinue = TutorialPanel.PressToContinue;
            return _pressToContinue;
        }       
    }
    public bool ExtraJustOnce
    {
        get
        {
            return TutorialPanel.ExtraJustOnce;
        }
        
    }

    

    #endregion





    public void Awake()
    {
        EventDelegate ed = new EventDelegate();
        ed.Set(this, "SetReadyToClick");
        TypewriterEffectTemplate.onFinished.Add(ed);
        _staticFloatingTextPanel = GameObject.FindGameObjectWithTag("TutorialPanel");
        ResetTutorial();
        if (secondTime) ShowExtras();
    }

    #region Start/Reset Tutorial
    /// <summary>
    /// Reset the tutorial
    /// </summary>
    /// <param name="StartTutorial"></param>
    void ResetTutorial()        
    {
        TutoIndex = 0;
        readyToClick = false;
        
        SkipTutorialButton.SetActive(false);
        RepeatTutorialButton.SetActive(true);
        

        for (int i = 0; i < TutorialArray.Length; i++)
        {
            TutorialArray[i].SetActive(false);
        }

        
        if ((!secondTime) || (secondTime && !ExtraJustOnce))
        {
            for (int i = 0; i < TutorialExtra.Length; i++)
            {
                if (TutorialExtra[i] != null) TutorialExtra[i].SetActive(false);
            }
        }
        
        
    }
    /// <summary>
    /// Reset and Go the tutorial (public versi�n for GameController
    /// </summary>
    public void ResetTutorialAndGo()
    {
        ResetTutorial();
        StartTutorial();
    }

    /// <summary>
    /// This starts the tutorial    
    /// </summary>
    public void StartTutorial()
    {
        //starting the 0 index of TutorialArray should start the process
        //(it will display the first tween
        hasTutorial = true;
        TutorialArray[0].SetActive(true);
        if (TutorialArray[0].GetComponent<resetTypewriter>() == null) TutorialArray[0].AddComponent<resetTypewriter>();
        CurrentTypeWritter = TutorialArray[0].GetComponent<resetTypewriter>();
        CurrentTypeWritter.Reset(TypewriterEffectTemplate);
        //if (secondTime) TutorialArray[0].GetComponentInChildren<TypewriterEffect>().ResetToBeginning();

        if (TutorialExtra.Length > 0)if (TutorialExtra[0] != null) TutorialExtra[0].SetActive(true);
        TutoIndex += 1;
        SkipTutorialButton.SetActive(false);
        RepeatTutorialButton.SetActive(false);

        SetPressToContinue();

    }

    #endregion

    #region ChangeState

    /// <summary>
    /// Changes the State between "PausePanel" and "TutorialPanel"
    /// </summary>
    /// <param name="b">false=goes to TutorialPanel, true=goes to PausePanel</param>
    public void ChangeState(bool b)
    {
        //bool b = StaticFloatingTextPanel.activeSelf;
        GUIPanel.SetActive(b);
        IntroPanel.SetActive(b);
        TutorialOverlayPanel.SetActive(!b);
        StaticFloatingTextPanel.SetActive(!b);
    }
    /// <summary>
    /// Static version of ChangeState
    /// </summary>
    /// <param name="b"></param>
    public static void ChangeMyState(bool b)
    {
        GameObject.FindObjectOfType<TutorialController>().ChangeState(b);

    }
    #endregion

    #region Click Behaviour

    public void SetReadyToClick()
    {
        readyToClick = true;
        SetPressToContinue();
    }


    /// <summary>
    /// MainClick behaviour
    /// </summary>
    public void Click()
    {
        //These will only happen when tutorialPanel is OFF.. (From the ? button)
        if (!StaticFloatingTextPanel.activeSelf)
        {
            //we go to tutorialPanel, and then return
            
            ChangeState(false);
            secondTime = true;
            ResetTutorial();
            //StartTutorial();
            return;
        }

        //These is the sequence for the tutorial


        if (!hasTutorial)
        { //when tutorial sequence finishes, goes to Pause Menu            
            if (!readyToClick && !secondTime) return;
            if (CurrentTypeWritter != null) CurrentTypeWritter.DisableTypeWriter();
            bool b = StaticFloatingTextPanel.activeSelf;
            ChangeState(b);
        }
        else
        { //Tutorial sequence >

            
            

            if (secondTime)
            {
                if (!readyToClick)
                {
                    if (TutorialArray[TutoIndex - 1] != null)
                    {
                        TutorialArray[TutoIndex - 1].GetComponent<resetTypewriter>().finishTypeWritter();
                    }
                    readyToClick = true;
                    if (TutoIndex >= TutorialArray.Length)
                    {
                        FinishTutorial();
                    }
                    return;
                }else
                {
                    NextHint();
                   // if (TutoIndex >= TutorialArray.Length) FinishTutorial();
                    return;
                }

                

            }
            else
            {
                if (!readyToClick) return;
                NextHint();
                return;
            }
            
            
        }
    }

    void NextHint() {

        if (!readyToClick) return;

        if (TutoIndex >= TutorialArray.Length)
        {
            if (secondTime && readyToClick) FinishTutorial();
            readyToClick = true; return;
        }
        //we set Active the current box, and the tween will start animating
        TutorialArray[TutoIndex].SetActive(true);
        //Se resetea el TypewriterEffect
        if (TutorialArray[TutoIndex].GetComponent<resetTypewriter>() == null) TutorialArray[TutoIndex].AddComponent<resetTypewriter>();
        CurrentTypeWritter = TutorialArray[TutoIndex].GetComponent<resetTypewriter>();
        CurrentTypeWritter.Reset(TypewriterEffectTemplate);

        //we disable last box (if it exists)
        if (TutoIndex - 1 >= 0) if (TutorialArray[TutoIndex - 1] != null)
            {                
                TutorialArray[TutoIndex - 1].SetActive(false);
                TutorialArray[TutoIndex - 1].GetComponent<resetTypewriter>().DisableTypeWriter();
            }


        //After initial Text, player can Skip!
        SkipTutorialButton.SetActive(true);

        //we set active the "extra" game object (in case it exists)
        if (TutorialExtra.Length >= (TutoIndex + 1))
        {
            if (TutorialExtra[TutoIndex] != null)
            {
                TutorialExtra[TutoIndex].SetActive(true);
                TutorialExtra[TutoIndex].BroadcastMessage("Freeze", SendMessageOptions.DontRequireReceiver);
            }
        }

        //For the next iteration (next click)
        TutoIndex += 1;

        //Set this to false, player will be "Stuck" until typegraphic tween finishes
        readyToClick = false;
        SetPressToContinue();

        //This checks if all items of the array have been displayed in order to finish the tutorial
        if (!secondTime) if (TutoIndex >= TutorialArray.Length) FinishTutorial();
    }

    /// <summary>
    /// Finish the Tutorial Sequence
    /// </summary>
    void FinishTutorial()
    {
        readyToClick = true;
        hasTutorial = false;
        sProfileManager.ProfileSingleton.profileLevels[GameController.s.level].TutorialDone = true;
        GameController.s.SaveProfile(true);
        //ResetTutorial();
    }


    /// <summary>
    /// This decides if it has to show the "Press to continue" text
    /// </summary>
    void SetPressToContinue()
    {
        if (secondTime) { PressToContinue.SetActive(true); return; }
        else { if (readyToClick) { PressToContinue.SetActive(true); return; } }
        PressToContinue.SetActive(false);
    }
    /// <summary>
    /// Skips Tutorial
    /// </summary>
    public void SkipTutorial()
    {
        if (CurrentTypeWritter != null) CurrentTypeWritter.DisableTypeWriter();
        ShowExtras();
        FinishTutorial();
        Click();
    }

    /// <summary>
    /// Usually Extra GameObjects on the TutorialExtra Array
    /// needs to be enabled for the game to function correctly
    /// </summary>
    public void ShowExtras()
    {
        bool gamestarted = GameController.s.gameStarted;
        for (int i = 0; i < TutorialExtra.Length; i++)
        {
            if (TutorialExtra[i] != null)
            {
                TutorialExtra[i].SetActive(true);
                IFreezable[] freezables = TutorialExtra[i].GetComponents<IFreezable>();
                for (int e = 0; e < freezables.Length; e++)
                {
                    if (gamestarted) { freezables[e].Freeze(); } else { freezables[e].Unfreeze(); }

                }
                IFreezable[] freezableschilds = TutorialExtra[i].GetComponentsInChildren<IFreezable>();
                for (int a = 0; a < freezableschilds.Length; a++)
                {
                    if (gamestarted) { freezableschilds[a].Freeze(); } else { freezableschilds[a].Unfreeze(); }
                }
            }

        }
    }


    #endregion


}
