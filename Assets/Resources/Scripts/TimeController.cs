using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public delegate void OnStartClockDelegate();

public class TimeController : MonoBehaviour {
    #region Singleton
    private static TimeController s_singleton = null;

    public static TimeController singleton
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<TimeController>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("No Existe Singleton TimeController");
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }
    public static TimeController s
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<TimeController>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("No Existe Singleton TimeController");
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }


    #endregion

    #region Events
    public static event OnStartClockDelegate OnStartClock;
    #endregion

    #region Declaración de Variables y Propiedades
    [Header("Timer Settings")]
    [SerializeField]
    //Se trata de los minutos por donde empieza el Timer
    int minutes;

    [SerializeField]
    //se trata de los segundos por empieza el Timer
    float seconds;
    
    [SerializeField]
    //Esto es solo para DEBUG (eliminar en versión final)
    //Es para saber en que "Step" va el timer en el inspector
    //Tambien habria que eliminar la parte de "out" de timer.UpdateTimer()
#pragma warning disable 0414
    int debugStep;
#pragma warning restore 0414

    [SerializeField]
    //Indica si el tiempo va hacia atras o hacia adelante
    public bool decrement = true;

    [Header("When to finish (0 = infinite)")]
    
    [SerializeField]
    //Indica cuando debe detenerse el temporizador. Si es 0 es infinito
    float finishAtSecond = 0f;

    bool FirstUpdate = false;

    //Mi Timer
    public Timer timer;

    //Este es el tiempo ACTUAL.
    [SerializeField]
    float _currentTime;
    public float currentTime
    {
        get
        {
            return _currentTime;
        }

        set
        {
            
            if (value != _currentTime)
            {
                //Al cambiar currentTime, se actualiza el GUI
                UpdateGUI(value);
            }

            _currentTime = value;
        }
    }
    public float timeSpent
    {
        get
        {
            if (decrement)
            {
                return seconds - _currentTime;
            }else
            {
                return _currentTime - seconds;
            }

        }
    }
    
    [Header("GUI")]
    [SerializeField]
    Text TimerGUI;   

    #endregion

    #region Metodos
    void Update()
    {
        //FailSafe
        if (timer == null) return;

        //Sumamos o restamos al temporizado tanto como el tiempo que tarda
        //en ejecutarse el tiempo anterior
        if (decrement)
        {
            //menos
            currentTime = timer.UpdateTimer(-Time.deltaTime, out debugStep);
        }else
        {
            //mas
            currentTime = timer.UpdateTimer(Time.deltaTime, out debugStep);
        }
        /*
        if (!FirstUpdate)
        {
            FirstUpdate = true;
            if (OnStartClock != null) OnStartClock();
        }
        */

        //debug
        //Esto es para ver como corre el temporizador en el inspector
        //Las dos lineas siguientes son innecesarias.
        minutes = Mathf.FloorToInt(currentTime) / 60;
        //seconds = currentTime % 60f;
        //UpdateGUI();
    }
    /*
    void Awake()
    {
        //Llamamos al CONTRUCTOR de la clase Timer y le pasamos los datos
        //Entre ellos:
        // Tiempo , la acción a ejecutar al terminar el timer, y el momento final de timer
       timer = new Timer(seconds,() => { Debug.Log("Timer Finished"); }, finishAtSecond);
        //por defecto el timer esta parado, lo empezamos
       timer.StartTimer();
    }
    */
    public void SetTimer(float time, Action FinishAction, bool decrement, float finishAtSeconds =0f, bool StartTimer=true)
    {
        this.decrement = decrement;
        this.finishAtSecond = finishAtSeconds;
        seconds = time;
        timer = new Timer(seconds, FinishAction, finishAtSecond);
        if (StartTimer) timer.StartTimer();
        currentTime = timer.UpdateTimer(0f, out debugStep);
        if (OnStartClock != null) OnStartClock();


    }

    /// <summary>
    /// Actualiza la GUI (un text)
    /// </summary>
    /// <param name="thisTime"></param>
    void UpdateGUI(float currentSeconds)
    {
        //Formatea el Texto y lo presenta.        
        TimeSpan ts = TimeSpan.FromSeconds(currentSeconds);
        //string s = ts.Milliseconds.ToString();
        //s = s.Substring(0, 1);        
        string minutesString = ts.Minutes.ToString();
        if (minutesString.Length <= 1) minutesString = "0" + minutesString;
        string secondsString = ts.Seconds.ToString();
        if (secondsString.Length <= 1) secondsString = "0" + Mathf.FloorToInt(currentTime % 60f);


        TimerGUI.text = minutesString + ":" + secondsString;// + "." + s;

    }
    /// <summary>
    /// Devuelve el momento en el tiempo hacia adelante
    /// Si el reloj es por Decrementos será hacia abajo
    /// Si el reloj es por Incremewntos será hacia arriba
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public int AdvanceTimeXSeconds(int seconds)
    {
        int r = 0;
        if (decrement)
        {
            r = (Mathf.FloorToInt(currentTime) - seconds) - 1;
        }else
        {
            r = (Mathf.FloorToInt(currentTime) + seconds);
        }
        return r;
    }

    public int GiveCurrentStep()
    {
        int r = 0;
        if (decrement)
        {
            r = Mathf.FloorToInt(currentTime) - 1;
        }else
        {
            r = Mathf.FloorToInt(currentTime) + 1;
        }
        return r;
    }


    #endregion
}

