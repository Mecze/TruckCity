using UnityEngine;
using System.Collections;

//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// Truck Entity
//////////////////////////////
/// "Truck" es un agente autonomo que se mueve por el mundo
///  gracias a sus scripts. 
///  Este script maneja la gasolina.
//////////////////////////////

public class GasMeter : MonoBehaviour {

    [SerializeField]
    SpriteRenderer meter;

    [SerializeField]
    float _currentGas;

    public float currentGas
    {
        get
        {
            return _currentGas;
        }

        set
        {
            UpdateGas(value, _currentGas);
            _currentGas = value;
            
        }
    }

    

    [SerializeField]
    string spriteName = "Gas_";

    [SerializeField]
    float[] gasSteps;

    bool _reverse;
    public bool reverse
    {
        get
        {
            return _reverse;
        }

        set
        {
            _reverse = value;
            Vector3 v = transform.localScale;
            if (_reverse) v.y = -Mathf.Abs(v.y);
            if (!_reverse) v.y = Mathf.Abs(v.y);
            transform.localScale = v;
        }
    }

    





    void UpdateGas(float value, float oldvalue)
    {
        if (WhichStep(oldvalue) == WhichStep(value)) return;
        string s = GameConfig.s.IMGPath + spriteName + WhichStep(value).ToString();
        meter.sprite = Resources.Load<Sprite>(s);





    }

    float WhichStep(float value)
    {
        float r = 0;
        for (int i = 0; i < gasSteps.Length; i++)
        {
            if (value <= gasSteps[i]) return gasSteps[i];
        }

        return r;

    }



    void Start()
    {
        //currentGas = 0.8f;
    }
}