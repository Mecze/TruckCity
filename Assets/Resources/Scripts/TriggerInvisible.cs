using UnityEngine;
using System.Collections;
using PicaVoxel;

//Attach this Script to an empty object with a BoxCollider set on "Trigger"
//fulfill the "Config parameters" on the inspector (Check "Config Parameters" region below)
//Also check the part: "Triggers that register Objects" region below.


[RequireComponent(typeof(BoxCollider))]
public class TriggerInvisible : MonoBehaviour {
    #region Config parameters

    [Header("My Volume (High)")]
    //Fill this with Volume that you want to turn invisible
    [SerializeField]
    Volume myVolume;

    [Header("My Sprite Renderer (Low)")]
    //Fill this with SpriteRenderer that you want to turn invisible
    [SerializeField]
    SpriteRenderer mySpriteRenderer;

    [Header("Colors")]
    //Pick the transparent color on the inspector
    [SerializeField]
    Color transparentColor;

    [SerializeField]
    //Pick the Solid color on the inspector
    Color solidColor;      

    [Header("Speed to Fade")]
    //Speed to Fade between states
    [SerializeField]
    float speedToFade = 10f;
    #endregion

    #region internal parameters
    /// <summary>
    /// True if it has to turn transparent
    /// </summary>
    bool isTransparent = false;    

    /// <summary>
    /// Reference to the material used.
    /// note: at the "Awake" a new clone material is created
    /// </summary>
    Material myMat;
    
    /// <summary>
    /// True after the initial setup of the material on the "Awake"
    /// </summary>
    bool setup = false;
    
    /// <summary>
    /// Keeps track of the number of Trucks under the building
    /// </summary>
    int _numberOfTrucks;
    public int numberOfTrucks
    {
        get
        {
            return _numberOfTrucks;
        }

        set
        {
            _numberOfTrucks = value;
            //It can never be lesser than 0
            if (_numberOfTrucks < 0) _numberOfTrucks = 0;
            //Checks if we have to go transparent
            ChangedTransparencyCheck(_numberOfTrucks);

        }
    }

    
    /// <summary>
    /// This property Sets and gets our Material's (myMat) "_Tint" Color.
    /// (Because I'm lazy)
    /// </summary>
    Color myColor
    {
        get
        {
            switch (GlobalQS)
            {
                case GraphicQualitySettings.Low:
                    return mySpriteRenderer.color;                    
                case GraphicQualitySettings.High:
                    if (myMat == null) return Color.white;
                    return myMat.GetColor("_Tint");                    
                default:
                    return Color.white;                    
            }

            
        }
        set
        {
            switch (GlobalQS)
            {
               case GraphicQualitySettings.Low:
                    mySpriteRenderer.color = value;
                    break;
                case GraphicQualitySettings.High:
                    if (myMat != null)myMat.SetColor("_Tint", value);                    
                    break;
                default:
                    break;
            }
        }
    }

   

    /// <summary>
    /// Global Quality Settings
    /// </summary>
    GraphicQualitySettings _globalQS = GraphicQualitySettings.None;
    GraphicQualitySettings GlobalQS
    {
        get
        {
            if ((int)_globalQS == 0) _globalQS = sProfileManager.ProfileSingleton.GlobalGraphicQualitySettings;
            if (_globalQS == GraphicQualitySettings.High && myVolume == null) _globalQS = GraphicQualitySettings.Low;
            if (_globalQS == GraphicQualitySettings.Low && mySpriteRenderer == null) _globalQS = GraphicQualitySettings.High;
            return _globalQS;
        }       
    }



    #endregion

    #region Methods

    void Awake()
    {
        switch (GlobalQS)
        {            
            case GraphicQualitySettings.Low:
                SetupLow();
                break;
            case GraphicQualitySettings.High:
                SetupHigh();
                break;
            default:
                break;
        }


       

    }

    void SetupLow()
    {
        //Nothing for now;
        setup = true;
    }

    void SetupHigh()
    {
        //Setup
        //We clone our own material and save it on myMat
        myMat = new Material(myVolume.Material);
        myMat.name = "clone";

        //We assign our mat to our volume
        myVolume.Material = myMat;
        //We need to call this to update the Material on the Chuncks
        myVolume.UpdateAllChunks();

        setup = true; //Setup complete
    }



    void Update()
    {
        if (!setup) return; //Do nothing if Setup is not complete
        //We lerp colors between states (see myColor)
        if (isTransparent)
        {
            
            myColor = Color.Lerp(myColor, transparentColor, speedToFade*Time.deltaTime);
        }
        else
        {
            myColor = Color.Lerp(myColor, solidColor, speedToFade * Time.deltaTime);
        }
    }

    /// <summary>
    /// Checks if we have to go Transparent
    /// (Later isTransparent is Checked on Update)
    /// </summary>
    /// <param name="trucks">number of trucks (under the building)</param>
    void ChangedTransparencyCheck(int trucks)
    {
        if (trucks == 0)
        {
            isTransparent = false;

        }
        else
        {
            isTransparent = true;

        }
    }
    #endregion


    #region Triggers that register Objects
    //You can change the way it register things (in my case Trucks) under the Volume (in my case buildings).
    //You can, for example, use other tag or tags to detect things around and diferent box sizes
    //You could also expose the tag to compare to the inspector using a serialized string variable
    

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Truck")
        {
            numberOfTrucks++;
        }


    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Truck")
        {
            numberOfTrucks--;
        }
    }
    #endregion

    

	



}