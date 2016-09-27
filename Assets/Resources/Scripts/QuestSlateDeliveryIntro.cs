using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class QuestSlateDeliveryIntro : MonoBehaviour {
    bool firstUpdate = true;
    public bool Intro = true;
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
    Text PositionText;

    [SerializeField]
    GameObject CompletedOBJ;
    
    [SerializeField]
    Transform StarAnchor;
    [SerializeField]
    GameObject StarPrefab;
    [SerializeField]
    Sprite StarImg;
    [SerializeField]
    Sprite UndoneStarImg;
    List<Image> stars;
    [SerializeField]
    Sprite CargoIMG;
    [SerializeField]
    Sprite MoneyIMG;
    [SerializeField]
    Sprite TimerIMG;
    [SerializeField]
    Sprite CheckIMG;
    [SerializeField]
    Sprite CrossIMG;

    void Awake()
    {
        //AutoAnchor();


    }


    #region autoAnchor
    public void AutoAnchor()
    {

        string s;
        if (Intro) { s = "IPQuestSlateAnchor"; } else { s = "OPQuestSlateAnchor"; }
            transform.SetParent(GameObject.FindGameObjectWithTag(s).transform);
            this.transform.localScale = Vector3.one;


    }
    #endregion

    #region UpdateGUI

    public void UpdateGUI()
    {
        if (_myQuest == null || _myCargoDelivered == null) return;
        //if (_myQuest.winCondition == WinCondition.Delivered) 
        switch (_myQuest.winCondition)
        {
            case WinCondition.Delivered:
                    CargoTypeIMG.color = GameConfig.s.cargoColors[(int)_myCargoDelivered.type];
                    CargoTypeIMG.sprite = CargoIMG;
                    NumberText.text = _myQuest.winAmount.ToString();
                    break;
            case WinCondition.Money:
                    CargoTypeIMG.sprite = MoneyIMG;
                    NumberText.text = _myQuest.winAmount.ToString();
                    break;
            case WinCondition.Time:
                    CargoTypeIMG.sprite = TimerIMG;
                    TimeSpan ts = TimeSpan.FromSeconds(_myQuest.winAmount);
                    NumberText.text = string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
                    break;
            default:
                break;
        }

        
        
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
            
            QuestText.text = (s + " "+ GameConfig.s.playerOrdersQuestSlate[(int)_myQuest.winCondition]);
        }
        else
        {
            QuestText.text = (GameConfig.s.playerOrdersQuestSlate[(int)_myQuest.winCondition]);
        }
        PositionText.text = position.ToString() + ".";



        
        if (Intro)
        {
            CompletedOBJ.SetActive(false);
        }else
        {
            CompletedOBJ.SetActive(true);
            if (_myQuest.completed)
            {
                CompletedOBJ.GetComponent<Image>().sprite = CheckIMG;
            }else
            {
                CompletedOBJ.GetComponent<Image>().sprite = CrossIMG;
            }

        }




    }



    #endregion


}
