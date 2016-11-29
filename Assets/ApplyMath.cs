using UnityEngine;
using System.Collections;

public class ApplyMath : StateMachineBehaviour {
    [Header("Number to Modify")]
    [SerializeField]
    string Parameter;
    [Header("By this number")]
    [SerializeField]
    string Coefficient;
    [Header("With this operation")]
    [SerializeField]
    operation Operation;
    [Header("When enter state, or exits")]
    [SerializeField]
    when When;
    [Header("Minimun value?")]
    [SerializeField]
    bool MinEnabled;
    [SerializeField]
    string MinParameter;
    [Header("Maximun value?")]
    [SerializeField]
    bool MaxEnabled;
    [SerializeField]
    string MaxParameter;

    [System.Serializable]
    protected enum operation { Add = 1, Substract = 2, Multiply = 3,Divide = 4, Equals=5 }

    [System.Serializable]
    protected enum when { Enter= 0, Exit = 1, Update = 2 }


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (When == when.Enter) DoApplyMath(animator);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (When == when.Update) DoApplyMath(animator, true);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (When == when.Exit) DoApplyMath(animator);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    void DoApplyMath(Animator a, bool update = false)
    {
        float delta = 1f;
        if (update) delta = Time.deltaTime;
        float r = a.GetFloat(Parameter);
        switch (Operation)
        {
            case operation.Add:
                r = r + (a.GetFloat(Coefficient)*delta);
                break;
            case operation.Substract:
                r = r - (a.GetFloat(Coefficient)*delta);
                break;
            case operation.Multiply:
                r = r * (a.GetFloat(Coefficient)*delta);
                break;
            case operation.Divide:
                r= r / (a.GetFloat(Coefficient)*delta);
                break;
            case operation.Equals:
                r= a.GetFloat(Coefficient);
                break;
            default:
                break;
        }

        if (MinEnabled)
        {
            if (r < a.GetFloat(MinParameter))
            {
                r = a.GetFloat(MinParameter);
            }

        }

        if (MaxEnabled) if (r > a.GetFloat(MaxParameter)) r = a.GetFloat(MaxParameter);

        a.SetFloat(Parameter, r);


    }
}
