using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using Eppy;

//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// Este Script Maneja la UI de los "Paquetitos"
/// que se ven encima de los edificios,
/// Va incluido en el Prefab "CargoSprite" y este
/// es generado por CargoBuilding.
/// Ademas, CargoBuilding asigna esta INSTACIA de
/// este script a la Instancia de AcceptsCargo y
/// ProducesCargo Correspondiente.
/// A partir de ah� son estos dos scritps los que se
/// encargan de actualizar a CargoSprite y sus Sprites
/// pasandole informaci�n a este script. Al hacerlo, 
/// este script modifica la UI. 
//////////////////////////////
#pragma warning disable 0169
public class CargoSprite : MonoBehaviour {
    [Header("My References")]
    [SerializeField]
    Text myText;
    [SerializeField]
    Image myImage;
    [SerializeField]
    RectTransform myTextTransform;
    [SerializeField]
    GameObject myTextGO;
    [SerializeField]
    GameObject myTimerGO;
    [SerializeField]
    Image myTimer;
    [SerializeField]
    Image myInnerTimerImage;
    [SerializeField]
    GameObject myOneDolarImage;



    [Header("CargoSprite Config")]
    /// <summary>
    /// True (This cargo is produced here) if the cargo sprite should have a Timer for producing and use Produce Config
    /// False (This cargo is delivered here) if the cargo sprite shows the amount of $ to gain when delivered
    /// </summary>
    [SerializeField] //debug
    bool _produced;
    
    /// <summary>
    /// moneyGain on Delivery (Only if _produced = false;
    /// </summary>
    [SerializeField] //debug
    int _moneyOnDelivery = 20;

    /// <summary>
    /// Amount of current items of this type produced
    /// </summary>
    [SerializeField] //debug
    int _amountOfItems = 0;

    /// <summary>
    /// Time needed to 1 produce Cicle
    /// Note: Always % of 6.
    /// </summary>
    [SerializeField]//debug
    int _timeToProduce = 12;
    
    /// <summary>
    /// Max amount of items stores on this building of this type
    /// </summary>
    [SerializeField]//debug
    int _maxAmountOfItems = 1;

    /// <summary>
    /// The current Timer time.
    /// </summary>
    [SerializeField]//debug
    float _timer = 0f;

    /// <summary>
    /// The Cargo Type of this object    
    /// </summary>
//    public CargoType cargoType;


    /// <summary>
    /// Where it spawns cargo or where it recolects it
    /// </summary>
    //public CardinalPoint direction;


    [Header("Constant Config")]
    [SerializeField]
    public List<float> timerSteps;




    #region properties

    /// <summary>
    /// moneyGain on Delivery (Only if _produced = false;
    /// </summary>
    public int moneyOnDelivery
    {
        get
        {
            return _moneyOnDelivery;
        }

        set
        {
            _moneyOnDelivery = value;
            if (!_produced)
            {
                myText.text = _moneyOnDelivery.ToString() + "$";
                myOneDolarImage.SetActive(true);
            }
        }
    }

    /// <summary>
    /// True (This cargo is produced here) if the cargo sprite should have a Timer for producing and use Produce Config
    /// False (This cargo is delivered here) if the cargo sprite shows the amount of $ to gain when delivered
    /// </summary>
    public bool produced
    {
        get
        {
            return _produced;
        }

        set
        {
            _produced = value;
            if (value)
            {
                if (_maxAmountOfItems > 1)
                {
                    myText.text = "x" + _amountOfItems.ToString();
                }else
                {
                    myText.text = "";
                }
                myOneDolarImage.SetActive(false);


            }
            else
            {
                myText.text = _moneyOnDelivery.ToString() + "$";
                timer = 0f;
                myOneDolarImage.SetActive(true);
            }


        }
    }

    /// <summary>
    /// The Text of this Item (stored directly on .Text of it's Text)
    /// </summary>
    public string text
    {
        get
        {
            return myText.text;
        }
        set
        {
            myText.text = value;
        }
    }


    /// <summary>
    /// the Timer of this Item 
    /// </summary>
    public float timer
    {
        get
        {

            return _timer;
        }
        set
        {
            
            _timer = value;
            SetTimerSteps();
        }
    }

