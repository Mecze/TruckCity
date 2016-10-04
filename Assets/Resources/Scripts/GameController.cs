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
public delegate void PauseEvent();


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


    [NonSerialized]
    public bool Pause = false;
    [Header("Levels")]
    /// <summary>
    /// apunta al nivel actual en la lista "levelconditions"
    /// </summary>
    [NonSerialized]
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
    UILabel MoneyText;
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
    public event ScoreEvent OnScore;
    public event MoneyEvent OnMoneyGain;
    public event CompeletedQuestEvent OnCompeletedQuest;
    public event PauseEvent OnPause;
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
    [SerializeField]
    TweenAlpha GUIPanelAlphaTween;
    [SerializeField]
    TweenAlpha IntroPanelAlphaTween;
    [SerializeField]
    TweenPosition PauseButtonPosTween;
    [SerializeField]
    GameObject QuestGrid;


    [Header("Prefabs")]
    [SerializeField]
    GameObject QuestSlatePrefab;
   /* [SerializeField]
    GameObject QuestSlatePrefabMoney;
    [SerializeField]
    GameObject QuestSlatePrefabTimer;
    */
    [SerializeField]
    GameObject IPQuestSlatePrefab;

    #region StartGame SEQUENCE
    //En orden
    //Todo lo ubicado en esta región se ejecuta en orden hasta la
    //siguiente region

    /// <summary>
    /// La Ejecución del juego empieza AQUI!
    /// </summary>
    void Awake()
    {
        fillmylevel();
    }
    /// <summary>
    /// Inicia la secuencia al inicio del juego
    /// </summary>
    void fillmylevel()
    {
        
        

        //Congela los elementos jubales por ahora
        FreezeGame(true);
        

        //GUI Activa el panel de entrada y la GUI detras
        IntroPanel.SetActive(true);
        GUIPanel.SetActive(true);        
        MoneyText.text = _money.ToString();

        //Ajustamos este nivel dependiendo de el sitio en las build settings de su escena
        level = SceneManager.GetActiveScene().buildIndex - 3;
        //Clonamos la configuración de este nivel (LevelConditions)
        if (sProfileManager.instance != null) myLevel = ObjectCloner.Clone<LevelConditions>(sProfileManager.instance.levelconditions.Find(x => x.level == level));



        //Configuramos el MODO de este Nivel
        //(Actualmente solo existe TimeAttack)
        switch (myLevel.mode)
        {
            case LevelMode.TimeAttack:
                //En esta parte se inician los Listeners para los
                //distintos eventos que pueden ocurrir durante el juego
                OnScore += TimeAttackOnScoreListener;
                OnMoneyGain += TimeAttackOnMoneyGainListener;
                OnCompeletedQuest += TimeAttackOnCompletedQuestListener;
                //Se inicializa el Timer (no empieza aún, está Freeze)
                TimeController.s.SetTimer((float)myLevel.startingTimer, TimeAttackEndGameVictoryCheck, true, 0f, false);
                break;
            default:
                break;
                
        }


                //LINKED QUEST SET UP
                //---
                //Repasamos las Quest de este nivel, creamos la LISTA por referencia
                //de todas las misiones asociadas a otras misiones
                //Nota este Foreach vale para otra cosa mas(ver mas abajo)
                int e = 0;
        foreach (Quest q in myLevel.quests)
        {
            q.completed = false;
            q.LinkedQuest = new List<Quest>();

            if (q.LinkedQuestIndex.Count > 0)
            {
                foreach (int i in q.LinkedQuestIndex)
                {
                    if (i < e)
                    {
                        q.LinkedQuest.Add(myLevel.quests[i]);
                        q.LinkedQuestEnabled = true;
                    }
                }

            }
            e += 1;

        //---
        //Creamos los SLATES apropiados (Para UI y para la INTRO)
            CreateGUISlate(q, e);
            CreateIPGUISlate(q, e, true);
        }

        //Tras crear Slates se le indica al Grid que se ajuste
        //(No es automático)
        GameObject.FindGameObjectWithTag("QuestGUI").GetComponent<UIGrid>().Reposition();
        GameObject.FindGameObjectWithTag("IPQuestSlateAnchor").GetComponent<UIGrid>().Reposition();
        //El juego comienza en Pause, no se deben ver repetidas las Quest:
        QuestGrid.SetActive(false); //QuestGrid son las quest principales. IP las del Menú

        //Ahora se Activa la Intro
        //(En la Escena viene ya activada por defecto) FailSafe:
        IntroPanel.SetActive(true);       

        //NOTA:
        //Cuando el jugador indica de empezar a jugar se ejecuta
        //"LaunchCountdownAnimation()" 
        //y despues del Countdown = "StartGame()" (ver Mas abajo)


    }

    /// <summary>
    /// Es lanzado desde la UI para empezar la partida.
    /// </summary>
    public void StartButtonClicked()
    {
        if (!Pause)
        {
            
            IntroPanelAlphaTween.PlayForward();
            //Esto inicializa el Countdown.
            StartCoroutine(StartButtonAfterFadeOut(GUIPanelAlphaTween.duration+0.1f, true));
            
            //Al terminar el Countdown se lanza "StartGame"
            //Desde el Animator de CountdownAnimator
        }else
        {
            //Si pause es TRUE quiere decir que NO es el inicio del MAPA
            //Si nó que se hizó pause
            Pause = false;
            IntroPanelAlphaTween.PlayForward();            
            StartCoroutine(StartButtonAfterFadeOut(GUIPanelAlphaTween.duration + 0.1f, false));
            

        }
    }

    IEnumerator StartButtonAfterFadeOut(float time, bool launchCountdown)
    {
        yield return new WaitForSeconds(time);
        if (launchCountdown)
        {
            IntroPanel.SetActive(false);
            GUIPanel.SetActive(true);
            QuestGrid.SetActive(true);
            CounddownOBJ.SetActive(true);
            PauseButtonPosTween.PlayForward();
            CoundownAnimator.SetBool("Start", true);
        }
        else
        {
            QuestGrid.SetActive(true);
            IntroPanel.SetActive(false);
            GUIPanel.SetActive(true);
            PauseButtonPosTween.PlayForward();
            FreezeGame(false);

        }

    }

    /// <summary>
    /// Empieza el juego
    /// Apaga el Freeze y el Countdown
    /// </summary>
    public void StartGame()
    {        
        FreezeGame(false);
        CounddownOBJ.SetActive(false);        
    }


    #region Slate Creation
    /// <summary>
    /// Aqui se Crean los Slates de la INTRO del juego que muestran
    /// las misiones al jugador
    /// Este metodo crea 1 Slate. Este metodo se ejecuta en un bucle para cada quest
    /// </summary>
    /// <param name="q">la Quest</param>
    /// <param name="position">la posicion (bucle)</param>
    /// <param name="intro">Si es intro o no</param>
    void CreateIPGUISlate(Quest q, int position, bool intro)
    {
        GameObject go;
        go = GameObject.Instantiate(QuestSlatePrefab);
        QuestSlate qs = go.GetComponent<QuestSlate>();
        qs.position = position;
        qs.MyCargoDelivered = CargosDelivered.Find(x => x.type == q.CargoType);
        qs.MyQuest = q;
        qs.IP = true;
        go.transform.SetParent(GameObject.FindGameObjectWithTag("IPQuestSlateAnchor").transform);
        go.transform.localScale = Vector3.one;


        //Este Metodo inicializa el Slate
        qs.SetSlate();
    }

    /// <summary>
    /// Aqui se crean los Slates de la Interfaz de Mision (arriba izquierda de la UI)
    /// Este metodo crea 1 Slate. Este metodo se ejecuta en un bucle para cada quest
    /// </summary>
    /// <param name="q">La mision</param>
    /// <param name="position">La posición (bucle)</param>
    void CreateGUISlate(Quest q, int position)
    {
        GameObject go;
        go = GameObject.Instantiate(QuestSlatePrefab);
        QuestSlate qs = go.GetComponent<QuestSlate>();
        qs.position = position;
        qs.MyCargoDelivered = CargosDelivered.Find(x => x.type == q.CargoType);
        qs.MyQuest = q;
        qs.IP = false;
        go.transform.SetParent(GameObject.FindGameObjectWithTag("QuestGUI").transform);
        go.transform.localScale = Vector3.one;
        

        //Este Metodo inicializa el Slate
        qs.SetSlate();

    }
    #endregion


    #endregion

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
    public void PauseButton()
    {
        PauseGame(true);
    }

    public void PauseGame(bool b=true)
    {
        if (b)
        {
            Pause = true;
            FreezeGame(true);
            QuestGrid.SetActive(false);
            IntroPanel.SetActive(true);
            PauseButtonPosTween.PlayReverse();
            IntroPanelAlphaTween.PlayReverse();
            //Lanzamos el eveto de Pause, esto actualizará los Quest Slates en el menú de inicio
            if (OnPause != null) OnPause();


        }else
        {
            StartButtonClicked();
        }
    }

    public void OnScoreCall(CargoType cargo)
    {
        if (OnScore != null) {
           
           OnScore(cargo);
                }
    }
    public void AddOnScoreEvent(ScoreEvent a)
    {
        OnScore += a;
    }
    


    

    public void RetryLevel()
    {
        //SceneManager.UnloadScene(SceneManager.GetActiveScene());
        LoadingScreenManager.LoadScene(level + 3, true, level + 3);
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene(1);
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
        //foreach (QuestSlate qsd in FindObjectsOfType<QuestSlate>()) qsd.UpdateGUI();
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



        //foreach (QuestSlate qsd in FindObjectsOfType<QuestSlate>()) qsd.UpdateGUI();
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



        //foreach (QuestSlate qsd in FindObjectsOfType<QuestSlate>()) qsd.UpdateGUI();
    }

    void TimeAttackEndGameVictoryCheck()
    {


        FreezeGame(true);
        FinishText.SetActive(true);
        SaveProfile();
        StartCoroutine(EndGameSequence());
        
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
            CreateIPGUISlate(q, e, false);
        }

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
            GameController.s.OnScoreCall(_type);
            
        }
    }

    #endregion


}

