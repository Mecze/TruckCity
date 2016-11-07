using UnityEngine;
using System.Collections;

public class GraphicsButton : MonoBehaviour {
    /*
    [SerializeField]
    UILocalize myLabel;
    [SerializeField]
    UILocalize myLabelShadow;
    */
    [SerializeField]
    Color SelectedColor;
    [SerializeField]
    GraphicQualitySettings MyCondition;


    //UIButton button;
    UISprite sprite;
    bool disabled = false;


    void Awake()
    {
        //button = GetComponent<UIButton>();
        sprite = GetComponent<UISprite>();
        Set(sProfileManager.ProfileSingleton.GlobalGraphicQualitySettings);
    }

    public void Set(GraphicQualitySettings GQS)
    {
        /*
        
        switch (GQS)
        {
            case GraphicQualitySettings.Low:
                

                
                
                break;
            case GraphicQualitySettings.Medium:

                
                break;

            case GraphicQualitySettings.High:
                myLabel.Key = "High";
                break;
            default:
                break;
        }
        myLabelShadow.Key = myLabel.Key;
        //myLabel
        */
        if (MyCondition == GQS) {
            sprite.color = SelectedColor;
            disabled = true;
        }

    }

    public void OnClick()
    {
        if (disabled) return;
        
        

        
        sProfileManager.ProfileSingleton.GlobalGraphicQualitySettings = MyCondition;
        Set(MyCondition);
        sSaveLoad.SaveProfile();
        QualitySettings.SetQualityLevel((int)MyCondition);
        LoadingScreenManager.LoadScene(1);
        return;
        
        
        
        
        

    }

	
}
