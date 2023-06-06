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
        _playerView.OnMove(_moveDirection.x, _moveDirection.z);


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
        _moveDirection = dir;

        _moveDirection *= _speed;

        _moveDirection.Normalize();

        _rigidbody.AddForce(dir, ForceMode.Acceleration);
    }

    private bool CheckFloor()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.2f);
    }

    public void StopMovement() => _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y,0);
    

}