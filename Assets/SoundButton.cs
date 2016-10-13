using UnityEngine;
using System.Collections;

public class SoundButton : MonoBehaviour {
    #region Singleton
    private static SoundButton s_singleton = null;

    public static SoundButton singleton
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<SoundButton>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("No Existe Singleton SoundButton");
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }
    public static SoundButton s
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<SoundButton>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("No Existe Singleton SoundButton");
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
            GameConfig.s.SoundState = value;
            SetSprite(value);
        }
    }


    void SetSprite(bool set)
    {
        if (set)
        {
            mySprite.spriteName = GameConfig.s.SoundSprite;

        }
        else
        {
            mySprite.spriteName = GameConfig.s.NoSoundSprite;
        }
    }


    void Awake()
    {
        _state = GameConfig.s.SoundState;
        SetSprite(GameConfig.s.SoundState);
    }

    public void Click()
    {
        state = !state;
    }




}
