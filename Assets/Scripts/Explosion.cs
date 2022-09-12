using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : PoolObject
{
    Animator animator;
    // Start is called before the first frame update
    void OnEnable()
    {
        animator = GetComponent<Animator>();
        ExplosionEndedState[] behaviors = animator.GetBehaviours<ExplosionEndedState>();
        behaviors[0].ExplosionObject = this;
    }

    public void Exploded() {
        // called by ExplosionEndedState::OnStateEnter
        Die();
    }
}
