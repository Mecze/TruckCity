using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.SceneManagement;



//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// GAME CONTROLLER
//////////////////////////////
/// Este Script inicializa el nivel al entrar
/// y lleva los computos importantes del nivel.
//////////////////////////////


public delegate void ScoreEvent(CargoType cargo);
public delegate void MoneyEvent(int increment);
public delegate void CompeletedQuestEvent(float momentInTime);
public delegate void PauseEvent();


public class GameController : MonoBehaviour {
#pragma warning disable 0169
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
                //Debug.LogError("No Existe Singleton GameController");
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }




    #endregion

    [Header("Menu Version of the GC")]
    [SerializeField]
    public bool MenuVersion = false;


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
    [Header("Delivered (Score)")]
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
            if (MenuVersion) return;
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
    CountDownSprite CountDownSprite;
    [SerializeField]
    GameObject CounddownOBJ;
    [SerializeField]
    GameObject FinishText;
    [SerializeField]
    TweenScale FinishTextScaleTween;
    [SerializeField]
    TweenAlpha GUIPanelAlphaTween;
    [SerializeField]
    TweenAlpha IntroPanelAlphaTween;
    //[SerializeField]
    //TweenAlpha OutroPanelAlphaTween;
    [SerializeField]
    TweenPosition PauseButtonPosTween;
    [SerializeField]
    GameObject QuestGrid;
    [SerializeField]
    UILabel IntroMenuMainLabel;
    [SerializeField]
    TweenAnimationController TimerTweenController;
    public Camera UICamera;
    


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

    /// <summary>
    /// Evita que se reproduzca dos veces el ending.
    /// </summary>
    bool gameEnding = false;
    public bool gameStarted = true;
    //bool PreeGameEngind = false;
    [System.NonSerialized]
    public bool MusicPlaying = false;

    [Header("Default Level (For testing)")]    
    public LevelConditions defaultLevel;


    #region StartGame SEQUENCE
    //En orden
    //Todo lo ubicado en esta región se ejecuta en orden hasta la
    //siguiente region

    /// <summary>
    /// La Ejecución del juego empieza AQUI!
    /// </summary>
    void Awake()
    {
        SetQuality();
        fillmylevel();
    }
    /// <summary>
    /// Ajusta el nivel de detalle de todos los elementos del juego con distintos
    /// niveles de detalle
    /// </summary>
    void SetQuality()
    {

        GraphicQualitySettings QS;
        if (sProfileManager.ProfileSingleton != null)
        {
            QS = sProfileManager.ProfileSingleton.GlobalGraphicQualitySettings;
        }else
        {
            QS = GraphicQualitySettings.High;
        }
        QualitySelector[] Qss = GameObject.FindObjectsOfType<QualitySelector>();
        QualitySelector.instances = Qss.Length;
        QualitySelector.finishedinstances = 0;
        if (Qss.Length < 1) return; //Failsafe
        string s;
        if (GameConfig.s != null)
        {
            s = GameConfig.s.LowIMGPath;
        }else
        {
            s = "IMG\\LowIMGs\\";
        }
        for (int i = 0; i < Qss.Length; i++)
        {
            Qss[i].Set(QS,s);
        }
    }


    /// <summary>
    /// Inicia la secuencia al inicio del juego
    /// </summary>
    void fillmylevel()
    {   
        if (!MenuVersion)
        {
            //Congela los elementos jubales por ahora
            FreezeGame(true);

            //Ajustamos este nivel dependiendo de el sitio en las build settings de su escena
            level = sProfileManager.ProfileSingleton.newLevelIndex;
            sProfileManager.ProfileSingleton.ChangingLevel = false;

            if (sProfileManager.instance != null) if (level > sProfileManager.instance.levelconditions.Count - 1) level = 1;
            //Clonamos la configuración de este nivel (LevelConditions)
            if (sProfileManager.instance != null)
            {
                LevelConditions LC = sProfileManager.instance.levelconditions.Find(x => x.Code == sProfileManager.ProfileSingleton.profileLevels[level].code);
                
                if (LC != null)myLevel = ObjectCloner.Clone<LevelConditions>(LC);
                if (LC == null) { myLevel = defaultLevel; Debug.LogWarning("Failed to Load Level Conditions"); }
            }
            //if sProfiel dues not exist, Bootstrap a TEST level
            if (sProfileManager.instance == null)
            {
                myLevel = defaultLevel;

            }
            
            //GUI Activa el panel de entrada y la GUI detras
            IntroPanel.SetActive(true);
            GUIPanel.SetActive(true);
            //Set Strings
            MoneyText.text = _money.ToString();
            IntroMenuMainLabel.text = "Level " + (level + 1).ToString() + " - Menu";

            //Configuramos el MODO de este Nivel
            //(Actualmente solo existe TimeAttack)
            //TODO: More LevelModes
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
                    TimeController.s.timer.AddAction(10, ActivateLowTimeTimerAnimation);
                    //TimeController.s.timer.AddAction(0, StopLowTimeTimerAnimation);
                    foreach (Quest q in myLevel.quests.FindAll(x => x.winCondition == WinCondition.Time))
                    {
                        TimeController.s.timer.AddAction((int)((float)myLevel.startingTimer - (float)q.winAmount), () => { q.finished = true; });
                    }

                    break;
                default:
                    break;

            }

            //CONFIG QUEST FOREACH LOOP (FOR en el futuro)

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
                CreateIPGUISlate(q, e, false);
            }

            //Tras crear Slates se le indica al Grid que se ajuste
            //(No es automático)
            GameObject.FindGameObjectWithTag("QuestGUI").GetComponent<UIGrid>().Reposition();
            GameObject.FindGameObjectWithTag("IPQuestSlateAnchor").GetComponent<UIGrid>().Reposition();
            //El juego comienza en Pause, no se deben ver repetidas las Quest:
            QuestGrid.SetActive(false); //QuestGrid son las quest principales. IP las del Menú

            //Ahora se Activa la Intro
            //(En la Escena viene ya activada por defecto) FailSafe:


            //Esta lina activa el tutorial (si es false) 
            //O va directo al menú de INTRO (si es true)
            if (sProfileManager.instance != null)
            {
                TutorialController.s.hasTutorial = !sProfileManager.ProfileSingleton.profileLevels[level].TutorialDone;
                TutorialController.ChangeMyState(sProfileManager.ProfileSingleton.profileLevels[level].TutorialDone);
                if (!sProfileManager.ProfileSingleton.profileLevels[level].TutorialDone) { TutorialController.s.ResetTutorialAndGo(); } else { TutorialController.s.secondTime = true; }
            }
            

            

            


        }else
        {
            //En caso de que esto sea el MENU (Menu Versión == true), ponemos musica de menu si es necesario.
            if (GameConfig.s.MusicState) if (!MusicStore.s.AliasIsPlaying("Menu")) MusicStore.s.PlayMusicByAlias("Menu", 0f, GameConfig.s.MusicVolume, true, 5f, true, 1f);
        }
        //NOTA:
        //Cuando el jugador indica de empezar a jugar se ejecuta
        //"LaunchCountdownAnimation()" 
        //y despues del Countdown = "StartGame()" (ver Mas abajo)


    }

    //no usado por ahora
    IEnumerator AdjustGrids(bool IP)
    {
        yield return new WaitForSeconds(0.1f);
        if (!IP)GameObject.FindGameObjectWithTag("QuestGUI").GetComponent<UIGrid>().Reposition();
        if (IP)GameObject.FindGameObjectWithTag("IPQuestSlateAnchor").GetComponent<UIGrid>().Reposition();
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

            //Musica:
            MusicPlaying = true;
            PlayLevelMusic(true);


        }
        else
        {
            //Si pause es TRUE quiere decir que NO es el inicio del MAPA
            //Si nó que se hizó pause
            Pause = false;
            IntroPanelAlphaTween.PlayForward();            
            StartCoroutine(StartButtonAfterFadeOut(GUIPanelAlphaTween.duration + 0.1f, false));
            

        }
    }

    

    /// <summary>
    /// Espera el FadeOut del Menú
    /// Ver StartButtonClicked()
    /// </summary>
    /// <param name="time"></param>
    /// <param name="launchCountdown"></param>
    /// <returns></returns>
    IEnumerator StartButtonAfterFadeOut(float time, bool launchCountdown)
    {
        yield return new WaitForSeconds(time);
        if (launchCountdown)
        {
            IntroPanel.SetActive(false);
            GUIPanel.SetActive(true);            
            QuestGrid.SetActive(true);
            //StartCoroutine(AdjustGrids(false));
            CounddownOBJ.SetActive(true);
            PauseButtonPosTween.PlayForward();
            CountDownSprite.StartCountDown();
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
    /// (Esto se ejecuta desde el ANIMADOR del CountDown inicial)
    /// </summary>
    public void StartGame()
    {
        gameStarted = true;
        gameEnding = false;
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
    /// <param name="outro">Si es intro o no</param>
    void CreateIPGUISlate(Quest q, int position, bool outro)
    {
        GameObject go;
        string s;
        if (outro)
        {
            s = "OPQuestSlateAnchor";
        }else
        {
            s = "IPQuestSlateAnchor";
        }
        go = GameObject.Instantiate(IPQuestSlatePrefab);
        QuestSlate qs = go.GetComponent<QuestSlate>();
        qs.position = position;
        qs.MyCargoDelivered = CargosDelivered.Find(x => x.type == q.CargoType);
        qs.MyQuest = q;
        qs.IP = true;
        go.transform.SetParent(GameObject.FindGameObjectWithTag(s).transform);
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
    
    #region Button Behaviours

    #region Pause Behaviour

    /// <summary>
    /// Lanzado desde el boton de pause
    /// </summary>
    public void PauseButton()
    {
        if (CounddownOBJ.activeSelf == false)
        PauseGame(true);
    }

    /// <summary>
    /// Accíón de pausar el juego
    /// </summary>
    /// <param name="b"></param>
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

    #endregion

    /// <summary>
    /// Se llama desde los botones de Retry.
    /// </summary>
    public void RetryLevel()
    {
        ResetMusic();
        //SceneManager.UnloadScene(SceneManager.GetActiveScene());
        sProfileManager.s.ChangeLevel(myLevel.Code);
    }

    /// <summary>
    /// Se llama desde los botones "Back to main menu"
    /// </summary>
    public void BackToMenu()
    {
        ResetMusic();
        LoadingScreenManager.LoadScene(1);
    }

    /// <summary>
    /// Try to Call for next Level
    /// </summary>
    public void NextLevel()
    {
        if (sProfileManager.s.IsNextLevelUnlocked(level))
        {
            sProfileManager.s.ChangeLevel(sProfileManager.ProfileSingleton.profileLevels[level + 1].code);
        }
    }
    #endregion

    #region Animation Controllers

    /// <summary>
    /// Activa la animación de poco tiempo y reproduce la musica o sonido correspondiente si procede
    /// </summary>
    void ActivateLowTimeTimerAnimation()
    {
        if (gameEnding) return;
        //Si la musica está activada:
        if (sProfileManager.ProfileSingleton.MusicState)
        {
            SoundSystem.s.FadeToMusic(0.2f, 0.2f, () =>
            {   
                //si el sonido está activado
                if (sProfileManager.ProfileSingleton.SoundState)
                {
                    SoundStore.s.PlaySoundByAlias("TimeRunOut", 0f, GameConfig.s.SoundVolume + 0.5f, false, 0f, false, 0f, () =>
                    {
                        if (!gameEnding) SoundSystem.s.AudioSources[0].pitch = 1.2f;
                        if (sProfileManager.ProfileSingleton.MusicState) SoundSystem.s.FadeToMusic(1f, 0.2f);
                    });
                }else
                {
                    //Si el sonido noe stá activado
                    if (!gameEnding) SoundSystem.s.AudioSources[0].pitch = 1.2f;
                }
            });
        }
        //Si la musica no está activada
        if (!sProfileManager.ProfileSingleton.MusicState)
        {
            //Se ejecuta este sonido si los sonidos están activados
            if (sProfileManager.ProfileSingleton.SoundState) SoundStore.s.PlaySoundByAlias("TimeRunOut", 0f, GameConfig.s.SoundVolume + 0.5f, false, 0f, false, 0f);            
            
            //Si todo esta desactivado no se hace nada.
        }

        //esto se hace siempre:
        TimerTweenController.PlayAnimations(true);
    }

    /// <summary>
    /// Para la animáción de poco tiempo
    /// </summary>
    void StopLowTimeTimerAnimation()
    {
        SoundSystem.s.AudioSources[0].pitch = 1f;
        TimerTweenController.StopAnimations();
    }


    #endregion
    
    #region levellisteners & endgameCheckers

    #region TimeAttackMode

    /// <summary>
    /// Listener del evento "OnScore" para el modo TimeAttack
    /// </summary>
    /// <param name="cargo"></param>
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

    /// <summary>
    /// Listener del evento "OnMoneyGained" para el modo TimeAttack
    /// </summary>
    /// <param name="increment"></param>
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

    /// <summary>
    /// Listener del evento "OnCompletedQuest" para el modo TimeAttack
    /// </summary>
    /// <param name="time"></param>
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
        bool c = myLevel.quests.Any(x => x.finished == false);
        //Si todas las misiones han sido cumplidas:
        if (!c)
        {
            StartCoroutine(PreemtytiveEndingTimeAttack());
        }

        //foreach (QuestSlate qsd in FindObjectsOfType<QuestSlate>()) qsd.UpdateGUI();
    }

    /// <summary>
    /// Inicializa el final del nivel
    /// </summary>
    void TimeAttackEndGameVictoryCheck()
    {
        if (gameEnding) return;
        gameEnding = true;
        gameStarted = false;
        if (sProfileManager.s != null && SoundSystem.s != null)
        {
            if (sProfileManager.ProfileSingleton.MusicState)
            {
                SoundSystem.s.FadeOutMusic(0.2f, () => { StopLowTimeTimerAnimation(); MusicStore.s.PlayMusicByAlias("Victory", 0f, 1f); });
            }
            else
            {
                StopLowTimeTimerAnimation();
            }
        }
        FreezeGame(true);
        FinishText.SetActive(true);
        FinishTextScaleTween.PlayForward();
        SaveProfile();
        StartCoroutine(EndGameSequence());

    }

    /// <summary>
    /// Termina el juego antes de tiempo.
    /// </summary>
    /// <returns></returns>
    IEnumerator PreemtytiveEndingTimeAttack()
    {
        //gameEnding = true;
        yield return new WaitForSeconds(2f);
        TimeAttackEndGameVictoryCheck();
    }

    #endregion

    #region Common To all Modes

    /// <summary>
    /// Este metodo permita a scripts de fuera lanzar el Evento "OnScore"
    /// </summary>
    /// <param name="cargo"></param>
    public void OnScoreCall(CargoType cargo)
    {
        if (OnScore != null)
        {

            OnScore(cargo);
        }
    }


    /// <summary>
    /// Corutina llamada desde el Victory Checker
    /// para iniciar el final del nivel
    /// </summary>
    /// <returns></returns>
    IEnumerator EndGameSequence()
    {
        yield return new WaitForSeconds(3.5f);
        FinishTextScaleTween.ResetToBeginning();
        FinishText.SetActive(false);
        OutroPanel.SetActive(true);
        int e = 1;
        foreach (Quest q in myLevel.quests)
        {
            CreateIPGUISlate(q, e, true);
            e++;
        }
        GameObject OPAnchor = GameObject.FindGameObjectWithTag("OPQuestSlateAnchor");
        OPAnchor.GetComponent<UIGrid>().Reposition();
        /*
        Vector3 pos = OPAnchor.transform.position;
        pos.y = 100f;
        OPAnchor.transform.position = pos;
        */
        OPAnchor.transform.position = OPAnchor.transform.parent.transform.position;
        PauseButtonPosTween.PlayReverse();
        GUIPanel.SetActive(false);
        OutroPanelScript ops = OutroPanel.GetComponent<OutroPanelScript>();
        ops.ShowPanel(level, myLevel.CheckQuests(), sProfileManager.s.IsNextLevelUnlocked(level));
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
            /*
            foreach (TruckEntity te in FindObjectsOfType<TruckEntity>()) te.Freeze();
            foreach (RoadEntity re in FindObjectsOfType<RoadEntity>()) re.Freeze();
            var type = typeof(IFreezable);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => type.IsAssignableFrom(p));
            foreach(var t in types)
            {
                
                
            }
            */

            foreach(IFreezable IF in InterfaceHelper.FindObjects<IFreezable>())
            {
                IF.Freeze();
            }

            if (TimeController.s.timer != null) TimeController.s.timer.StopTimer();

        }
        else
        {
            /*
            foreach (TruckEntity te in FindObjectsOfType<TruckEntity>()) te.Unfreeze();
            foreach (RoadEntity re in FindObjectsOfType<RoadEntity>()) re.Unfreeze();
            */
            foreach (IFreezable IF in InterfaceHelper.FindObjects<IFreezable>())
            {
                IF.Unfreeze();
            }
            if (TimeController.s.timer != null) TimeController.s.timer.StartTimer();
        }
    }
    #endregion
    
    #region Other Tools


    /// <summary>
    /// Se lanza desde diferentes sitios para lanzar la musica correcta
    /// </summary>
    /// <param name="startLevel"></param>
    public void PlayLevelMusic(bool startLevel)
    {
        float delay = 0f;
        if (startLevel) delay = 3f;
        if (MusicButton.s == null || MusicStore.s == null) return;
        if (!MenuVersion) if (!MusicStore.s.AliasIsPlaying(myLevel.MusicAlias)) MusicStore.s.PlayMusicByAlias(myLevel.MusicAlias, delay, GameConfig.s.MusicVolume, true, 2f, true, 2f, null, () => { if (MusicButton.s != null) MusicButton.s.Clickable = true; });
        if (MenuVersion) if (!MusicStore.s.AliasIsPlaying("Menu")) MusicStore.s.PlayMusicByAlias("Menu", 0f, GameConfig.s.MusicVolume, true, 0f, true, 1f, null, () => { if (MusicButton.s != null) MusicButton.s.Clickable = true; });
    }

    /// <summary>
    /// Resetea la Musica a su estado original, si procede
    /// </summary>
    void ResetMusic()
    {
        if (sProfileManager.ProfileSingleton.MusicState) SoundSystem.s.FadeOutMusic(0.2f, () => { SoundSystem.s.AudioSources[0].pitch = 1f; });
    }

    /// <summary>
    /// Guarda el Perfil.
    /// </summary>
    public void SaveProfile(bool force = false)
    {
        if (sProfileManager.s == null) return;
        bool b = false;
        int stars = myLevel.CheckQuests();
        if (stars > 0) { b = true; sProfileManager.ProfileSingleton.profileLevels[level].beated = true; }
        if (sProfileManager.ProfileSingleton.profileLevels[level].stars < stars) { b = true; sProfileManager.ProfileSingleton.profileLevels[level].stars = stars; }
        if (b || force) sSaveLoad.SaveProfile();


    }

    #region FloatingTextThing
    /*
    /// <summary>
    /// Genera un Floating Text con los siguientes parametros:
    /// </summary>
    /// <param name="pos">Posición en el mundo (se traducirá a posición en la pantalla)</param>
    /// <param name="text">Texto a mostrar</param>
    /// <param name="publiccolor">Color del Texto a mostrar</param>
    /// <param name="spriteName">El nombre en el Atlas del Sprite a mostrar</param>
    /// <param name="CargoColor">Color del Sprite a Mostrar (Si se usa Color.Black, se usa el color original del sprite)</param>
    /// <param name="delay">[Opcional] Tiempo de espera hasta que se Spawnea (por defecto = 0)</param>
    public void FloatingTextSpawn(Vector2 pos, string text, enumColor publiccolor, string spriteName, Color CargoColor, float delay = 0f)
    {
        Vector3 realPos = new Vector3(pos.x, defaulYFloatingText, pos.y);
        FloatingTextSpawn(realPos, text, publiccolor, spriteName, CargoColor, delay);
    }
    /// <summary>
    /// Genera un Floating Text con los siguientes parametros:
    /// </summary>
    /// <param name="pos">Posición en el mundo (se traducirá a posición en la pantalla)</param>
    /// <param name="text">Texto a mostrar</param>
    /// <param name="publiccolor">Color del Texto a mostrar</param>
    /// <param name="spriteName">El nombre en el Atlas del Sprite a mostrar</param>
    /// <param name="CargoColor">Color del Sprite a Mostrar (Si se usa Color.Black, se usa el color original del sprite)</param>
    /// <param name="delay">[Opcional] Tiempo de espera hasta que se Spawnea (por defecto = 0)</param>
    public void FloatingTextSpawn(float x, float z, string text, enumColor publiccolor, string spriteName, Color CargoColor, float delay = 0f)
    {
        Vector3 realPos = new Vector3(x, defaulYFloatingText, z);
        FloatingTextSpawn(realPos, text, publiccolor, spriteName, CargoColor, delay);
    }
    */
    /// <summary>
    /// Genera un Floating Text con los siguientes parametros:
    /// </summary>
    /// <param name="pos">Posición en el mundo (se traducirá a posición en la pantalla)</param>
    /// <param name="text">Texto a mostrar</param>
    /// <param name="publiccolor">Color del Texto a mostrar</param>
    /// <param name="spriteName">El nombre en el Atlas del Sprite a mostrar</param>
    /// <param name="CargoColor">Color del Sprite a Mostrar (Si se usa Color.Black, se usa el color original del sprite)</param>
    /// <param name="delay">[Opcional] Tiempo de espera hasta que se Spawnea (por defecto = 0)</param>
    public void FloatingTextSpawn(Transform pos, string text, enumColor publiccolor, string spriteName, Color CargoColor, float delay = 0f)
    {
        if (MenuVersion) return;
        StartCoroutine(SpawnFloatingText(pos, text, publiccolor, spriteName, CargoColor, delay));
    }
    //Aquí se ejecuta el Delay de "FloatingTextSpawn(...)"
    IEnumerator SpawnFloatingText(Transform pos, string text, enumColor publiccolor, string spriteName, Color CargoColor, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject go = (GameObject)GameObject.Instantiate(textGOPrefab);
        go.GetComponent<FloatingText>().WakeMeUp(text, spriteName, GameConfig.s.publicColors[(int)publiccolor], pos, CargoColor);
    }

    #endregion



    #endregion

}



public enum LevelMode { TimeAttack = 0 }

[System.Serializable]
public class LevelConditions
{
    #region Code
    [SerializeField]
    string code;
    /// <summary>
    /// The code of the LevelCondition. It has to match ANY "ProfileLevel" Code
    /// </summary>
    public string Code
    {
        get
        {
            return code;
        }

        set
        {
            code = value;
        }
    }

    #endregion

    #region LevelToLoad

    [SerializeField]
    int buildSettingOrder;
    /// <summary>
    /// The Level to LOAD on the buildSettingsOrder
    /// </summary>
    public int BuildSettingOrder
    {
        get
        {
            return buildSettingOrder;
        }

        set
        {
            buildSettingOrder = value;
        }
    }

    #endregion

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

    #region MusicUsed
    [SerializeField]
    string _musicAlias;
    public string MusicAlias
    {
        get
        {
            return _musicAlias;
        }

        set
        {
            _musicAlias = value;
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

public delegate void FailQuestDelegate(Quest thisQuest);


[System.Serializable]
public class Quest{
    public event FailQuestDelegate OnFailQuest;
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
            if (_completed) _finished = true;
        }
    }


    #endregion

    #region finished
    //if Finished is true and completed is false, the quest failed
    [SerializeField]
    bool _finished;
    public bool finished
    {
        get
        {
            if (_completed)
            {
                _finished = true;
            }
            return _finished;
        }
        set
        {
            _finished = value;
            //Launch the fail event quest.
            if (_finished == true && _completed == false) if (OnFailQuest != null) OnFailQuest(this);
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
        if (TypeOfWinChecked != WinCondition.Time) if (_winCondition != TypeOfWinChecked) return false;
        if (_completed) return false;
        if (TypeOfWinChecked == WinCondition.Delivered)
        {
            if (_cargoType != cargo && _cargoType != CargoType.None) return false;
        }

        if (_linkedQuestEnabled)
        {
            
                if (!(_linkedQuest.Any(x => x.completed == false)))
                {//Si no encuentra false (todas las quests enlazadas están true)
                    switch (TypeOfWinChecked)
                    {
                        case WinCondition.Delivered:
                            if (amountToCheck >= _winAmount) _completed = true;
                            break;
                        case WinCondition.Money:
                            if (amountToCheck >= _winAmount) _completed = true;
                            break;
                        case WinCondition.Time:
                            if (Under) if (amountToCheck <= _winAmount) _completed = true;
                            if (!Under) if (amountToCheck >= _winAmount) _completed = true;
                            break;
                        default:
                            break;
                    }
                }
        }else
        {
            if (_winCondition != TypeOfWinChecked) return false;
            if (amountToCheck >= _winAmount) _completed = true;
        }
        if (_completed) _finished = true;

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

