using UnityEngine;
using System.Collections;

public class GuiTutorialButton : MonoBehaviour {
    public void Click()
    {
        TutorialController.s.Click();
    }
}
