using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class QuestSlateDelivery : MonoBehaviour {
    bool firstUpdate = true;
    public int position = 1;

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
    Text QuestText;
    [SerializeField]
    Text CurrentText;
    [SerializeField]
    GameObject CompletedOBJ;
    [SerializeField]
    GameObject CurrentTextOBJ;
    [SerializeField]
    Transform StarAnchor;
    [SerializeField]
    GameObject StarPrefab;
    [SerializeField]
    Sprite StarImg;
    [SerializeField]
    Sprite UndoneStarImg;
    List<Image> stars;



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
        if (_myQuest.winCondition == WinCondition.Delivered) CargoTypeIMG.color = GameConfig.s.cargoColors[(int)_myCargoDelivered.type];
        if (_myQuest.winCondition != WinCondition.Time)
        {
            NumberText.text = _myQuest.winAmount.ToString();
        }else
        {
            TimeSpan ts = TimeSpan.FromSeconds(_myQuest.winAmount);
            NumberText.text = string.Format("{0:D2}:{1:D2}", ts.Minutes,ts.Seconds);
        }

        if (_myQuest.winCondition == WinCondition.Delivered) CurrentText.text = _myCargoDelivered.delivered.ToString() + "/" + _myQuest.winAmount.ToString();
        //if (_myQuest.winCondition == WinCondition.Money) CurrentText.text = _myCargoDelivered.delivered.ToString();
        CompletedOBJ.SetActive(_myQuest.completed);
        CurrentTextOBJ.SetActive(!_myQuest.completed);


        if (firstUpdate)
        {
            stars = new List<Image>();
            firstUpdate = false;
            for (int i = 1; i <= _myQuest.starRewards; i++)
            {
                GameObject go = GameObject.Instantiate(StarPrefab);
                go.transform.SetParent(StarAnchor);
                go.transform.localScale = Vector3.one;
                stars.Add(go.GetComponent<Image>());
            }
        }

        if (_myQuest.completed) foreach (Image img in stars) img.sprite = StarImg;
        if (!_myQuest.completed) foreach (Image img in stars) img.sprite = UndoneStarImg;
        
        if (_myQuest.LinkedQuestEnabled)
        {
            string s = "Do ";
            bool first = true;
            foreach (int i in _myQuest.LinkedQuestIndex)
            {
                if (!first) s += " & ";
                s += (i + 1).ToString();
                first = false;
            }
            //QuestText.text = s + QuestText.text;
            QuestText.text = (position.ToString() + ". " + s + " " + GameConfig.s.playerOrdersQuestSlate[(int)_myQuest.winCondition]);
        }
        else
        {
            QuestText.text = (position.ToString() + ". " + GameConfig.s.playerOrdersQuestSlate[(int)_myQuest.winCondition]);
        }




    }



    #endregion


}
