using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour {
    UILabel myLabel;
    float timer = 0f;

    [SerializeField]
    float UpdateTime = 0.2f;
    [SerializeField]
    Color GoodColor;
    [SerializeField]
    Color MediumColor;
    [SerializeField]
    Color BadColor;


    void Awake() { myLabel = GetComponent<UILabel>();   }
    

    void Update()
    {

        if (myLabel == null) return;
        if (Time.time < timer) return;

        timer = Time.time + UpdateTime;
        float fps = 1f / Time.deltaTime;
        myLabel.text = fps.ToString("F1") + " fps";
        if (fps < 30f) myLabel.color = BadColor;
        if (fps >= 30f && fps < 60f) myLabel.color = MediumColor;
        if (fps >= 60f) myLabel.color = GoodColor;


    }

}
