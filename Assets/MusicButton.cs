using UnityEngine;
using System.Collections;

public class MusicButton : MonoBehaviour {
    #region Singleton
    private static MusicButton s_singleton = null;

    public static MusicButton singleton
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<MusicButton>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("No Existe Singleton MusicButton");
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }
    public static MusicButton s
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<MusicButton>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                //Debug.LogError("No Existe Singleton MusicButton");
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }


    #endregion

    [SerializeField]
    UISprite mySprite;

    bool _state = true;
    public bool state
    {
        get
        {
            return _state;
        }

        set
        {
            _state = value;
            GameConfig.s.MusicState = value;
            SetSprite(value);
        }
    }
    public bool Clickable = true;


    void SetSprite(bool set)
    {
        if (set)
        {
            mySprite.spriteName = GameConfig.s.MusicSprite;

        }
        else
        {
            mySprite.spriteName = GameConfig.s.NoMusicSprite;
        }
    }

    void Awake()
    {
        _state = GameConfig.s.MusicState;
        SetSprite(GameConfig.s.MusicState);
    }

    public void Click()
    {
        if (Clickable)state = !state;
    }



}
