using UnityEngine;
using System.Collections;
#pragma warning disable 0649
public class UnlockAnim : MonoBehaviour {
    public bool UnlockMe = false;
    [SerializeField]
    sMenuButton myButton;

	
    void Update()
    {
        if (UnlockMe)
        {
            UnlockMe = false;
            myButton.myProfileLevel.locked = false;
            myButton.UpdateGUI();
            sSaveLoad.SaveProfile();
            myButton.UnlockAnimator.SetBool("Start", false);
            StartCoroutine(HideMe());
        }
    }
    IEnumerator HideMe()
    {
        yield return new WaitForSeconds(0.5f);
        myButton.UnlockPanel.SetActive(false);
    }


}
#pragma warning restore 0649