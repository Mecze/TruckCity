using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SoundSystem : Singleton<SoundSystem>
{
    [Header("Audio Sources Reference")]
    [SerializeField]
    List<AudioSource> AudioSources;


    [Header("Location of Files")]
    [SerializeField]
    string MusicPath;
    [SerializeField]
    string SoundsPath;


    [Header("List of Songs (Music)")]
    [SerializeField]
    List<string> Songs;
    

    public override void Awake()
    {
        base.Awake();
        StartPlaying(MusicPath, Songs[0], 1.5f, 1f, true, true, 10f);
        //AudioSources[0].clip = LoadClip(MusicPath, Songs[0]);
        //AudioSources[0].Play();
    }







    /*
    public void PlayMusic()
    {
        if (AudioSources[0].isPlaying)
        {
            AudioSources[0].clip
        }else
        {

        }
    }
    */
    /// <summary>
    /// Empieza a sonar un clip. Si se trata de Musica lo hace en SU audiosource
    /// especifico, si no lo es lo hace en uno libre.
    /// </summary>
    /// <param name="path">Lugar del clip</param>
    /// <param name="name">nombre del clip</param>
    /// <param name="delay">(opcional)Tiempo de espera hasta iniciar el sonido(por defecto = 0f)</param>
    /// <param name="volume">(opcional)Volumen (maximo) (por defecto = 1f)</param>
    /// <param name="Music">(opcional) Si es Musica (por defecto = false)</param>
    /// <param name="fadein">(opcional) Si el clip entra con fadein (por defecto = false)</param>
    /// <param name="fadeintime">(opcional) Si el clip entra con fadein, cuanto tarda en llegar al maximo, en segundos (por defecto = 2f)</param>
    /// <param name="fadeout">(opcinal) Si hubiera un clip anterior sonando en el mismo AudioSource, si queremos que el sonido anterior haga FadeOut(por defecto =false) </param>
    /// <param name="fadeouttime">(opcional) Si el clip anterior sale con FadeOut, cuanto tarda en llegar a 0, en segundos (por defecto = 2f)</param>
    void StartPlaying(string path, string name, float delay = 0f, float volume = 1f, bool Music = false, bool fadein = false, float fadeintime = 2f, bool fadeout = false, float fadeouttime = 2f)
    {
        StartCoroutine(_StartPlaying(path, name, delay, volume, Music, fadein, fadeintime, fadeout, fadeouttime));
    }
    IEnumerator _StartPlaying(string path, string name, float delay = 0f, float volume = 1f, bool Music = false, bool fadein = false, float fadeintime = 2f, bool fadeout = false, float fadeouttime = 2f)
    {
        //Esperamos X segundos
        yield return new WaitForSeconds(delay);

        int i = 0; //Si es Musica, i ser� igual a 0;
        if (!Music)
        {//Si no es musica hayamos un audio source correcto para nuestro sonido
            i = GetFreeAudioSource(); //Hayamos el AudioSource apropiado
        }
        if (i > 0)
        {
            //Se encontr� un AudioSource Libre:                
            //-----------
            if (fadein) AudioSources[i].volume = 0f;
            if (!fadein) AudioSources[i].volume = volume;
            AudioSources[i].PlayOneShot(LoadClip(path, name));
            if (fadein) VolumeFadeIn(i, fadeintime, true, volume);
            //-----------
        }
        else
        {
            //Todos los AudioSources estan Ocupados � i == 0 (es Musica)
            //-----------

            if (i < 0) i = Mathf.Abs(i);//Puesto que i es negativo, sacamos el abs

            //Si el usuario quiere FadeOut del sonido anterior:
            if (fadeout)
            {   //Se llama al FadeOut y se le pasa como "Action" que al terminar el
                //FadeOut ejecute el nuevo sonido
                VolumeFadeOut(i, fadeouttime, 0.1f, true, () =>
                {
                    //ACTION---
                    if (fadein) AudioSources[i].volume = 0f;
                    if (!fadein) AudioSources[i].volume = volume;
                    if (i != 0) AudioSources[i].PlayOneShot(LoadClip(path, name));
                    if (i == 0) { AudioSources[i].clip = LoadClip(path, name); AudioSources[i].Play(); }
                    if (fadein) VolumeFadeIn(i, fadeintime, true, volume);
                    //---ACTION!!
                });


            }
            else
            {
                //Si el usuario No quiere Fade Out del sonido anterior
                AudioSources[i].Stop();
                if (fadein) AudioSources[i].volume = 0f;
                if (!fadein) AudioSources[i].volume = volume;
                if (i != 0) AudioSources[i].PlayOneShot(LoadClip(path, name));
                if (i == 0) { AudioSources[i].clip = LoadClip(path, name); AudioSources[i].Play(); }
                if (fadein) VolumeFadeIn(i, fadeintime, true, volume);
            }





        }
    }









    #region VolumeFadeOut & In
    /// <summary>
    /// Silencia, poco a poco el volumen de un AudioSource
    /// Este metodo presupone que el limite inferior ser� 0.
    /// </summary>
    /// <param name="AudioSourceIndex">El indice del AudioSource de la Lista</param>
    /// <param name="time">El tiempo de fade out</param>
    /// <param name="step">(opcional) Cada cuanto se actualiza (por defecto = 0.1f)</param>
    /// <param name="stopPlaying">(opcional) Hace STOP al terminar (por defecto = true)</param>
    /// <param name="OnFinish">(opcional)Ejecutar algo cuando termine esto? usar una Accion aqu� (por defecto = nada)</param>
    void VolumeFadeOut(int AudioSourceIndex, float time, float step = 0.1f, bool stopPlaying = true, Action OnFinish = null)
    {
        StartCoroutine(_VolumeFade(AudioSourceIndex, time, 0f, step, stopPlaying, OnFinish));
        
    }
    /// <summary>
    /// Silencia, poco a poco el volumen de un AudioSource
    /// </summary>
    /// <param name="AudioSourceIndex">El indice del AudioSource de la Lista</param>
    /// <param name="time">El tiempo de fade out</param>
    /// <param name="fromZero">(opcional)Desde 0f. Si es false se empieza desde el volumen actual del AudioSource. Si es true el AudioSource se pone a 0f al principio (Por defecto = true)</param>
    /// <param name= "max">(opcinal)Hasta "donde" debe llegar (por defecto = 1f)</param> 
    /// <param name="step">(opcional) Cada cuanto se actualiza (por defecto = 0.1f)</param>
    /// <param name="stopPlaying">(opcional) Hace STOP al terminar (por defecto = true)</param>
    /// <param name="OnFinish">(opcional)Ejecutar algo cuando termine esto? usar una Accion aqu� (por defecto = nada)</param>
    void VolumeFadeIn(int AudioSourceIndex, float time, bool fromZero = true, float max = 1f, float step = 0.1f, bool stopPlaying = false, Action OnFinish = null)
    {
        if (fromZero) AudioSources[AudioSourceIndex].volume = 0f;
        StartCoroutine(_VolumeFade(AudioSourceIndex, time, max, step, stopPlaying, OnFinish));
    }
    IEnumerator _VolumeFade(int AudioSourceIndex, float time, float toVolume, float step = 0.1f, bool stopPlaying = true, Action OnFinish = null)
    {        
        float currentTime = 0f;               
        //Desde donde
        float fromVolume = AudioSources[AudioSourceIndex].volume;
        //sign indica si es positivo o negativo (suma o resta) despues
        float sign = 1;
        if (fromVolume > toVolume) sign = -1;

        //Bucle principal
        while (currentTime < time)
        {
            //Tardar� "time" tiempo... y se ejecuta cada "step" tiempo
            yield return StartCoroutine(Delay(step));
            AudioSources[AudioSourceIndex].volume = AudioSources[AudioSourceIndex].volume + ( sign*(Mathf.Abs(fromVolume-toVolume) / (time / step)));
            currentTime += step;
        }
        AudioSources[AudioSourceIndex].volume = toVolume;
        //Al terminar determina se se apaga el sonido
        if (stopPlaying) AudioSources[AudioSourceIndex].Stop();
        //Lanzamos el callback
        if (OnFinish != null) OnFinish();
    }

    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    #endregion

   




    #region utils
    /// <summary>
    /// Devuelve el indice del primer AudioSource libre.
    /// Si estan todos ocupados devuelve el negativo (-1,-2..) del indice
    /// del AudioSource mas cercano a terminar
    /// </summary>
    /// <returns></returns>
    int GetFreeAudioSource()
    {
        for (int i = 1; i < AudioSources.Count; i++)
        {
            if (!AudioSources[i].isPlaying)
            {
                return i;
                break;
            }
            
        }
        int winner = 0;
        float timewinner = 0f;
        for (int i = 1; i < AudioSources.Count; i++)
        {
            float e = TimeRemaining(i);
            if (e < timewinner || winner == 0)
            {
                winner = -i;
                timewinner = e;
            }
        }
        return winner;


    }
    /// <summary>
    /// Devuelve el tiempo restante del audio que suena en un AudioSource
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    float TimeRemaining(int index)
    {
        if (AudioSources[index].isPlaying) {
            return AudioSources[index].clip.length - AudioSources[index].time;
        }else
        {
            return 0f;
        }
    }

    /// <summary>
    /// Carga el clip desde RESOURCES.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    AudioClip LoadClip(string path, string name)
    {
        return Resources.Load<AudioClip>(path + name);
    }

    #endregion






}
