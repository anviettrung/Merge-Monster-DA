/*
 * How to use: https://www.patreon.com/posts/how-to-smoothly-53074538
*/
using UnityEngine;

public class ActiveRootMotion : StateMachineBehaviour
{
	public bool isActive = true;
	public bool resetOnStart = true;
	public bool resetOnExit = false;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (resetOnStart)
			animator.transform.SetPositionAndRotation(animator.transform.parent.position, animator.transform.parent.rotation);

		animator.applyRootMotion = isActive;
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (resetOnExit)
			animator.transform.SetPositionAndRotation(animator.transform.parent.position, animator.transform.parent.rotation);
	}
}