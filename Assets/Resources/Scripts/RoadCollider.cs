using UnityEngine;
using System.Collections;

public class RoadCollider : MonoBehaviour {

    [SerializeField]
    bool endRoad = false;

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
        if (gameObject.name == "LeftCollider2")
        {
           // Debug.Log("Debug");
        }
        TruckEntity te = other.GetComponent<TruckEntity>();
        //INNER PART
        int numberOfConditions = roadDirectionCondition.Length;

        for (int i = 0; i < numberOfConditions; i++)
        {
            if (myRoadEntity.direction == roadDirectionCondition[i])
            {
                if (truckDirectionCondition[i] == te.direction)
                {
                    te.ChangeDirection(newTruckDirection[i], newTurn[i]);
                    return;
                }
            }
        }




        //ENDROADPART
        if (endRoad == false) return;
        CardinalPoint thisDirection = truckDirectionCondition[0];
        if (te.direction == thisDirection)
        {


            CardinalPoint direction = te.direction;
            RoadEntity NextTile;
            if (gameObject.name == "TopCollider")
                if (direction != CardinalPoint.N) return;
            if (gameObject.name == "BottomCollider")
                if (direction != CardinalPoint.S) return;
            if (gameObject.name == "LeftCollider")
                if (direction != CardinalPoint.W) return;
            if (gameObject.name == "RightCollider")
                if (direction != CardinalPoint.E) return;
            

            if (MapController.s.CheckNextTile(myRoadEntity.position, direction, out NextTile))
            {
                bool b = RoadEntity.CheckConnection(myRoadEntity, NextTile);
                if (!b)
                {
                    te.ChangeDirection(RoadEntity.ReverseDirection(te.direction), Turn.Reverse);
                }
            }
            else
            {
                //Da la vuelta al camion
                te.ChangeDirection(RoadEntity.ReverseDirection(te.direction), Turn.Reverse);
            }
        }


    }
}