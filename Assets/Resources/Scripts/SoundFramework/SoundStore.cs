using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// TRUCK CITY SOUNDSYSTEM FRAMEWORK
//////////////////////////////
/// Script secundario de sistema de sonido de truck city
/// 
//////////////////////////////

[RequireComponent(typeof(SoundSystem))]
public class SoundStore : MonoBehaviour {
    #region Singleton
    private static SoundStore s_singleton = null;

    public static SoundStore singleton
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<SoundStore>();
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
    public static SoundStore s
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<SoundStore>();
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
    #region Reference
    SoundSystem mySoundSystem;
    void Awake()
    {
        DoReferences();
    }
    void DoReferences()
    {
        mySoundSystem = GetComponent<SoundSystem>();
    }
    
    #endregion


    /// <summary>
    /// Almacen de Sonidos y su respectivo "Alias"
    /// </summary>
    [SerializeField]
    List<SoundParity> Sounds;
    AudioClip lastPlayedCLip;


    /// <summary>
    /// Reproduce un clip de audio con un "Alias"
    /// En caso de haber mas de uno con el mismo Alias los elige aleatoriamente
    /// </summary>
    /// <param name="alias">El Alias</param>
    /// <param name="delay">(Por defecto = 0f) Demora</param>
    /// <param name="volume">(Por defecto = 1f) Volumen</param>
    /// <param name="fadein">(Por defecto = false) Si deseamos "Fade in"</param>
    /// <param name="fadeintime">(Por defecto = 0.1f) Duración del "Fade in"</param>
    /// <param name="fadeout">(Por defecto = false) Si deseamos Fade Out del sonido anterior (Solo en caso de que no queden AudioSources libres)</param>
    /// <param name="fadeouttime">(Po defecto = 0.1f) Duración del "Fade out"</param>
    public void PlaySoundByAlias(string alias, float delay = 0f, float volume = 1f, bool fadein = false, float fadeintime = 0.1f, bool fadeout = false, float fadeouttime = 0.1f, Action OnFinish = null)
    {
        if (mySoundSystem == null) DoReferences();

        //se elige el clip
        AudioClip ac;
        List<SoundParity> aliasSounds = Sounds.FindAll(x => x.Alias == alias);
        do
        {
            ac = aliasSounds[aliasSounds.RandomIndex()].clip;
        } while (ac == lastPlayedCLip && aliasSounds.Count > 1); //Se evita que seá el mismo

        //guardamos el ultimo clip que ha sonado
        lastPlayedCLip = ac;

        //Reproducimos el sonido
        mySoundSystem.PlaySound(ac, delay, volume, fadein, fadeintime, fadeout, fadeouttime,OnFinish);
    }


    /// <summary>
    /// Averigua si algun elemento del Alias está siendo reproducido ahora
    /// </summary>
    /// <param name="Alias">El Alias</param>
    /// <returns>El Index del Audio Source que lo reproduce</returns>
    public int AliasIsPlaying(string Alias)
    {
        List<SoundParity> list = Sounds.FindAll(x => x.Alias == Alias);
        List<AudioSource> sources = mySoundSystem.AudioSources;
        for (int i = 0; i < list.Count; i++)
        {
            for (int e = 0; e < sources.Count; e++)
            {
                if (sources[e].isPlaying && sources[e].clip == list[i].clip)
                {
                    return e;
                }
            }

        }

        return -1;
    }




    /// <summary>
    /// Para de reproducir todos los AudioSources que estén reproduciendo el Alias indicado
    /// </summary>
    /// <param name="Alias"></param>
    public void StopAlias(string Alias)
    {
        int i = AliasIsPlaying(Alias);
        while(i > -1)
        {
            mySoundSystem.AudioSources[i].Stop();
        }
    }


}

