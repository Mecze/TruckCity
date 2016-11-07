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
            if (GameConfig.s != null) GameConfig.s.MusicState = value;
            SetSprite(_state);
            
        }
    }

    public bool Clickable
    {
        get
        {
            return clickable;
        }

        set
        {
            clickable = value;
        }
    }

    private bool clickable = true;


    void SetSprite(bool set)
    {
        if (set)
        {
            if (GameConfig.s != null)
            {
                mySprite.spriteName = GameConfig.s.MusicSprite;
            }else
            {
                mySprite.spriteName = "ButtonIcons_BGM";
            }

        }
        else
        {
            if (GameConfig.s != null)
            {
                
                mySprite.spriteName = GameConfig.s.NoMusicSprite;
               // Debug.Log("saf");
            }else
            {
                mySprite.spriteName = "ButtonIcons_BGMOff";
            }
                
        }
    }

    void Awake()
    {
        if (GameConfig.s != null)
        {
            _state = GameConfig.s.MusicState;
        }else
        {
            _state = false;
        }
    
        SetSprite(_state);
    }

    public void Click()
    {
        Debug.Log("ho!");
        if (Clickable)state = !state;
    }



}
