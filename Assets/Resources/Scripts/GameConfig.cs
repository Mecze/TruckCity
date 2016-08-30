using UnityEngine;
using System.Collections;

public enum enumColor { Red = 0, Green = 1, Yellow = 2, Blue = 3, Black = 4 }
public enum CargoType { None = 0, Pink = 1, Brown = 2 }

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
    public string IMGPath;


    public Color[] publicColors;

    #region cargoFILE Path configuration
    [Header("Cargo: filename of the Materials Config")]
    public string cargoMaterialFileName;
    [Header("Cargo: filename of the Sprite Config")]
    public string cargoSpriteCommonFileName;
    public string[] cargoSpriteFileName;

    public Color[] cargoColors;


    #endregion


}