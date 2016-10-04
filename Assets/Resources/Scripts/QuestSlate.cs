using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;



//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// Este Script Maneja de manera autonoma los QuestSlate
/// cada QuestSlate lleva asociado este Script.
/// Este Script es referenciado desde la secuencia de Inicio de Nivel
/// en el GameController
//////////////////////////////


public class QuestSlate : MonoBehaviour {
    
    public int position = 1;
    public bool IP = false;

    #region Properties

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
            //UpdateGUI();
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
            //ShowSlate();
        }
    }


    bool _questCompleted = false;
    public bool questCompleted
    {
        get
        {
            return _questCompleted;
        }

        set
        {
            if (value != _questCompleted)
            {
                _questCompleted = value;
                if (value)
                {
                    QuestCompletedAnimation();
                }
                else
                {
                    QuestNotCompletedAnimation();
                }
            }
        }
    }



    #endregion

    #region References

    [Header("Child References")]
    [SerializeField]
    UISprite MainIMG;
    [SerializeField]
    UILabel MainLabel;
    [SerializeField]
    UILabel StarLabel;
    [SerializeField]
    UISprite ShadowIMG;
   
    [Header("Animations")]
    [SerializeField]
    TweenScale StarScaleTween;
    [SerializeField]
    TweenAlpha StarAlphaTween;
    [SerializeField]
    TweenScale CheckScaleTween;
    [SerializeField]
    TweenAlpha CheckAlphaTween;

    [Header("My Own Tweens")]
    [SerializeField]
    TweenColor myTweenColor;
    [SerializeField]
    TweenPosition myTweenPosition;

    [Header("Image Names On Atlas")]
    [SerializeField]
    string DeliveryName;
    [SerializeField]
    string MoneyName;
    [SerializeField]
    string TimeName;

    #endregion

    #region Metodos
    
    #region Animations With Tweens
    //StarCompletion
    void QuestCompletedAnimation()
    {
        
        myTweenColor.ResetToBeginning();
        StarScaleTween.PlayForward();
        StarAlphaTween.PlayForward();
        CheckAlphaTween.PlayForward();
        CheckScaleTween.PlayForward();
        myTweenColor.PlayForward();
    }
    void QuestNotCompletedAnimation()
    {
        
        myTweenColor.ResetToBeginning();
        StarScaleTween.PlayReverse();
        StarAlphaTween.PlayReverse();
        CheckAlphaTween.PlayReverse();
        CheckScaleTween.PlayReverse();
        myTweenColor.PlayForward();
    }

    //Show Slate
    void ShowSlate(bool reverse = true, float Wait = 2f)
    {
        if (IP) return;
        FixY();
        myTweenPosition.PlayForward();
        StartCoroutine(UpdateSlateAfterShow(myTweenPosition.duration + myTweenPosition.duration/2f));
        if (reverse) StartCoroutine(ReverseShowSlate(Wait));
        
    }
    IEnumerator UpdateSlateAfterShow(float wait)
    {
        yield return new WaitForSeconds(wait);
        UpdateGUI();
    }
    IEnumerator ReverseShowSlate(float waittime)
    {
        yield return new WaitForSeconds(waittime);
        myTweenPosition.PlayReverse();
    }

    void FixY()
    {
        //Fix TweenPosition Y (variable)
        myTweenPosition.from.y = GetComponent<UIWidget>().transform.localPosition.y;
        myTweenPosition.to.y = GetComponent<UIWidget>().transform.localPosition.y;
    }

    #endregion

    #region EventListeners
    void OnScoreListener(CargoType cargoDelivered)
    {
        Debug.Log("Event Cargo Delivered ONScoreListener() QuestSlate");
        if (cargoDelivered != _myQuest.CargoType) return;
        if (_myQuest.completed && _myCargoDelivered.delivered > _myQuest.winAmount) return;
        ShowSlate();
    }


    #endregion
        
    #region UpdateGUI

    /// <summary>
    /// Se ejecuta al ser creado desde GAMECONTROLLER
    /// Prepara el SLATE para el juego
    /// </summary>
    public void SetSlate()
    {
        //Register EventListeners
        //Si son de INTRO no se registran Eventos
        if (!IP)
        {
            if (_myQuest.winCondition == WinCondition.Delivered)
            {
                GameController.s.OnScore += OnScoreListener;
            }            

        }else
        {
            GameController.s.OnPause += UpdateGUI;
        }
        //UI SETUP
        //-------

        StarLabel.text = _myQuest.starRewards.ToString();
        switch (_myQuest.winCondition)
        {
            case WinCondition.Delivered:
                MainIMG.color = GameConfig.s.cargoColors[(int)_myQuest.CargoType];
                MainLabel.text = "0/" + _myQuest.winAmount.ToString();
                MainIMG.spriteName = DeliveryName;
                ShadowIMG.gameObject.SetActive(true);
                break;
            case WinCondition.Money:
                MainLabel.text = _myQuest.winAmount.ToString() + " $";
                MainIMG.spriteName = MoneyName;
                ShadowIMG.gameObject.SetActive(false);
                break;
            case WinCondition.Time:
                MainLabel.text = _myQuest.winAmount.ToString();
                MainIMG.spriteName = TimeName;
                ShadowIMG.gameObject.SetActive(false);

                break;
            default:
                break;
        }

        if (IP) UpdateGUI();

    }
    /* //DESTRUCTOR!
    ~QuestSlate()
    {//Unsubscribe Events
        if (_myQuest.winCondition == WinCondition.Delivered) GameController.s.OnScore -= OnScoreListener;
    }
    */

    void UpdateGUI()
    {
        //questCompleted = true;
        if (_myQuest == null || _myCargoDelivered == null) return;

        

        questCompleted = _myQuest.completed;

        if (_myQuest.winCondition == WinCondition.Delivered) MainLabel.text = MyCargoDelivered.delivered.ToString()+ "/" + _myQuest.winAmount.ToString();
        



    }



    #endregion
    #endregion

}
