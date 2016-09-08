using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public delegate void ScoreEvent(CargoType cargo);
public delegate void MoneyEvent(int increment);
public delegate void CompeletedQuestEvent(float momentInTime);


public class GameController : MonoBehaviour {
    #region Singleton
    private static GameController s_singleton = null;

    public static GameController singleton
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<GameController>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("No Existe Singleton GameController");
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }
    public static GameController s
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<GameController>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("No Existe Singleton GameController");
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }




    #endregion
    [Header("FloatingTextConfig")]
    [SerializeField]
    GameObject textGOPrefab;
    public float defaulYFloatingText;

    [Header("Levels")]
    /// <summary>
    /// apunta al nivel actual en la lista "levelconditions"
    /// </summary>
    public int level = 0;
    /// <summary>
    /// Lista de niveles.
    /// </summary>

    public LevelConditions myLevel;
    [Header("Delivered")]
    #region delivered (score)
    [SerializeField]
    List<CargoDelivered> _cargosDelivered;
    public List<CargoDelivered> CargosDelivered
    {
        get
        {
            return _cargosDelivered;
        }

        set
        {
            _cargosDelivered = value;
        }
    }

    #endregion
    [Header("Money")]
    #region money

    [SerializeField]
    Text MoneyText;
    [SerializeField]
    private int _money = 0;
    public int money
    {
        get
        {
            return _money;
        }

        set
        {
            int i = value - _money;
            _money = value;
            if (OnMoneyGain != null) OnMoneyGain(i);
            MoneyText.text = _money.ToString();
        }
    }


    #endregion
    public static event ScoreEvent OnScore;
    public static event MoneyEvent OnMoneyGain;
    public static event CompeletedQuestEvent OnCompeletedQuest;
    [Header("GUI")]
    [SerializeField]
    GameObject IntroPanel;
    [SerializeField]
    GameObject OutroPanel;
    [SerializeField]
    GameObject GUIPanel;
    [SerializeField]
    Animator CoundownAnimator;
    [SerializeField]
    GameObject CounddownOBJ;
    [SerializeField]
    GameObject FinishText;


    [Header("Prefabs")]
    [SerializeField]
    GameObject QuestSlatePrefabDelivered;
    [SerializeField]
    GameObject QuestSlatePrefabMoney;
    [SerializeField]
    GameObject QuestSlatePrefabTimer;
    [SerializeField]
    GameObject IPQuestSlatePrefab;



    void Awake()
    {
        fillmylevel();
        
        
    }   

   

    

    #region FloatingTextThing

    /// <summary>
    /// Genera un Floating text con el texto que le pedimos en la posición indicada
    /// </summary>
    /// <param name="pos">VECTOR2 (Y equivale a Z) (se usa la Y por defecto)</param>
    /// <param name="text">texto a mostrar</param>
    /// <param name="publiccolor">enumColor= Rojo, amarillo, ....</param>
    public void FloatingTextSpawn(Vector2 pos, string text, enumColor publiccolor)
    {
        Vector3 realPos = new Vector3(pos.x, defaulYFloatingText, pos.y);
        FloatingTextSpawn(realPos, text, publiccolor);


    }
    /// <summary>
    /// Genera un Floating text con el texto que le pedimos en la posición indicada
    /// </summary>
    /// <param name="x">La X de la posición (se usa la Y por defecto)</param>
    /// <param name="z">La Z de la posición (se usa la Y por defecto</param>
    /// <param name="text">texto a mostrar</param>
    /// <param name="publiccolor">enumColor= Rojo, amarillo, ....</param>
    public void FloatingTextSpawn(float x, float z, string text, enumColor publiccolor)
    {
        Vector3 realPos = new Vector3(x, defaulYFloatingText, z);
        FloatingTextSpawn(realPos, text, publiccolor);


    }

    /// <summary>
    /// Genera un Floating text con el texto que le pedimos en la posición indicada (Y personalizada)
    /// </summary>
    /// <param name="pos">Posición, se usa la Y indicada (y NO la por defecto)</param>
    /// <param name="text">texto a mostrar</param>
    /// <param name="publiccolor">enumColor= Rojo, amarillo, ....</param>
    public void FloatingTextSpawn(Vector3 pos, string text, enumColor publiccolor)
    {
        GameObject go = (GameObject)GameObject.Instantiate(textGOPrefab,pos,Quaternion.identity);
        go.GetComponent<FloatingText>().phrase = text;
        go.GetComponent<FloatingText>().myColor = GameConfig.s.publicColors[(int)publiccolor];
        go.GetComponent<FloatingText>().WakeMeUp();

    }
    #endregion

    #region levelmanagement

    public static void OnScoreCall(CargoType cargo)
    {
        if (OnScore != null) OnScore(cargo);
    }
    public void LaunchCountdownAnimation()
    {
        IntroPanel.SetActive(false);
        GUIPanel.SetActive(true);
        CounddownOBJ.SetActive(true);
        CoundownAnimator.SetBool("Start", true);
    }


    public void StartGame()
    {

        FreezeGame(false);
        CounddownOBJ.SetActive(false);


    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(level+2);
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene(1);
    }


    void fillmylevel()
    {
        FreezeGame(true);
        IntroPanel.SetActive(true);
        GUIPanel.SetActive(true);
        MoneyText.text = _money.ToString();
        if (sProfileManager.instance != null) myLevel = ObjectCloner.Clone<LevelConditions>(sProfileManager.instance.levelconditions.Find(x => x.level == level));
        int e = 0;
        foreach (Quest q in myLevel.quests) {
            q.completed = false;
            q.LinkedQuest = new List<Quest>();
            
            if (q.LinkedQuestIndex.Count > 0)
            {                
                foreach (int i in q.LinkedQuestIndex)
                {
                    if (i < e) {
                        q.LinkedQuest.Add(myLevel.quests[i]);
                        q.LinkedQuestEnabled = true;
                    }
                }
                
            }
            e += 1;
            CreateGUISlate(q, e);
            CreateIPGUISlate(q, e, true);
        }

        GUIPanel.SetActive(false);
        IntroPanel.SetActive(true);
        configthislevel();
    }

    void CreateIPGUISlate(Quest q, int position, bool intro)
    {
        GameObject go;
        go = GameObject.Instantiate(IPQuestSlatePrefab);
        QuestSlateDeliveryIntro qs = go.GetComponent<QuestSlateDeliveryIntro>();
        qs.Intro = intro;
        qs.position = position;
        qs.MyCargoDelivered = CargosDelivered.Find(x => x.type == q.CargoType);
        qs.MyQuest = q;
        qs.AutoAnchor();
        
    }


    void CreateGUISlate(Quest q, int position)
    {
        GameObject go;
        switch (q.winCondition)
        {
            case WinCondition.Delivered:
                go = GameObject.Instantiate(QuestSlatePrefabDelivered);
                QuestSlateDelivery qs = go.GetComponent<QuestSlateDelivery>();
                qs.position = position;
                qs.MyCargoDelivered = CargosDelivered.Find(x => x.type == q.CargoType);
                qs.MyQuest = q;
                break;
            case WinCondition.Money:
                go = GameObject.Instantiate(QuestSlatePrefabMoney);
                QuestSlateDelivery qs1 = go.GetComponent<QuestSlateDelivery>();
                qs1.position = position;
                qs1.MyCargoDelivered = CargosDelivered.Find(x => x.type == q.CargoType);
                qs1.MyQuest = q;
                break;
            case WinCondition.Time:
                go = GameObject.Instantiate(QuestSlatePrefabTimer);
                QuestSlateDelivery qs2 = go.GetComponent<QuestSlateDelivery>();
                qs2.position = position;
                qs2.MyCargoDelivered = CargosDelivered.Find(x => x.type == q.CargoType);
                qs2.MyQuest = q;
                break;
            default:
                break;
        }
        

    }

    void configthislevel()
    {
        switch (myLevel.mode)
        {
            case LevelMode.TimeAttack:
                OnScore = TimeAttackOnScoreListener;
                OnMoneyGain = TimeAttackOnMoneyGainListener;
                OnCompeletedQuest = TimeAttackOnCompletedQuestListener;



                TimeController.s.SetTimer((float)myLevel.startingTimer, TimeAttackEndGameVictoryCheck, true, 0f, false);
                break;
            default:
                break;
        }


    }

    IEnumerator EndGameSequence()
    {
        yield return new WaitForSeconds(2f);
        OutroPanel.SetActive(true);
        GUIPanel.SetActive(false);
        int e = 0;
        foreach (Quest q in myLevel.quests)
        {               
            e += 1;
            //CreateGUISlate(q, e);
            CreateIPGUISlate(q, e,false);
        }

    }


    #region levellisteners & endgameCheckers
    #region TimeAttackMode
    void TimeAttackOnScoreListener(CargoType cargo)
        {
        //Checkeamos si se cumplió alguna misión!
        CargoDelivered CD = _cargosDelivered.Find(x => x.type == cargo);
        if (CD == null) {
            Debug.LogError("Cannot Find CargoType: " + cargo.ToString());
            return;
        }
        int cargoamount = CD.delivered;
        bool b = false;
        foreach (Quest q in myLevel.quests)
        {
            if (q.CheckQuest(cargoamount, WinCondition.Delivered, cargo)) b = true;
        }
        //Si se cumplió alguna misión lanzamos el evento de misión completada (que tal vez complete otras misiones)
        if (b && OnCompeletedQuest != null) OnCompeletedQuest(TimeController.s.timeSpent);
        foreach (QuestSlateDelivery qsd in FindObjectsOfType<QuestSlateDelivery>()) qsd.UpdateGUI();
    }
    void TimeAttackOnMoneyGainListener(int increment)
    {
        //Checkeamos si se cumplió alguna misión!
        bool b = false;
        foreach (Quest q in myLevel.quests) 
        {
            if (q.CheckQuest(money, WinCondition.Money)) b = true;
        }
        //Si se cumplió alguna misión lanzamos el evento de misión completada (que tal vez complete otras misiones)
        if (b && OnCompeletedQuest != null) OnCompeletedQuest(TimeController.s.timeSpent);



        foreach (QuestSlateDelivery qsd in FindObjectsOfType<QuestSlateDelivery>()) qsd.UpdateGUI();
    }
    void TimeAttackOnCompletedQuestListener(float time)
    {        
        //Checkeamos si se cumplió alguna misión!
        bool b = false;
        foreach (Quest q in myLevel.quests)
        {
            if (q.CheckQuest((int)time, WinCondition.Time, CargoType.None, true)) b = true;
        }
        //Si se cumplió alguna misión lanzamos el evento de misión completada (que tal vez complete otras misiones)
        if (b && OnCompeletedQuest != null) OnCompeletedQuest(TimeController.s.timeSpent);



        foreach (QuestSlateDelivery qsd in FindObjectsOfType<QuestSlateDelivery>()) qsd.UpdateGUI();
    }

    void TimeAttackEndGameVictoryCheck()
    {


        FreezeGame(true);
        FinishText.SetActive(true);
        SaveProfile();
        StartCoroutine(EndGameSequence());
        
    }
    void SaveProfile() {
        bool b = false;
        int stars = myLevel.CheckQuests();
        if (stars > 0) { b = true; sProfileManager.ProfileSingleton.profileLevels[level].beated = true; }
        if (sProfileManager.ProfileSingleton.profileLevels[level].stars < stars) { b = true; sProfileManager.ProfileSingleton.profileLevels[level].stars = stars; }
        if (b) sSaveLoad.SaveProfile();       


    }
    #endregion
    #endregion




    #region Freeze Control

    
    /// <summary>
    /// freezes all game
    /// </summary>
    /// <param name="freeze">True= freezes, false= unfreezes</param>
    void FreezeGame(bool freeze)
    {
        
        if (freeze)
        {
            foreach (TruckEntity te in FindObjectsOfType<TruckEntity>()) te.Freeze();
            foreach (RoadEntity re in FindObjectsOfType<RoadEntity>()) re.Freeze();
            if (TimeController.s.timer != null) TimeController.s.timer.StopTimer();
        }
        else
        {
            foreach (TruckEntity te in FindObjectsOfType<TruckEntity>()) te.Unfreeze();
            foreach (RoadEntity re in FindObjectsOfType<RoadEntity>()) re.Unfreeze();
            if (TimeController.s.timer != null) TimeController.s.timer.StartTimer();
        }
    }
    #endregion


    #endregion








}



