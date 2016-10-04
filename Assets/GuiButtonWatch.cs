using UnityEngine;
using System.Collections;

public class GuiButtonWatch : MonoBehaviour {

    [SerializeField]
    GameObject GUIPanel;
    [SerializeField]
    GameObject IntroPanel;
    [SerializeField]
    GameObject WatchPanel;


    public void Click()
    {
        GUIPanel.SetActive(!GUIPanel.activeSelf);
        IntroPanel.SetActive(GUIPanel.activeSelf);
        WatchPanel.SetActive(!GUIPanel.activeSelf);
    }

}
