using UnityEngine;
using System.Collections;

public enum enumColor { Red = 0, Green = 1,Yellow = 2,Blue = 3 }

public class GameController : MonoBehaviour {
    #region Singleton
    private static GameController s_singleton = null;

    public static GameController singleton
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<GameController>();
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
    public static GameController s
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = FindObjectOfType<GameController>();
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

    [SerializeField]
    GameObject textGOPrefab;

    public float defaulYFloatingText;

    public Color[] publicColors;




    /// <summary>
    /// Genera un Floating text con el texto que le pedimos en la posición indicada
    /// </summary>
    /// <param name="pos">VECTOR2 (Y equivale a Z) (se usa la Y por defecto)</param>
    /// <param name="text">texto a mostrar</param>
    /// <param name="publiccolor">enumColor= Rojo, amarillo, ....</param>
    public void FloatingTextSpawn(Vector2 pos, string text, enumColor publiccolor)
    {
        Vector3 realPos = new Vector3(pos.x, defaulYFloatingText, pos.y);
        FloatingTextSpawn(realPos, text, publiccolor);


    }
    /// <summary>
    /// Genera un Floating text con el texto que le pedimos en la posición indicada
    /// </summary>
    /// <param name="x">La X de la posición (se usa la Y por defecto)</param>
    /// <param name="z">La Z de la posición (se usa la Y por defecto</param>
    /// <param name="text">texto a mostrar</param>
    /// <param name="publiccolor">enumColor= Rojo, amarillo, ....</param>
    public void FloatingTextSpawn(float x, float z, string text, enumColor publiccolor)
    {
        Vector3 realPos = new Vector3(x, defaulYFloatingText, z);
        FloatingTextSpawn(realPos, text, publiccolor);


    }

    /// <summary>
    /// Genera un Floating text con el texto que le pedimos en la posición indicada (Y personalizada)
    /// </summary>
    /// <param name="pos">Posición, se usa la Y indicada (y NO la por defecto)</param>
    /// <param name="text">texto a mostrar</param>
    /// <param name="publiccolor">enumColor= Rojo, amarillo, ....</param>
    public void FloatingTextSpawn(Vector3 pos, string text, enumColor publiccolor)
    {
        GameObject go = (GameObject)GameObject.Instantiate(textGOPrefab,pos,Quaternion.identity);
        go.GetComponent<FloatingText>().phrase = text;
        go.GetComponent<FloatingText>().myColor = publicColors[(int)publiccolor];
        go.GetComponent<FloatingText>().WakeMeUp();

    }

    void Start()
    {
        //FloatingTextSpawn(4f, 3f, "Prueba", enumColor.Blue);
    }


}