public enum LevelMode { TimeAttack = 0 }

[System.Serializable]
public class LevelConditions
{
    #region Level Mode
    [SerializeField]
    LevelMode _mode;
    public LevelMode mode
    {
        get
        {
            return _mode;
        }

        protected set
        {
            _mode = value;
        }
    }
    #endregion

    #region StartingMoney
    [SerializeField]
    int _startingMoney;
    public int startingMoney
    {
        get
        {
            return _startingMoney;
        }

        protected set
        {
            _startingMoney = value;
        }
    }
    #endregion

    #region StartingTimer
    [SerializeField]
    int _startingTimer;
    public int startingTimer
    {
        get
        {
            return _startingTimer;
        }

        protected set
        {
            _startingTimer = value;
        }
    }
    #endregion

    #region level
    [SerializeField]
    int _level;
    public int level
    {
        get
        {
            return _level;
        }

        protected set
        {
            _level = value;
        }
    }
    #endregion

    #region quests
    [SerializeField]
    List<Quest> _quests;
    public List<Quest> quests
    {
        get
        {
            return _quests;
        }

        set
        {
            _quests = value;
        }
    }
    #endregion

    #region Effects
    [SerializeField]
    List<ConstantEffect> _effects;
    public List<ConstantEffect> effects
    {
        get
        {
            return _effects;
        }

        set
        {
            _effects = value;
        }
    }


