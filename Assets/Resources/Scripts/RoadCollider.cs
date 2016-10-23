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
            if (gameObject.name == "TopEndCollider")
                if (direction != CardinalPoint.N) return;
            if (gameObject.name == "BottomEndCollider")
                if (direction != CardinalPoint.S) return;
            if (gameObject.name == "LeftEndCollider")
                if (direction != CardinalPoint.W) return;
            if (gameObject.name == "RightEndCollider")
                if (direction != CardinalPoint.E) return;
            if (gameObject.name == "LeftEndCollider2")
            {
                if (direction != CardinalPoint.W) return;
                Debug.Log("I Debug Here!");
            }


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