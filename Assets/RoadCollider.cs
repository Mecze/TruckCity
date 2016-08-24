using UnityEngine;
using System.Collections;

public class RoadCollider : MonoBehaviour {
    [SerializeField]
    RoadID myRoadId;
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
            if (myRoadId.roadID == roadDirectionCondition[i])
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
            RoadID NextTile;
            if (gameObject.name == "TopCollider")
                if (direction != TruckDirection.N) return;
            if (gameObject.name == "BottomCollider")
                if (direction != TruckDirection.S) return;
            if (gameObject.name == "LeftCollider")
                if (direction != TruckDirection.W) return;
            if (gameObject.name == "RightCollider")
                if (direction != TruckDirection.E) return;

            if (MapController.s.CheckNextTile(myRoadId.pos, direction, out NextTile))
            {
                bool b = RoadID.CheckConnection(myRoadId, NextTile);
                if (!b) te.ChangeDirection(RoadID.ReverseDirection(te.direction), Turn.Reverse);
            }
            else
            {
                //Da la vuelta al camion
                te.ChangeDirection(RoadID.ReverseDirection(te.direction), Turn.Reverse);
            }


        }

    }
}