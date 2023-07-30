using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimValue : StateMachineBehaviour
{
    public string animParameterName;
    [Space]
    public float lowerValue;
    public float higherValue;
    public bool isRoundNumber = true;
    [Space]
    public bool playOnce = false;


    private int playCount = 0;


    public void DoRandomValue(Animator animator)
    {
        var value = Random.Range(lowerValue, higherValue);
        value = isRoundNumber ? Mathf.Round(value) : value;
        animator.SetFloat(animParameterName, value);
    }


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if ((playOnce && playCount == 0) || !playOnce)
        {
            DoRandomValue(animator);
            playCount++;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
