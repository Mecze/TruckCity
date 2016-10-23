using UnityEngine;
using System.Collections;

public class GraphicsButton : MonoBehaviour {
    [SerializeField]
    UILocalize myLabel;
    [SerializeField]
    UILocalize myLabelShadow;



    void Awake()
    {
        Set(sProfileManager.ProfileSingleton.GlobalGraphicQualitySettings);
    }

    public void Set(GraphicQualitySettings GQS)
    {
        
        
        switch (GQS)
        {
            case GraphicQualitySettings.Low:
                myLabel.Key = "Low";
                //myLabelShadow.text = myLabel.text;
                break;
            case GraphicQualitySettings.High:
                myLabel.Key = "High";
                break;
            default:
                break;
        }
        myLabelShadow.Key = myLabel.Key;
        //myLabel
    }

    public void OnClick()
    {
        Debug.Log("hi!");
        GraphicQualitySettings GQS = sProfileManager.ProfileSingleton.GlobalGraphicQualitySettings;
        if (GQS == GraphicQualitySettings.Low)
        {
            GQS = GraphicQualitySettings.High;
            sProfileManager.ProfileSingleton.GlobalGraphicQualitySettings = GQS;
            Set(GQS);
            sSaveLoad.SaveProfile();
            QualitySettings.SetQualityLevel((int)GQS);
            LoadingScreenManager.LoadScene(1);
            return;
        }
        if (GQS== GraphicQualitySettings.High)
        {
            GQS = GraphicQualitySettings.Low;
            sProfileManager.ProfileSingleton.GlobalGraphicQualitySettings = GQS;
            Set(GQS);
            sSaveLoad.SaveProfile();
            QualitySettings.SetQualityLevel((int)GQS);
            LoadingScreenManager.LoadScene(1);
            return;
        }
        

    }

	
}
