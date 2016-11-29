using UnityEngine;
using System.Collections;

public class NinetyDegreesState : StateMachineBehaviour {
    public string position;
    bool stateEntered = false;
    
    GreenRotationRoad myGreenRoad;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("NoAnim", false);
        animator.SetBool("Next", false);
        
        if (stateInfo.IsName("90_" + position + "_state"))
        {
            animator.SetBool("GoTo" + position, false);
        }

    }

  
	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!animator.IsInTransition(0) && stateEntered)
        {
            stateEntered = false;
            if (myGreenRoad == null) myGreenRoad = animator.GetComponentInParent<GreenRotationRoad>();
            myGreenRoad.UnParentTrucksOnMe(position);
        }
        
        if (animator.IsInTransition(0) && !stateEntered)
        {
            stateEntered = true;
            if (!((animator.GetCurrentAnimatorStateInfo(0).shortNameHash == stateInfo.shortNameHash))) return;            
            if (myGreenRoad == null) myGreenRoad = animator.GetComponentInParent<GreenRotationRoad>();
            myGreenRoad.ParentTrucksToMe();
        }


    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
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
