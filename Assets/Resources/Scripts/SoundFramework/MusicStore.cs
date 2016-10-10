using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// TRUCK CITY SOUNDSYSTEM FRAMEWORK
//////////////////////////////
/// Script secundario de sistema de sonido de truck city
/// 
//////////////////////////////

[RequireComponent(typeof(SoundSystem))]
public class MusicStore : MonoBehaviour
{
    #region Singleton
    private static MusicStore s_singleton = null;

    public static MusicStore singleton
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<MusicStore>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("No Existe Singleton MusicStore");
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }
    public static MusicStore s
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<MusicStore>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("No Existe Singleton MusicStore");
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
    List<SoundParity> Songs;
    AudioClip lastPlayedCLip;


    /// <summary>
    /// Reproduce una canción guardada en este almacen con un "Alias"
    /// En caso de haber mas de una con el mismo Alias las elige aleatoriamente
    /// </summary>
    /// <param name="alias">El Alias</param>
    /// <param name="delay">(Por defecto = 0f) Demora</param>
    /// <param name="volume">(Por defecto = 1f) Volumen</param>
    /// <param name="fadein">(Por defecto = false) Si deseamos "Fade in"</param>
    /// <param name="fadeintime">(Por defecto = 2f) Duración del "Fade in"</param>
    /// <param name="fadeout">(Por defecto = false) Si deseamos Fade Out del sonido anterior (Solo en caso de que no queden AudioSources libres)</param>
    /// <param name="fadeouttime">(Po defecto = 2f) Duración del "Fade out"</param>
    public void PlayMusicByAlias(string alias, float delay = 0f, float volume = 1f, bool fadein = false, float fadeintime = 2f, bool fadeout = false, float fadeouttime = 2f)
    {
        if (mySoundSystem == null) DoReferences();

        //se elige el clip
        AudioClip ac;
        List<SoundParity> aliasSounds = Songs.FindAll(x => x.Alias == alias);
        do
        {
            ac = aliasSounds[aliasSounds.RandomIndex()].clip;
        } while (ac == lastPlayedCLip && aliasSounds.Count > 1); //Se evita que seá el mismo

        //guardamos el ultimo clip que ha sonado
        lastPlayedCLip = ac;

        //Reproducimos el sonido
        mySoundSystem.PlayMusic(ac, delay, volume, fadein, fadeintime, fadeout, fadeouttime);
    }
}