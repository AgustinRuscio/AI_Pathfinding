//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] [RequireComponent(typeof(Animator))]
public class PlayerModel : MonoBehaviour
{
    //------Components
    private PlayerController _playerContoller = new PlayerController();

    private Rigidbody _rigidbody;
    private Animator _animator;

    [Header("Atributs")]

    [SerializeField]
    private float _speed;

    private int _life = 3;

    private int _chains;
    private int _chainsNeeded = 5;

    [SerializeField]
    private float _jumpforce;

    private Vector3 _moveDirection;
    private Vector3 _animDirecton;

    private event Action OnJump;
    private event Action OnPuch;

    private event Action<float, float> OnMovement;


    [Header("Canvas Atributs")]
    [SerializeField]
    private TextMeshProUGUI _lifeTex;

    [SerializeField]
    private TextMeshProUGUI _chainsTex;

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

        OnPuch += _playerView.OnPuch;

        UpdateHud();
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

    public void TakeChain()
    {
        _chains++;

        UpdateHud();

        if (_chains >= _chainsNeeded)
            EventManager.Trigger(EventEnum.WinCondition);
    }

    public void TakeDamage()
    {
        OnPuch();

        _life--;
        UpdateHud();

        if (_life <= 0)
            GameOver();
    }

    private void UpdateHud()
    {
        switch (_life)
        {

            case 0:
                _lifeTex.text = "Death";
                break;

            case 1:
                _lifeTex.text = "Run for your life";
                break;

            case 2:
                _lifeTex.text = "Carefull";
                break;

            case 3:
                _lifeTex.text = "You are fine";
                break;
        }

        _chainsTex.text = _chains.ToString();
    }

    private void GameOver() => EventManager.Trigger(EventEnum.GameOver);
    

    private bool CheckFloor() => Physics.Raycast(transform.position, Vector3.down, 1.2f);
    
    public void StopMovement() => _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y,0);
}