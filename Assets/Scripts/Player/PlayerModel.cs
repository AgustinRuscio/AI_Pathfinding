//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerModel : MonoBehaviour
{
    [Header("Components")]
    private PlayerController _playerContoller = new PlayerController();

    private Rigidbody _rigidbody;
    private Animator _animator;

    [Header("Atributs")]

    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _jumpforce;

    private Vector3 _moveDirection;
    private Vector3 _animDirecton;

    event Action OnJump;
    event Action<float, float> OnMovement;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();


        PlayerView _playerView = new PlayerView();
        _playerView.SetAnimator(_animator);
        
        _playerContoller.SetPlayerModel(this);

        OnJump += JumpLogic;
        OnJump += _playerView.OnJump;

        OnMovement += _playerView.OnMove;
    }

    private void Update()
    {
        _playerContoller.ArtificialUpdate();

        OnMovement?.Invoke(_animDirecton.x, _animDirecton.z);

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
        _animDirecton = dir;

        _moveDirection = dir.x * transform.right;
        _moveDirection += dir.z * transform.forward;

        _moveDirection.Normalize();
        _moveDirection *= _speed;

        _rigidbody.AddForce(_moveDirection, ForceMode.Acceleration);
        //_rigidbody.velocity = new Vector3(_moveDirection.x, _rigidbody.velocity.y, _moveDirection.z) ;
    }

    private bool CheckFloor() => Physics.Raycast(transform.position, Vector3.down, 1.2f);
    
    public void StopMovement() => _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y,0);
}