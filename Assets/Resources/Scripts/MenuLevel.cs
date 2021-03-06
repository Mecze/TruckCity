using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// Este Script se encarga de actualizar 1 solo boton del menú principal
/// Se trata de un script ubicado en un Prefab
/// y manejar que se hace cuando se pincha
//////////////////////////////

   
public class MenuLevel : MonoBehaviour {
    [Header("References")]
    [SerializeField]
    UILabel levelLabel;
    [SerializeField]
    UILabel shadowLabel;
    //[SerializeField]
    //UISprite image;
    [SerializeField]
    GameObject lockPanel;
    [SerializeField]
    List<GameObject> stars;
    [SerializeField]
    UILabel lockStars;
    [SerializeField]
    TweenScale tween2;

    public bool hover = false;
    public bool pressed = false;
    public bool dragging = false;

    ProfileLevels _myProfileLevel;

    public ProfileLevels myProfileLevel
    {
        get
        {
            return _myProfileLevel;
        }

        set
        {
            _myProfileLevel = value;
            UpdateGUI();

        }
    }

    void Awake()
    {
        
    }

    public void UpdateGUI()
    {
        FontShadowColors fsc;
        if (_myProfileLevel.index >= GameConfig.s.MenuLevelFontShadowColors.Length)
        {
            fsc = GameConfig.s.MenuLevelFontShadowColors[0];
        }
        else
        {
            fsc = GameConfig.s.MenuLevelFontShadowColors[_myProfileLevel.index];
        }
        levelLabel.text = (_myProfileLevel.index+1).ToString();
        shadowLabel.text = levelLabel.text;
        levelLabel.color = fsc.fontColor;
        levelLabel.effectColor = fsc.OutlineColor;
        shadowLabel.color = fsc.shadowColor;
        shadowLabel.effectColor = fsc.shadowColor;
        if (!myProfileLevel.locked)
        {
            lockPanel.SetActive(false);
            for (int i = 0; i <= myProfileLevel.stars-1; i++)
            {
                stars[i].SetActive(true);
            }
            
            


        }
        else
        {
            lockPanel.SetActive(true);
            lockStars.text = "+ " + (myProfileLevel.starsToUnlock - sProfileManager.ProfileSingleton.stars).ToString();
            
        }
        
    }
    public void clicked()
    {
        if (_myProfileLevel.locked) return;
        sMenu.singleton.OnLevelButtonClick(_myProfileLevel.code);
    }
    public void back()
    {
        if (dragging)
        {
            dragging = false; return;
        }
        if (!pressed)
        {
#if UNITY_STANDALONE_WIN 
            if (hover){
#endif
             clicked();
#if UNITY_STANDALONE_WIN
            }
#endif
            //pressed = true;
        }
        else
        {
            //tween2.PlayReverse();
            //pressed = false;
        }
        //ProfileManager.instance.LoadingScreen = true;

    }
    
    public void HoverOut()
    {
        hover = false;
        if (pressed)
        {
            tween2.PlayReverse();
            pressed = false;
        }
    }
    public void HoverIn()
    {
        hover = true;
    }
    public void OnPress()
    {
        pressed = true;
        tween2.PlayForward();        
    }
    public void OnRelease()
    {
        pressed = false;
        tween2.PlayReverse();        
    }
    public void OnDrag()
    {
        dragging = true;
    }
    public void OnDragEnd()
    {
        //dragging = false;
    }

}



