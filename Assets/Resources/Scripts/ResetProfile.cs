using UnityEngine;
using System.Collections;

public class ResetProfile : MonoBehaviour {

    [SerializeField]
    GameObject ResetSurePanel;
    
    void Awake()
    {
        ResetSurePanel.SetActive(false);
    }



    public void OnResetProfileButton()
    {
        ResetSurePanel.SetActive(!ResetSurePanel.activeSelf);
    }

    public void OnSure()
    {
        DoResetProfile();
    }

    void DoResetProfile()
    {
        sProfileManager.ProfileSingleton = sProfileManager.NewProfile(true);
        sProfileManager.s.InitializeGame(true);
//        LoadingScreenManager.LoadScene(1);

    }

}
