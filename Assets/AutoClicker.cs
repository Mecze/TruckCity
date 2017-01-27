using UnityEngine;
using System.Collections;

public class AutoClicker : MonoBehaviour {
    [SerializeField]
    bool activate = true;
    [SerializeField]
    float minFrecuency = 1f;
    [SerializeField]
    float maxFrecuency = 10f;

    float timer = 0f;

    GreenRotationRoad GreenRoad;
    PurpleRotationRoad PurpleRoad;

	



    void Awake()
    {
        if (GetComponent<GreenRotationRoad>() != null) GreenRoad = GetComponent<GreenRotationRoad>();
        if (GetComponent<PurpleRotationRoad>() != null) PurpleRoad = GetComponent<PurpleRotationRoad>();


        ResetTimer();


    }


    void Update()
    {
        if (!activate) return;
        if (GreenRoad != null && timer < Time.time)
        {
            GreenRoad.SimulateClick();
            ResetTimer();
        }
        if (PurpleRoad != null && timer < Time.time)
        {
            PurpleRoad.SimulateClick();
            ResetTimer();
        }

    }


    void ResetTimer()
    {
        timer = Time.time + Random.Range(minFrecuency, maxFrecuency);
    }



}
