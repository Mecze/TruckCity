using UnityEngine;
using System.Collections;

public class LevelNumberAwake : MonoBehaviour {

    [SerializeField]
    string Extra = "";

    void OnEnable()
    {
        GetComponent<UILabel>().text = (GameController.s.level + 1).ToString()+ Extra;

    }
}
