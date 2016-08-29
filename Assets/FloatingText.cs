using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {

    [SerializeField]
    Text text;

    public string phrase;

    public Color myColor;
	
    /// <summary>
    /// Inicia el texto flotante (se suele llamar despues de ser creado el objeto)
    /// </summary>
	public void WakeMeUp () {
        //posición de la camara
        Vector3 v = Camera.main.transform.position;
        
        //la x de esa posición será la misma que la mia
        v.x = this.transform.position.x;        

        //Miro a la camara
        transform.LookAt(v);

        //Ahora revierto el eje X de mi rotación
        Quaternion r = this.transform.rotation;
        r.x = -r.x;
        this.transform.rotation = r;

        //Inicializo el texto
        text.enabled = true;
        text.text = phrase;
        text.color = myColor;
        Destroy(this.gameObject, 2f);
	}

    void Update()
    {

        this.transform.Translate(Vector3.up * Time.deltaTime);        
        myColor.a -= (0.5f * Time.deltaTime);
        text.color = myColor;
    }
}
