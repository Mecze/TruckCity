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
    bool border = true;
    /*
    [Header("Reverse Conditions")] //Prohibido!
    [SerializeField]
    RoadDirection[] reverseRoadDirectionCondition;
    [SerializeField]
    TruckDirection[] reverseTruckDirectionCondition;
    */


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
        if (border)
        {
            /*
            if (te.direction == RoadEntity.ReverseDirection(truckDirectionCondition[0]) && !myRoadEntity.direction.HasDirection(RoadEntity.ReverseDirection(te.direction)))
            {
                TruckDirection r = RoadEntity.ReverseDirection(te.direction);
                te.ChangeDirection(r,Turn.Reverse, 0.45f);
            }
            */
            /*
            TruckDirection direction = te.direction;
            RoadEntity NextTile;
            if (gameObject.name == "TopCollider")
                if (direction != TruckDirection.N) return;
            if (gameObject.name == "BottomCollider")
                if (direction != TruckDirection.S) return;
            if (gameObject.name == "LeftCollider")
                if (direction != TruckDirection.W) return;
            if (gameObject.name == "RightCollider")
                if (direction != TruckDirection.E) return;

            if (MapController.s.CheckNextTile(myRoadEntity.position, direction, out NextTile))
            {
                bool b = RoadEntity.CheckConnection(myRoadEntity, NextTile);
                if (!b) te.ChangeDirection(RoadEntity.ReverseDirection(te.direction), Turn.Reverse);
            }
            else
            {
                //Da la vuelta al camion
                te.ChangeDirection(RoadEntity.ReverseDirection(te.direction), Turn.Reverse);
            }
            */


        }

    }
}