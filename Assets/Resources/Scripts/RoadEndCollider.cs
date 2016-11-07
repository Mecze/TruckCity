using UnityEngine;
using System.Collections;

public class RoadEndCollider : MonoBehaviour {

    [SerializeField]
    RoadEntity myRoad;

    [SerializeField]
    CardinalPoint thisDirection;

  


    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Truck") return;
        //TruckEntity te = other.GetComponent<TruckEntity>();





        /*
        if (te.direction == RoadEntity.ReverseDirection(thisDirection))
        {
            if (!myRoad.direction.HasDirection(thisDirection))
            {
                //te.ChangeDirection(RoadEntity.ReverseDirection(te.direction), Turn.Reverse);
            }
        }
        */

        /*
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


                if (MapController.s.CheckNextTile(myRoad.position, direction, out NextTile))
            {
                bool b = RoadEntity.CheckConnection(myRoad, NextTile);
                if (!b) te.ChangeDirection(RoadEntity.ReverseDirection(te.direction), Turn.Reverse);
            }
            else
            {
                //Da la vuelta al camion
                te.ChangeDirection(RoadEntity.ReverseDirection(te.direction), Turn.Reverse);
            }
        }
        */
    }


}