    #endregion

    #region usingEffects
    [SerializeField]
    bool _usingEffects;
    public bool usingEffects
    {
        get
        {
            return _usingEffects;
        }

        protected set
        {
            _usingEffects = value;
        }
    }








    #endregion
       

    #region constructor

    public LevelConditions(int level, LevelMode levelmode, int startingmoney, int startingtimer, List<Quest> listofquests, List<ConstantEffect> listofeffects = null)
    {
        _level = level;
        _quests = listofquests;
        if (listofeffects == null) { _usingEffects = false; } else { _usingEffects = true; }
        _effects = listofeffects;
        _mode = levelmode;
        _startingMoney = startingmoney;
        _startingTimer = startingtimer;

    }


    #endregion
    


    /// <summary>
    /// Devuelve el numero de Estrellas conseguidas
    /// </summary>
    /// <returns>numero de 0-3</returns>
    public int CheckQuests()
    {
        int i = 0;
        foreach (Quest q in _quests)
        {
            if (q.completed) i += q.starRewards;
        }
        return i;
    }


}

public enum WinCondition { Delivered = 0, Money = 1, Time = 2 }

[System.Serializable]
public class Quest{
    #region completed
    [SerializeField]
    bool _completed;
    public bool completed
    {
        get
        {
            return _completed;
        }

        set
        {
            _completed = value;
        }
    }


