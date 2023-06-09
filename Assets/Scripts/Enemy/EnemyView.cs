//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView
{
    private Animator _animator;

    public EnemyView SetAnimator(Animator animator)
    {
        _animator = animator;
        return this;
    }
}