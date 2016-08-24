using UnityEngine;
using System.Collections;

public class GameConfig : MonoBehaviour {
    #region Singleton
    private static GameConfig s_singleton = null;

    public static GameConfig singleton
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<GameConfig>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("No Existe Singleton MapController");
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }
    public static GameConfig s
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<GameConfig>();
            }
            if (s_singleton == null)
            {
                //Esto no deberia pasar nunca!
                Debug.LogError("No Existe Singleton MapController");
            }
            return s_singleton;
        }
        set { s_singleton = value; }
    }
    #endregion

    public string materialsPath;


}