using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TimeController : MonoBehaviour {
    [Header("Timer Settings")]
    [SerializeField]
    int minutes;
    [SerializeField]
    float seconds;
    [SerializeField]
    int debugStep;
    [SerializeField]
    bool decrement = true;
    [Header("If !decrement. When to finish (0 = infinite)")]    
    [SerializeField]
    int finishAtMinutes;
    [SerializeField]
    float finishAtSeconds;


    public Timer timer;
    public TimeStep currentTime;

    void Update()
    {
        if (timer == null) return;
        if (decrement)
        {
            currentTime = timer.UpdateTimer(-Time.deltaTime, out debugStep);
        }else
        {
            currentTime = timer.UpdateTimer(Time.deltaTime, out debugStep);
        }

        //debug
        minutes = currentTime.minutes;
        seconds = currentTime.seconds;
    }




    void Awake()
    {
       timer = new Timer(minutes, seconds,() => { Debug.Log("Timer Finished"); }, finishAtMinutes, finishAtSeconds);
       timer.StartTimer();


    }



}









public class Timer
{
    bool started;
    int minutes = 1;
    float seconds = 1f;    
    int lastStep = 0;
    int finishStep = 0;
    Dictionary<int, Action> ActionList;
    Action finishedAction;

    #region constructores
    public Timer(int minute, float second, Action FinishedAction, int finMin = 0, float finSec = 0f)
    {
        if (minute < 0) minute = 0;
        if (second < 0f) second = 0f;

        while (second >= 60f)
        {
            second -= 60f;
            minute += 1;
        }
        ActionList = new Dictionary<int, Action>();

        finishedAction = FinishedAction;
        finishStep = (finMin*60) + (int)finSec;
        
        minutes = minute;
        seconds = second;
        lastStep = Mathf.FloorToInt(seconds) + (60 * minutes);
    }
    public Timer(float second, Action action, float finSec)
    {
        new Timer(0, second, action, 0,finSec);
    }
    #endregion

    #region TimerUpdate
    /// <summary>
    /// Hace pasar el tiempo y devuelve el tiempo actual
    /// Si se le pasa 0 o "started" es == false, devuelve el tiempo actual sin cambiar nada
    /// </summary>
    /// <param name="deltaTime">el tiempo que pasa (negativo para bajar, positivo para subir</param>
    /// <returns></returns>
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
                if (minutes < 0f)
                {
                    //Si llega a 0,0
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
            //Es decir, el ultimo "int" por que pasamos.
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
        debugStep = lastStep;

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

}


public class TimeStep
{
    public int minutes;
    public float seconds;

    public TimeStep(int minute, float second)
    {
        minutes = minute;
        seconds = second;
    }
    public TimeStep()
    {
        minutes = 0;
        seconds = 0f;
    }

}

