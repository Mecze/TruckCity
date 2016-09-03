using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public delegate void ScoreEvent(int increment);
public delegate void MoneyEvent(int increment);






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

    #region delivered (score)
    private int _delivered = 0;
    public int delivered
    {
        get
        {
            return _delivered;
        }

        set
        {
            int i = value - _delivered;
            _delivered = value;
            if (OnScore != null) OnScore(i);

        }
    }


    #endregion
    #region money
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
        }
    }
    #endregion
    public static event ScoreEvent OnScore;
    public static event MoneyEvent OnMoneyGain;



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

    void fillmylevel()
    {
        myLevel = ObjectCloner.Clone<LevelConditions>(sProfileManager.instance.levelconditions.Find(x => x.level == level));
        foreach (Quest q in myLevel.quests) q.completed = false;
        //TODO: UPDATEGUI
        configthislevel();
    }

    void configthislevel()
    {
        switch (myLevel.mode)
        {
            case LevelMode.TimeAttack:
                OnScore = TimeAttackOnScoreListener;
                OnMoneyGain = TimeAttackOnMoneyGainListener;



                TimeController.s.SetTimer((float)myLevel.startingTimer, TimeAttackEndGameVictoryCheck, true);
                break;
            default:
                break;
        }


    }
    #region levellisteners & endgameCheckers
    #region TimeAttackMode
    void TimeAttackOnScoreListener(int increment)
        {
            //Checkeamos si se cumplió alguna misión!
            foreach (Quest q in myLevel.quests) q.CheckQuest(delivered, WinCondition.Delivered);
        //TODO: UpdateGUI;
    }
    void TimeAttackOnMoneyGainListener(int increment)
    {
        //Checkeamos si se cumplió alguna misión!
        foreach (Quest q in myLevel.quests) q.CheckQuest(money, WinCondition.Money);
        //TODO: UpdateGUI;
    }

    void TimeAttackEndGameVictoryCheck()
    {
        int stars = myLevel.CheckQuests();
        //TODO: DO GUI
        Debug.Log("Has Conseguido " + stars.ToString() + " estrellas!");
    }
    #endregion
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
            if (q.completed && i < q.starRewards) i = q.starRewards;
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
    }

    #endregion

    public void CheckQuest(int amountToCheck, WinCondition TypeOfWinChecked)
    {
        if (_winCondition != TypeOfWinChecked) return;
        if (_completed) return;
        if (amountToCheck >= _winAmount) _completed = true;
        
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

