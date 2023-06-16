//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using UnityEngine;

public class EnemyView
{
    private Animator _animator;

    public EnemyView SetAnimator(Animator animator)
    {
        _animator = animator;
        return this;
    }

    public void Punch() => _animator.SetTrigger("Punch");
    
}