//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System;
using UnityEngine;

public class PlayerController
{
    private PlayerModel _playerModel;

    event Action OnPlay;

    public PlayerController SetPlayerModel(PlayerModel playerModel)
    {
        _playerModel = playerModel;
        return this;
    }

    public PlayerController() => SetMovement();
    

    private void SetMovement()
    {
        OnPlay += JumpController;
        OnPlay += MovementController;
    }

    public void ArtificialUpdate()
    {
        OnPlay();
        PauseButton();
    }

    private void MovementController() => _playerModel.Move(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
    
    private void JumpController()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _playerModel.Jump();
    } 

    private void PauseButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.Instance.GameOverCheck)
            EventManager.Trigger(EventEnum.Pause, true);
    }
}