//Aquí empieza la clase TIMER
public class Timer
{
    #region Declaración de Variables y Propiedades
    bool started; //si es false no corre el tiempo
    //Tiempo actual    
    float currentTime = 1f; //EN SEGUNDOS!
    //Esto es el ultimo "int" por que pasamos ocn nuestro float (seconds)
    int lastStep = 0;
    //esto es el "int" en el que el Timer se acaba
    int finishStep = 0;
    //La lista de acciones
    Dictionary<int, Action> ActionList;
    //la accion final
    Action finishedAction;
    #endregion
    #region constructores

    public Timer(float second, Action FinishedAction, float finSec = 0f)
    {
        
        if (second < 0f) second = 0f;       
       
        //iniciamos el diccionaio
        ActionList = new Dictionary<int, Action>();
        
        //transformamos minutos y segundos de fin de Timer en Step:
        finishStep = (int)finSec;

        //Asignamos lo demas
        finishedAction = FinishedAction;
        
        currentTime = second;
        //lastStep es igual al tiempo en el que estamos transformado en Step
        lastStep = (int)currentTime;
    }
   
    #endregion

    #region Metodos

    #region TimerUpdate
    /// <summary>
    /// Hace pasar el tiempo y devuelve el tiempo actual
    /// Si se le pasa 0 o "started" es == false, devuelve el tiempo actual sin cambiar nada
    /// </summary>
    /// <param name="deltaTime">el tiempo que pasa (negativo para bajar, positivo para subir</param>
    /// <returns>El tiempo actual</returns>
    public float UpdateTimer(float deltaTime, out int debugStep)
    {
        //Si este Timer está parado no hacemos nada
        if (started == true)
        {

            //Actualizamos los segundos
            currentTime += deltaTime;
                        

            //Comprobación de si lo segundos llegan a 0
            while (currentTime <= 0f)
            {                   
                TimerFinished();
                currentTime = 0f;                    
                break; 
            }                        

            //esta parte llama a las acciones registradas en el diccionario 
            //Calculamos en que "Step" estamos 
            int thisStep = 0;
            // miramos el int mas pequeño
            //Es decir, el ultimo "int" por el que pasamos.
            thisStep = Mathf.FloorToInt(currentTime);            
            
            
            while (thisStep < lastStep)
            {
                lastStep -= 1;
                if (ActionList.ContainsKey(lastStep)) ActionList[lastStep]();
            }
            while (thisStep > lastStep)
            {
                lastStep += 1;
                if (ActionList.ContainsKey(lastStep)) ActionList[lastStep]();
            }

            //Comprueba si ha llegado el final del timer
            int i = 0;
            if (deltaTime < 0f) i = -1;
            if (finishStep + i == lastStep ) {
                TimerFinished();
            }
            
        }
        //Linea de Debug, para ver luego en el inspector por el step en el que estamos
        debugStep = lastStep;

        //Generamos el RETURN
        return currentTime;

        
    }
    #endregion

    #region TimerFinished
    /// <summary>
    /// Tiempo finalizado, se ejecua la Acción "FinishedAction" si exite;
    /// </summary>
    void TimerFinished()
    {
        if (!started) return;
        if (finishedAction != null) finishedAction();
        started = false;
    }
    #endregion

    #region Action Add/Remove
    /// <summary>
    /// Añade acción.
    /// </summary>
    /// <param name="second">El segundo en el que ejecutar</param>
    /// <param name="action">La Accción</param>
    /// <returns>True si salió bien, False si ya existia</returns>
    public bool AddAction(int second, Action action)
    {
        if (ActionList.ContainsKey(second))
        {
            ActionList[second] += action;
        }
        else
        {
            ActionList.Add(second, action);
        }
        return true;
    }
    /// <summary>
    /// Quita una acción.
    /// </summary>
    /// <param name="second">El segundo en el que ejecutar</param>    
    /// <returns>True si salió bien, False si ya existia</returns>
    public bool RemoveAction(int second)
    {
        if (ActionList.ContainsKey(second)) return false;
        ActionList.Remove(second);
        return true;
    }
    #endregion

    #region Start/Stop Timer
    /// <summary>
    /// Empieza el Timer.
    /// </summary>
    public void StartTimer()
    {
        started = true;
    }
    /// <summary>
    /// Congela el Timer.
    /// </summary>
    public void StopTimer()
    {
        started = false;
    }
    #endregion
    #endregion
}






