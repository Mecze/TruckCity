using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


public enum AIWanderState {WalkingTowards = 0, LookingTrucks = 1, LookingPoint = 2 }

public class Wander : MonoBehaviour
{
    [SerializeField]
    AIWanderState AIstate = AIWanderState.LookingTrucks;
    [SerializeField]
    float ChangeStateInterval = 3f;
    [SerializeField]
    float speed = 0.01f;
    [SerializeField]
    Animator walkAnimator;

    int TruckPicked = 0;
    int interestPicked = 0;
    int interestPickedToLook = 0;

    [SerializeField]
    List<Transform> InterestPoints;
    public List<Transform> nearbyTruck;


    bool changedState = false;
    


    void Awake()
    {
        if (InterestPoints == null) InterestPoints = new List<Transform>();
        if (nearbyTruck == null) nearbyTruck = new List<Transform>();
        StartCoroutine(newState());

    }

    

    IEnumerator newState() 
    {
        while (true)
        {
            newRandomState();
            yield return new WaitForSeconds(ChangeStateInterval);
        }


    }

    void newRandomState()
    {
        AIWanderState lastState = AIstate;
        int max = Enum.GetValues(typeof(AIWanderState)).Length;
        int random = 0;
        do
        {
            random = UnityEngine.Random.Range(0, max);
            AIstate = (AIWanderState)random;            
        } while (AIstate == lastState);
        changedState = true;


    }

    void Update()
    {
        bool b = false;
        switch (AIstate)
        {
            
            case AIWanderState.WalkingTowards:
                if (changedState)
                {
                    PickDirection();
                    walkAnimator.SetBool("Walk", true);
                }
                Vector3 v = (Vector3.forward);                
                transform.transform.Translate(v * speed * Time.deltaTime);

                break;
            case AIWanderState.LookingTrucks:
                if (nearbyTruck.Count < 1)
                {//No trucks
                    //Change state
                    newRandomState();
                    b = true;//re-True changedstate later
                    break;
                }
                //Chaned to this state or the truck we were watching left
                if (changedState || nearbyTruck.ElementAtOrDefault(TruckPicked) == null )
                {//pick new truck
                    walkAnimator.SetBool("Walk", false);
                    TruckPicked = UnityEngine.Random.Range(0, nearbyTruck.Count);
                }
                Vector3 ve = nearbyTruck[TruckPicked].position;
                ve.y = transform.position.y;
                transform.LookAt(ve);

                break;
            case AIWanderState.LookingPoint:
                if (changedState)
                {
                    PickDirectionTolook();
                    walkAnimator.SetBool("Walk", false);
                }
                break;
            default:
                break;


        }

        changedState = false;
        if (b)
        {
            b = false;
            changedState = true;
        }


    }

    void PickDirection()
    {

        int max = InterestPoints.Count;
        int last = interestPicked;        
        do
        {
            interestPicked = UnityEngine.Random.Range(0, max);            
            Vector3 v = InterestPoints[interestPicked].position;
            v.y = transform.position.y;            
            transform.LookAt(v);

        } while (interestPicked == last);        
    }
    void PickDirectionTolook()
    {

        int max = InterestPoints.Count;
        int last = interestPickedToLook;
        do
        {
            interestPickedToLook = UnityEngine.Random.Range(0, max);
            Vector3 v = InterestPoints[interestPickedToLook].position;
            v.y = transform.position.y;
            transform.LookAt(v);

        } while (interestPickedToLook == last);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "AICollider")
        {
            AIstate = AIWanderState.LookingTrucks;
            changedState = true;
        }

    }

}