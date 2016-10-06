using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {

    [SerializeField]
    UILabel myText;
    [SerializeField]
    UISprite mySprite;
    [SerializeField]
    TweenPosition myTweenPostion;
    [SerializeField]
    TweenAlpha myTweenAlpha;


    /// <summary>
    /// Distancia para la posici�n final de la Y de final de myTweenPosition
    /// </summary>
    public float distanceOfY;
	
    /// <summary>
    /// Inicia el texto flotante (se suele llamar despues de ser creado el objeto)
    /// </summary>
	public void WakeMeUp (string phrase, string spriteName, Color textColor, Transform worldPosition, Color cargoColor) {

        //Emparentamos este objeto con su respectivo padre
        this.transform.SetParent(GameObject.FindGameObjectWithTag("FloatingTextGUIAnchor").transform);
        this.transform.localScale = Vector3.one;

        //Se ajusta la posici�n de este objeto (de World Position a "Screen" Position) Puesto que es un elemento de UI
        this.transform.position = NGUIMath.WorldToLocalPoint(worldPosition.position, Camera.main, UICamera.s.gameObject.GetComponent<Camera>(), this.transform);

            //UICamera.s.gameObject.GetComponent<Camera>().WorldToScreenPoint(worldPosition.position);
            //NGUIMath.OverlayPosition(this.transform, worldPosition);
            //Camera.main.WorldToScreenPoint(worldPosition);

        //Se cambia el Sprite
        mySprite.spriteName = spriteName;
        if (cargoColor != Color.black) mySprite.color = cargoColor;


        //Inicializo el texto        
        myText.text = phrase;
        myText.color = textColor;

        //Se ajusta la posici�n inicial y final del Tween de Posici�n (varia!)
        Vector3 v = new Vector3();
        v = this.transform.position;
        myTweenPostion.from = v;
        v.y = v.y + distanceOfY;
        myTweenPostion.to = v;
        
        //Se inician los Tweens animaciones
        myTweenAlpha.PlayForward();
        myTweenPostion.PlayForward();
        Destroy(this.gameObject, myTweenAlpha.duration+0.1f);
	}

    
    
}