    #endregion
    
    #region LinkedQuest
    [NonSerialized]
    List<Quest> _linkedQuest;
    
    public List<Quest> LinkedQuest
    {
        get
        {
            return _linkedQuest;
        }

        set
        {
            _linkedQuest = value;
        }
    }
    #endregion
    
    #region LinkedQuestIndex
    [SerializeField]
    List<int> _LinkedQuestIndex;
    public List<int> LinkedQuestIndex
    {
        get
        {
            return _LinkedQuestIndex;
        }

        set
        {
            _LinkedQuestIndex = value;
        }
    }
    #endregion

    #region LinkedQuestsBool
    [SerializeField]
    bool _linkedQuestEnabled = false;
    public bool LinkedQuestEnabled
    {
        get
        {
            return _linkedQuestEnabled;
        }

        set
        {
            _linkedQuestEnabled = value;
        }
    }
    #endregion 
    #region winCondition
    [SerializeField]
    WinCondition _winCondition;
    public WinCondition winCondition
    {
        get
        {
            return _winCondition;
        }

        protected set
        {
            _winCondition = value;
        }
    }
    #endregion
    #region winAmount
    [SerializeField]
    int _winAmount;
    public int winAmount
    {
        get
        {
            return _winAmount;
        }

        protected set
        {
            _winAmount = value;
        }
    }
    #endregion

