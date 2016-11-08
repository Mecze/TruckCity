using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


public enum AIWanderState {None = 0, WalkingTowards = 1, LookingTrucks = 2, LookingPoint = 3 }
#pragma warning disable 0649
public class Wander : MonoBehaviour
{
    [Header("Avalaible Behaviours")]
    [SerializeField]
    bool WalkTowards = true;
    [SerializeField]
    bool LookingTrucks = true;
    [SerializeField]
    bool LookingPoints = true;

    [Header("Config")]
    [SerializeField]
    float ChangeStateInterval = 3f;
    [SerializeField]
    float walkSpeed = 0.01f;
    [SerializeField]
    Animator walkAnimator;

    int TruckPicked = 0;
    int interestPicked = 0;
    int interestPickedToLook = 0;

    [SerializeField]
    List<Transform> InterestPoints;
    [SerializeField]
    List<Transform> LookPoints;
    public List<Transform> nearbyTruck;

    List<AIWanderState> array;

    bool changedState = false;

    [Header("Current State (debug)")]
    [SerializeField]
    AIWanderState AIstate = AIWanderState.LookingTrucks;



    void Awake()
    {
        array = ((AIWanderState[])Enum.GetValues(typeof(AIWanderState))).ToList();
        if (!LookingPoints) array.Remove(AIWanderState.LookingPoint);
        if (!LookingTrucks) array.Remove(AIWanderState.LookingTrucks);
        if (!WalkTowards) array.Remove(AIWanderState.LookingTrucks);
        array.Remove(AIWanderState.None);

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
        int curr = array.Count;

        

        AIWanderState lastState = AIstate;
        int max = Enum.GetValues(typeof(AIWanderState)).Length;
        //int random = 0;
        //bool correctPick = false;
        if (curr > 1)
        {
            
            
                AIstate = array.FindAll(x => array[array.RandomIndex()] != lastState)[0];
            
            /*
            do
            {
                random = UnityEngine.Random.Range(1, max);
                AIstate = (AIWanderState)random;
                correctPick = true;

                if (AIstate == AIWanderState.LookingPoint && !LookingPoints) correctPick = false;
                if (AIstate == AIWanderState.LookingTrucks && !LookingTrucks) correctPick = false;
                if (AIstate == AIWanderState.WalkingTowards && !WalkTowards) correctPick = false;


            } while (AIstate == lastState || correctPick == false);
            */
        }else
        {
            if (curr == 1)AIstate = array[0];
            if (curr == 0) AIstate = AIWanderState.LookingTrucks;
        }
        changedState = true;
        //if (AIstate == AIWanderState.None) AIstate = AIWanderState.LookingTrucks;

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
                    
                }
                walkAnimator.SetBool("Walk", true);
                Vector3 v = (Vector3.forward);                
                transform.transform.Translate(v * walkSpeed * Time.deltaTime);

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
                    
                    TruckPicked = UnityEngine.Random.Range(0, nearbyTruck.Count);
                }
                walkAnimator.SetBool("Walk", false);
                Vector3 ve = nearbyTruck[TruckPicked].position;
                ve.y = transform.position.y;
                transform.LookAt(ve);

                break;
            case AIWanderState.LookingPoint:
                if (changedState)
                {
                    PickDirectionTolook();
                    
                }
                walkAnimator.SetBool("Walk", false);
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
        //Failsafes
        if (InterestPoints.Count == 1) interestPicked = 0;        
        if (InterestPoints.Count <= 1) return;        

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
        //Failsafes
        if (LookPoints.Count == 1) interestPickedToLook = 0;        
        if (LookPoints.Count <= 1) return;

        int max = LookPoints.Count;
        int last = interestPickedToLook;
        do
        {
            interestPickedToLook = UnityEngine.Random.Range(0, max);
            Vector3 v = LookPoints[interestPickedToLook].position;
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
            if (AIstate == AIWanderState.LookingTrucks && !LookingTrucks)
            {
                AIstate = AIWanderState.LookingPoint;
                changedState = false;
                if (AIstate == AIWanderState.LookingPoint && !LookingPoints) AIstate = AIWanderState.WalkingTowards;
            }
            
        }

    }

}
#pragma warning restore 0649