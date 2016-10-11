using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// TRUCK CITY SOUNDSYSTEM FRAMEWORK
//////////////////////////////
/// Script principal del sistema de sonido de Truck City.
/// [Requiere uno o mas Audio Sources]
//////////////////////////////

[RequireComponent(typeof(AudioSource))]
public class SoundSystem : Singleton<SoundSystem>
{
    #region References
    List<AudioSource> _audioSources;
    public List<AudioSource> AudioSources
    {
        get
        {
            if (_audioSources == null) {
                DoReferences();
            }

            return _audioSources;
        }        
    }

    void Awake()
    {
        base.Awake();
        DoReferences();
        MusicStore.s.PlayMusicByAlias("Menu", 1.5f, 1f, true, 5f);
    }
    void DoReferences()
    {
        _audioSources = GetComponents<AudioSource>().ToList<AudioSource>();
        AudioSources[0].loop = true; //Este es la musica!
    }
    #endregion





    [Header("Location of Files")]
    [SerializeField]
    string MusicPath;
    [SerializeField]
    string SoundsPath;

    //Internal
    bool cancelFadeInMusic = false;
    bool cancelFadeOutMusic = false;
    

    #region public Play

    #region Sounds
    /// <summary>
    /// Reproduce el Sonido especificado
    /// </summary>
    /// <param name="soundPath">El lugar relativo de la localización de Sonidos de Resources de Truck City</param>
    /// <param name="delay">(Defecto = 0f)Demora para empezar</param>
    /// <param name="volume">(Defecto = 1f)Volumen</param>
    /// <param name="fadein">(Defecto = false)Si se desea "fade in"</param>
    /// <param name="fadeintime">(Defecto = 0.1f)Duración del "fade in"</param>
    /// <param name="fadeout">(Defecto = false)Si se desea "fade out" del sonido anterior (solo en caso de Audio Sources llenos)</param>
    /// <param name="fadeoutTime">(Defecto = 0.1f)Duración del "fade out"</param>
    public void PlaySound(string soundPath, float delay = 0f, float volume = 1f, bool fadein = false,float fadeintime = 0.1f, bool fadeout = false, float fadeoutTime = 0.1f, Action OnFinish = null)
    {
        StartPlaying(SoundsPath, soundPath, delay, volume, false, fadein, fadeintime, fadeout, fadeoutTime, OnFinish);
    }
    /// <summary>
    /// Reproduce el Sonido especificado
    /// </summary>
    /// <param name="clip">El AudioClip a reproducir</param>
    /// <param name="delay">(Defecto = 0f)Demora para empezar</param>
    /// <param name="volume">(Defecto = 1f)Volumen</param>
    /// <param name="fadein">(Defecto = false)Si se desea "fade in"</param>
    /// <param name="fadeintime">(Defecto = 0.1f)Duración del "fade in"</param>
    /// <param name="fadeout">(Defecto = false)Si se desea "fade out" del sonido anterior (solo en caso de Audio Sources llenos)</param>
    /// <param name="fadeoutTime">(Defecto = 0.1f)Duración del "fade out"</param>
    public void PlaySound(AudioClip clip, float delay = 0f, float volume = 1f, bool fadein = false, float fadeintime = 0.1f, bool fadeout = false, float fadeoutTime = 0.1f, Action OnFinish = null)
    {
        StartPlaying(clip, delay, volume, false, fadein, fadeintime, fadeout, fadeoutTime,OnFinish);
    }
    #endregion