    #region CargoType
    [SerializeField]
    CargoType _cargoType;
    public CargoType CargoType
    {
        get
        {
            return _cargoType;
        }

        set
        {
            _cargoType = value;
        }
    }

    #endregion


    [SerializeField]
    int _starRewards;
    public int starRewards
    {
        get
        {
            return _starRewards;
        }

        protected set
        {
            _starRewards = value;
        }
    }

    







    #region constructor

    public Quest(WinCondition wincondition, int amount)
    {
        _winCondition = wincondition;
        _winAmount = amount;
        _linkedQuestEnabled = false;
    }

    #endregion

    public bool CheckQuest(int amountToCheck, WinCondition TypeOfWinChecked, CargoType cargo = CargoType.None, bool Under = true)
    {
        if (_winCondition != TypeOfWinChecked) return false;
        if (_completed) return false;
        if (TypeOfWinChecked == WinCondition.Delivered)
        {
            if (_cargoType != cargo) return false;
        }

        if (_linkedQuestEnabled)
        {
            if (!(_linkedQuest.Any(x => x.completed == false)))
            {//Si no encuentra false (todas las quests enlazadas están true)
                if (Under) if (amountToCheck <= _winAmount) _completed = true;
                if (!Under) if (amountToCheck >= _winAmount) _completed = true;
            }


        }else
        {
            if (amountToCheck >= _winAmount) _completed = true;
        }


        return _completed;
        
        
    }


}

public enum EffectTarget { Delivered = 0, Money = 1, Timer = 2 }
[System.Serializable]
public class ConstantEffect
{
    #region target
    [SerializeField]
    EffectTarget _target;
    public EffectTarget target
    {
        get
        {
            return _target;
        }

        protected set
        {
            _target = value;
        }
    }
    #endregion
    #region decrement
    [SerializeField]
    bool _decrement = true;
    public bool decrement
    {
        get
        {
            return _decrement;
        }

        protected set
        {
            _decrement = value;
        }
    }


    #endregion
    #region amount
    [SerializeField]
    int _amount;
    public int amount
    {
        get
        {
            return _amount;
        }

        protected set
        {
            _amount = value;
        }
    }


    #endregion
    #region action
    [System.NonSerialized]
    Action _action;
    public Action Action
    {
        get
        {
            return _action;
        }

        protected set
        {
            _action = value;
        }
    }
    #endregion
    #region constructor
    public ConstantEffect(EffectTarget effecttarget, bool decrement, int effectamount, Action effectaction = null)    
    {
        if (effectaction == null) { _action = () => { }; } else { _action = effectaction; }
        _target = effecttarget;
        _decrement = decrement;
        _amount = effectamount;
    }
    #endregion
}




[System.Serializable]
public class CargoDelivered
{
    #region type
    [SerializeField]
    CargoType _type;
    public CargoType type
    {
        get
        {
            return _type;
        }

        set
        {
            _type = value;
        }
    }
    #endregion

    #region UIShown
    [SerializeField]
    bool _uishown;
    public bool uishown
    {
        get
        {
            return _uishown;
        }

        set
        {
            _uishown = value;
        }
    }


    #endregion

    #region delivered
    [SerializeField]
    int _delivered;
    public int delivered
    {
        get
        {
            return _delivered;
        }

        set
        {
            _delivered = value;
            GameController.OnScoreCall(_type);
            
        }
    }

    #endregion


}

