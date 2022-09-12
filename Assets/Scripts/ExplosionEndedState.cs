using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEndedState : StateMachineBehaviour
{
    public Explosion ExplosionObject;

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ExplosionObject.Exploded();
    }
}