    #region Music
    /// <summary>
    /// Reproduce el Sonido especificado
    /// </summary>
    /// <param name="soundPath">El lugar relativo de la localización de Musica de Resources de Truck City</param>
    /// <param name="delay">(Defecto = 0f)Demora para empezar</param>
    /// <param name="volume">(Defecto = 1f)Volumen</param>
    /// <param name="fadein">(Defecto = false)Si se desea "fade in"</param>
    /// <param name="fadeintime">(Defecto = 2f)Duración del "fade in"</param>
    /// <param name="fadeout">(Defecto = false)Si se desea "fade out" de la canción anterior</param>
    /// <param name="fadeoutTime">(Defecto = 2f)Duración del "fade out"</param>
    public void PlayMusic(string musicPath, float delay = 0f, float volume = 1f, bool fadein = false, float fadeintime = 2f, bool fadeout = false, float fadeoutTime = 2f, Action OnFinish = null)
    {
        StartPlaying(MusicPath, musicPath, delay, volume, true, fadein, fadeintime, fadeout, fadeoutTime, OnFinish);
    }

    /// <summary>
    /// Reproduce el Sonido especificado
    /// </summary>
    /// <param name="soundPath">El lugar relativo de la localización de Musica de Resources de Truck City</param>
    /// <param name="delay">(Defecto = 0f)Demora para empezar</param>
    /// <param name="volume">(Defecto = 1f)Volumen</param>
    /// <param name="fadein">(Defecto = false)Si se desea "fade in"</param>
    /// <param name="fadeintime">(Defecto = 2f)Duración del "fade in"</param>
    /// <param name="fadeout">(Defecto = false)Si se desea "fade out" de la canción anterior</param>
    /// <param name="fadeoutTime">(Defecto = 2f)Duración del "fade out"</param>
    public void PlayMusic(AudioClip clip, float delay = 0f, float volume = 1f, bool fadein = false, float fadeintime = 2f, bool fadeout = false, float fadeoutTime = 2f, Action OnFinish = null)
    {
        StartPlaying(clip, delay, volume, true, fadein, fadeintime, fadeout, fadeoutTime,OnFinish);
    }



    #endregion




    #endregion

