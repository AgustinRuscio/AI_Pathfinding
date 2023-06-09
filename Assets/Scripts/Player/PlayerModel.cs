//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerModel : MonoBehaviour
{
    [Header("Components")]
    
    private PlayerView _playerView = new PlayerView(); 
    private PlayerController _playerContoller = new PlayerController();

    private Rigidbody _rigidbody;
    private Animator _animator;

    [Header("Atributs")]

    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _jumpforce;

    [SerializeField]
    private Vector3 _moveDirection;

    event Action OnJump;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();

        _playerContoller.SetPlayerMode(this);
        _playerView.SetAnimator(_animator);

        OnJump += JumpLogic;
        OnJump += _playerView.OnJump;
    }

    private void Update()
    {
        _playerContoller.ArtificialUpdate();
        _playerView.OnMove(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


        if(_moveDirection == Vector3.zero)
            StopMovement();
    }

    public void Jump()
    {
        if (CheckFloor()) OnJump?.Invoke();
    }

    private void JumpLogic() => _rigidbody.AddForce(Vector3.up * _jumpforce, ForceMode.Impulse);
    

    public void Move(Vector3 dir)
    {
        _moveDirection = dir.x * transform.right;
        _moveDirection += dir.z * transform.forward;

        _moveDirection *= _speed;
        _moveDirection.Normalize();

        _rigidbody.AddForce(_moveDirection, ForceMode.Acceleration);
    }

    private bool CheckFloor() => Physics.Raycast(transform.position, Vector3.down, 1.2f);
    
    public void StopMovement() => _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y,0);
}