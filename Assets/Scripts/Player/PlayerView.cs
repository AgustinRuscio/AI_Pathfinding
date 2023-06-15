//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView
{
    private Animator _animator;

    public PlayerView SetAnimator(Animator animator)
    {
        _animator = animator;
        return this;
    }

    public void OnMove(float x, float z)
    {
        _animator.SetFloat("x", x);
        _animator.SetFloat("z", z);
    }


    public void OnPuch() => _animator.SetTrigger("TakePunch");

    public void OnJump() => _animator.SetTrigger("Jump");
}