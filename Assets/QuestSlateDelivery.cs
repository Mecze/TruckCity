using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QuestSlateDelivery : MonoBehaviour {
    Quest _myQuest;
    public Quest MyQuest
    {
        get
        {
            return _myQuest;
        }

        set
        {
            _myQuest = value;
            UpdateGUI();
        }
    }

    CargoDelivered _myCargoDelivered;
    public CargoDelivered MyCargoDelivered
    {
        get
        {
            return _myCargoDelivered;
        }

        set
        {
            _myCargoDelivered = value;
            UpdateGUI();
        }
    }

    

    [SerializeField]
    Image CargoTypeIMG;
    [SerializeField]
    Text NumberText;
    [SerializeField]
    Text CurrentText;
    [SerializeField]
    GameObject CompletedOBJ;
    [SerializeField]
    GameObject CurrentTextOBJ;



    void Awake()
    {
        AutoAnchor();


    }


    #region autoAnchor
    void AutoAnchor()
    {
        
        
            transform.SetParent(GameObject.FindGameObjectWithTag("QuestGUI").transform);
            this.transform.localScale = Vector3.one;


    }
    #endregion

    #region UpdateGUI

    public void UpdateGUI()
    {
        if (_myQuest == null || _myCargoDelivered == null) return;
        CargoTypeIMG.color = GameConfig.s.cargoColors[(int)_myCargoDelivered.type];
        NumberText.text = _myQuest.winAmount.ToString();
        CurrentText.text = _myCargoDelivered.delivered.ToString() + "/" + _myQuest.winAmount.ToString();
        CompletedOBJ.SetActive(_myQuest.completed);
        CurrentTextOBJ.SetActive(!_myQuest.completed);

    }



    #endregion


}