    /// <summary>
    /// Time needed to 1 produce Cicle
    /// Note: Always % of 6.
    /// </summary>
    public int timeToProduce
    {
        get
        {
            return _timeToProduce;
        }

        set
        {
            _timeToProduce = value;
        }
    }

    /// <summary>
    /// Max amount of items stores on this building of this type
    /// </summary>
    public int maxAmountOfItems
    {
        get
        {
            return _maxAmountOfItems;
        }

        set
        {
            _maxAmountOfItems = value;
        }
    }

    /// <summary>
    /// Amount of current items of this type produced
    /// </summary>
    public int amountOfItems
    {
        get
        {
            return _amountOfItems;
        }

        set
        {
            //En caso de que estemos a TOPE de storage de Cargo y restemos uno, iniciamos el CD
            //if (_amountOfItems == _maxAmountOfItems && value < _amountOfItems) StartCooldown();

            //Ocurre el SET
            _amountOfItems = value;

            //Cambios para la interfaz
            if (_amountOfItems == maxAmountOfItems)
            {
                myTimer.fillAmount = 0f;
            }
            if (_amountOfItems < 1) myText.text = "";
            if (_amountOfItems >= 1) myText.text = "x" + _amountOfItems.ToString();
            if (_amountOfItems == 0)
            {
                myImage.fillAmount = 0f;
                myText.text = "0";
            }
            if (_amountOfItems != 0) myImage.fillAmount = 1f;
        }
    }

    


    #endregion




    #region Methods
    /*
    void Awake()
    {
        Vector2 asdf = myTextTransform.sizeDelta;
        //Initialize(); //debug
    }
    void OnDisble()
    {
        if (_produced) TimeController.OnStartClock -= StartCooldown;
        if (_produced) TimeController.OnStartClock -= CooldownTick;
    }
    */

    public void SetColor(Color cargoColor, Color textColor)
    {
        myText.color = textColor;
        myImage.color = cargoColor;
        myTimer.color = cargoColor;
        myInnerTimerImage.color = cargoColor;
    }

    public void Adjust()
    {
        OnRectTransformDimensionsChange();
    }

    //Se adapta a su padre si es cambiado de tama�o
    void OnRectTransformDimensionsChange()
    {
        if (myTextTransform == null) return;
        RectTransform RT = GetComponent<RectTransform>();        
        myTextTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ((myTextTransform.anchorMax.x - myTextTransform.anchorMin.x)* RT.rect.width)/myTextTransform.localScale.x);
        myTextTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ((myTextTransform.anchorMax.y - myTextTransform.anchorMin.y) * RT.rect.height) / myTextTransform.localScale.y);
        //myTextTransform.sizeDelta = (myTextTransform.anchorMax - myTextTransform.anchorMin) / myTextTransform.localScale.x;
    }

    public void SetTimerSteps()
    {       
        myTimer.fillAmount = timerSteps[timerSteps.GetClosestIndex(_timer)];
        if (_amountOfItems == maxAmountOfItems) myTimer.fillAmount = 0f;
    }
    /*
    public void Initialize()
    {
        //If this Produces:
        if (_produced)
        {
            if (amountOfItems < maxAmountOfItems)
            {
                //TimeController.OnStartClock += StartCooldown;
                TimeController.OnStartClock += CooldownTick;
                amountOfItems = amountOfItems;
            }
        }
        else
        {
            myText.text = moneyOnDelivery.ToString() + "$";
            myTimer.fillAmount = 0f;
        }


    }

    void StartCooldown()
    {
        int delta = 1;
        if (TimeController.s.decrement) delta = -2;
        TimeController.s.timer.AddAction(((int)TimeController.s.currentTime) + delta, CooldownTick);
    }


    void CooldownTick()
    {
        Debug.Log(TimeController.s.currentTime.ToString() + " CooldownTick: " + timer.ToString());
        timer = timer + 1f;
        if (timer > timeToProduce)
        {
            timer = 0f;
            amountOfItems++;
            
        }
        if (amountOfItems != maxAmountOfItems)
        {
            StartCooldown();
        }
    }

    */
    #endregion




}
