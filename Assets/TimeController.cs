using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class TimeController : MonoBehaviour {
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
    bool decrement = true;

    [Header("When to finish (0 = infinite)")]

    [SerializeField]
    //Indica cuando debe detenerse el temporizador. Si es 0 es infinito
    int finishAtMinutes;
    [SerializeField]
    //Indica cuando debe detenerse el temporizador. Si es 0 es infinito
    float finishAtSeconds;

    //Mi Timer
    public Timer timer;
    //Este es el tiempo ACTUAL.
    //TimeStep contiene: Minutos(int) y segundos(float) (ver mas abajo)
    TimeStep _currentTime;
    public TimeStep currentTime
    {
        get
        {
            return _currentTime;
        }

        set
        {
            if (_currentTime == null) _currentTime = new TimeStep(minutes, seconds);
            if (_currentTime.minutes != value.minutes || _currentTime.seconds != value.seconds)
            {
                UpdateGUI(value);
            }

            _currentTime = value;
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

        
        //debug
        //Esto es para ver como corre el temporizador en el inspector
        //Las dos lineas siguientes son innecesarias.
        minutes = currentTime.minutes;
        seconds = currentTime.seconds;
    }

    void Awake()
    {
        //Llamamos al CONTRUCTOR de la clase Timer y le pasamos los datos
        //Entre ellos:
        // Tiempo , la acción a ejecutar al terminar el timer, y el momento final de timer
       timer = new Timer(minutes, seconds,() => { Debug.Log("Timer Finished"); }, finishAtMinutes, finishAtSeconds);
        //por defecto el timer esta parado, lo empezamos
       timer.StartTimer();
    }
    /// <summary>
    /// Actualiza la GUI (un text)
    /// </summary>
    /// <param name="thisTime"></param>
    void UpdateGUI(TimeStep thisTime)
    {
        //Formatea el Texto y lo presenta.
        double totalMiliSeconds = (thisTime.minutes * 60000) + (int)(seconds *1000);
        TimeSpan ts = TimeSpan.FromMilliseconds(totalMiliSeconds);
        //string s = ts.Milliseconds.ToString();
        //s = s.Substring(0, 1);
        string minutesString = ts.Minutes.ToString();
        if (minutesString.Length <= 1) minutesString = "0" + minutesString;
        string secondsString = ts.Seconds.ToString();
        if (secondsString.Length <= 1) secondsString = "0" + secondsString;


        TimerGUI.text = minutesString + ":" + secondsString;// + "." + s;

    }

    #endregion
}

//Aquí empieza la clase TIMER
public class Timer
{
    #region Declaración de Variables y Propiedades
    bool started; //si es false no corre el tiempo
    //Tiempo actual
    int minutes = 1;
    float seconds = 1f;    
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

    public Timer(int minute, float second, Action FinishedAction, int finMin = 0, float finSec = 0f)
    {
        if (minute < 0) minute = 0;
        if (second < 0f) second = 0f;
        //Si se alimenta el constructor con mas de 60 segundos:
        while (second >= 60f)
        {
            second -= 60f;
            minute += 1;
        }
        //iniciamos el diccionaio
        ActionList = new Dictionary<int, Action>();
        
        //transformamos minutos y segundos de fin de Timer en Step:
        finishStep = (finMin*60) + (int)finSec;

        //Asignamos lo demas
        finishedAction = FinishedAction;
        minutes = minute;
        seconds = second;
        //lastStep es igual al tiempo en el que estamos transformado en Step
        lastStep = (int)seconds + (60 * minutes);
    }
    public Timer(float second, Action action, float finSec)
    {//otro constructor
        new Timer(0, second, action, 0,finSec);
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
    public TimeStep UpdateTimer(float deltaTime, out int debugStep)
    {
        //Si este Timer está parado no hacemos nada
        if (started == true)
        {

            //Actualizamos los segundos
            seconds += deltaTime;

            //Comprobación de si los segundos llegan a 60
            while (seconds >= 60f)
            {
                minutes += 1;
                seconds -= 60f;
            }

            //Comprobación de si lo segundos llegan a 0
            while (seconds <= 0f)
            {
                minutes -= 1;
                if (minutes < 0f) //Si llega a 0 segundos , 0 minutos
                {                    
                    TimerFinished();
                    seconds = 0f;
                    minutes = 0;
                    break;
                }
                else
                {
                    seconds += 60f;
                }
            }                        

            //esta parte llama a las acciones registradas en el diccionario 
            //Calculamos en que "Step" estamos 
            int thisStep = 0;
            //Si subimos, miramos el Int mas pequeño, si bajamos el mas grande
            //Es decir, el ultimo "int" por el que pasamos.
            if (deltaTime >= 0f) thisStep = Mathf.FloorToInt(seconds) + (60 * minutes);            
            if (deltaTime <= 0f) thisStep = Mathf.CeilToInt(seconds) + (60 * minutes);
            
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
            if (finishStep == lastStep) TimerFinished();
            
        }
        //Linea de Debug, para ver luego en el inspector por el step en el que estamos
        debugStep = lastStep;

        //Generamos el RETURN
        TimeStep ts = new TimeStep(minutes, seconds);

        return ts;
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
        if (ActionList.ContainsKey(second)) return false;
        ActionList.Add(second, action);
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

public class TimeStep
{
    #region Declaración de Variables y Propiedades
    public int minutes;
    public float seconds;
    #endregion

    #region Constructors
    public TimeStep(int minute, float second)
    {
        minutes = minute;
        seconds = second;
    }
    public TimeStep() //por defecto
    {
        minutes = 0;
        seconds = 0f;
    }    
    #endregion
}