    #region StartPlaying


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
    void StartPlaying(string path, string name, float delay = 0f, float volume = 1f, bool Music = false, bool fadein = false, float fadeintime = 2f, bool fadeout = false, float fadeouttime = 2f, Action OnFinish = null)
    {
        StartCoroutine(_StartPlaying(LoadClip(path,name), delay, volume, Music, fadein, fadeintime, fadeout, fadeouttime, OnFinish));
    }
    /// <summary>
    /// Empieza a sonar un clip. Si se trata de Musica lo hace en SU audiosource
    /// especifico, si no lo es lo hace en uno libre.
    /// </summary>
    /// <param name="clip">El Clip a sonar</param>    
    /// <param name="delay">(opcional)Tiempo de espera hasta iniciar el sonido(por defecto = 0f)</param>
    /// <param name="volume">(opcional)Volumen (maximo) (por defecto = 1f)</param>
    /// <param name="Music">(opcional) Si es Musica (por defecto = false)</param>
    /// <param name="fadein">(opcional) Si el clip entra con fadein (por defecto = false)</param>
    /// <param name="fadeintime">(opcional) Si el clip entra con fadein, cuanto tarda en llegar al maximo, en segundos (por defecto = 2f)</param>
    /// <param name="fadeout">(opcinal) Si hubiera un clip anterior sonando en el mismo AudioSource, si queremos que el sonido anterior haga FadeOut(por defecto =false) </param>
    /// <param name="fadeouttime">(opcional) Si el clip anterior sale con FadeOut, cuanto tarda en llegar a 0, en segundos (por defecto = 2f)</param>
    void StartPlaying(AudioClip clip, float delay = 0f, float volume = 1f, bool Music = false, bool fadein = false, float fadeintime = 2f, bool fadeout = false, float fadeouttime = 2f, Action OnFinish = null)
    {
        StartCoroutine(_StartPlaying(clip, delay, volume, Music, fadein, fadeintime, fadeout, fadeouttime, OnFinish));
    }
    IEnumerator _StartPlaying(AudioClip clip, float delay = 0f, float volume = 1f, bool Music = false, bool fadein = false, float fadeintime = 2f, bool fadeout = false, float fadeouttime = 2f, Action OnFinish = null)
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
            AudioSources[i].PlayOneShot(clip);
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
                    if (i != 0) AudioSources[i].PlayOneShot(clip);
                    if (i == 0) { AudioSources[i].clip = clip; AudioSources[i].Play(); }
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
                if (i != 0) AudioSources[i].PlayOneShot(clip);
                if (i == 0) { AudioSources[i].clip = clip; AudioSources[i].Play(); }
                if (fadein) VolumeFadeIn(i, fadeintime, true, volume);
            }





        }

        //La Action OnFinish
        if (OnFinish != null)
        {
            while (AudioSources[i].isPlaying)
            {
                yield return Delay(0.1f);
            }
            OnFinish();
        }
        


    }


    #endregion

    #region public Stop (usually Music)
    /// <summary>
    /// Fades and Stops Playing the music
    /// </summary>
    /// <param name="fadeouttime"></param>
    public void FadeOutMusic(float fadeouttime = 2f, Action Finish = null)
    {
        if (Finish == null)
        {
            Finish = () => { cancelFadeInMusic = false; };
        }else
        {
            Finish += () => { cancelFadeInMusic = false; };
        }



        cancelFadeInMusic = true;
        VolumeFadeOut(0, fadeouttime,0.1f,true,Finish);
    }


    #endregion

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
    /// Aumenta el volumen, poco a poco el de un AudioSource
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

    //Funcion interna de VolumeFadeOut y VolumeFadeIn
    IEnumerator _VolumeFade(int AudioSourceIndex, float time, float toVolume, float step = 0.1f, bool stopPlaying = true, Action OnFinish = null)
    {        
        float currentTime = 0f;     
                  
        //Desde donde
        float fromVolume = AudioSources[AudioSourceIndex].volume;

        //sign indica si es positivo o negativo (suma o resta) despues
        float sign = 1;
        if (fromVolume > toVolume) sign = -1;

        //Bucle principal
        while (currentTime < time 
            && 
            (( AudioSourceIndex != 0  ) 
                ||
                (AudioSourceIndex == 0  
                    && ( sign ==-1 && !cancelFadeOutMusic
                        || sign == 1 && !cancelFadeInMusic))))
        {
            //Tardara "time" tiempo... y se ejecuta cada "step" tiempo
            yield return StartCoroutine(Delay(step));

            //Bajar o subir volumen principal. ++///////////////////////////////////////Volumen + (Signo+/-) * ((Diferencia total) * (tiempo total / tiempo desde la ultima ejecución))
            AudioSources[AudioSourceIndex].volume = AudioSources[AudioSourceIndex].volume + ( sign*(Mathf.Abs(fromVolume-toVolume) / (time / step)));

            //Sumamos el tiempo que ha pasado
            currentTime += step;
        }
        //Cuando termina el bucle ajustamos por ultima vez el volumen, para evitar incomodos valores decimales que puedan quedar al final
        //(Pero solo si el bucle no fue interrumpido!)
        if ((AudioSourceIndex != 0)||
                (AudioSourceIndex == 0
                    && (sign == -1 && !cancelFadeOutMusic
                        || sign == 1 && !cancelFadeInMusic)))  AudioSources[AudioSourceIndex].volume = toVolume;

        //Al terminar determina se se apaga el sonido
        if (stopPlaying) AudioSources[AudioSourceIndex].Stop();

        //Lanzamos el callback
        if (OnFinish != null) OnFinish();
    }

    //Funcion interna de _VolumeFade
    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    #endregion

    #region utils

    /// <summary>
    /// Devuelve "True" si este clip está soando ahora
    /// </summary>
    /// <param name="clip">Este clip</param>
    /// <returns></returns>
    public bool ThisClipIsPlaying(AudioClip clip)
    {
        return (AudioSources.Find(x => x.isPlaying && x.clip == clip));
    }

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
