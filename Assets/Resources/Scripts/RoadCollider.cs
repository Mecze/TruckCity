using UnityEngine;
using System.Collections;

public class RoadCollider : MonoBehaviour {
    
    [SerializeField]
    RoadEntity myRoadEntity;
    [Header("Turn Conditions")]
    [SerializeField]
    RoadDirection[] roadDirectionCondition;
    [SerializeField]
    CardinalPoint[] truckDirectionCondition;
    [SerializeField]
    CardinalPoint[] newTruckDirection;
    [SerializeField]
    Turn[] newTurn;
    [SerializeField]
    

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Truck") return;
        TruckEntity te = other.GetComponent<TruckEntity>();

        int numberOfConditions = roadDirectionCondition.Length;

        for (int i = 0; i < numberOfConditions; i++)
        {
            if (myRoadEntity.direction == roadDirectionCondition[i])
            {
                if (truckDirectionCondition[i] == te.direction)
                {
                    te.ChangeDirection(newTruckDirection[i], newTurn[i]);
                }
            }
        }
        

    }
}