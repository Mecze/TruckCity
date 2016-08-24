using UnityEngine;
using System.Collections;

public class RoadCollider : MonoBehaviour {
    [SerializeField]
    RoadEntity myRoadEntity;
    [SerializeField]
    RoadDirection[] roadDirectionCondition;
    [SerializeField]
    TruckDirection[] truckDirectionCondition;
    [SerializeField]
    TruckDirection[] newTruckDirection;
    [SerializeField]
    Turn[] newTurn;
    [SerializeField]
    bool border = true;

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


        }

    }
}