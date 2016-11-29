using UnityEngine;
using System.Collections;

public class NinetyDegreesStatePurple : StateMachineBehaviour {
    public string position;
    public bool ChangeUpwards;
    [SerializeField]
    bool stateEntered = false;
    PurpleRotationRoad myPRR;
    
    CardinalPoint current
    {
        get
        {
            switch (position)
            {
                case "One":
                    return CardinalPoint.W;
                case "Two":
                    return CardinalPoint.S;
                case "Three":
                    return CardinalPoint.E;                
            }
            return CardinalPoint.None;
        }
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (myPRR == null) myPRR = animator.transform.gameObject.GetComponentInParent<PurpleRotationRoad>();
        animator.SetBool("NoAnim", false);
        animator.SetBool("Next", false);

        if (stateInfo.IsName("90_" + position + "_state"))
        {
            animator.SetBool("GoTo" + position, false);
        }
        if (ChangeUpwards) animator.SetBool("Upwards", !animator.GetBool("Upwards"));
        if (myPRR == null) return;
        if (position == "One") { myPRR.SetPurpleRoadFrame(0, ChangeUpwards); myPRR.SetMainFrame(0); }            
        if (position == "Two") { myPRR.SetPurpleRoadFrame(1, ChangeUpwards); myPRR.SetMainFrame(1); }
        if (position == "Three") { myPRR.SetPurpleRoadFrame(2, ChangeUpwards); myPRR.SetMainFrame(2); }
        myPRR.UpdateRoadDirection(current.ComposeRoadDirection(CardinalPoint.N));
    }


    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //WE EXIT TRANSITION
        if (!animator.IsInTransition(0) && stateEntered)
        {
            stateEntered = false;
            //if (!((animator.GetCurrentAnimatorStateInfo(0).shortNameHash == stateInfo.shortNameHash))) return;
            //CODE HERE            
            if (myPRR == null) return;
            myPRR.EndAnimation(animator.GetBool("Upwards"), (position =="Two"));
            //if (ChangeUpwards) animator.SetBool("Upwards", !animator.GetBool("Upwards"));

        }

        //WE ENTER INTO TRANSITION
        if (animator.IsInTransition(0) && !stateEntered)
        {
            stateEntered = true;
            if (!((animator.GetCurrentAnimatorStateInfo(0).shortNameHash == stateInfo.shortNameHash))) return;
            //CODE HERE
            if (myPRR == null) myPRR = animator.transform.gameObject.GetComponentInParent<PurpleRotationRoad>();
            if (myPRR == null) return;
            myPRR.StartAnimation(animator.GetBool("Upwards"));
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
