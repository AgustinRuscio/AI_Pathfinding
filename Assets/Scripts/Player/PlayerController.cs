using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController
{
    private PlayerModel _playerModel;

    event Action OnPlay;

    public PlayerController SetPlayerMode(PlayerModel playerModel)
    {
        _playerModel = playerModel;
        return this;
    }

    public PlayerController()
    {
        SetMovement();
    }

    private void SetMovement()
    {
        OnPlay += JumpController;
        OnPlay += MovementController;
    }

    private void DeleteMovement()
    {
        OnPlay = null;
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
        if (Input.GetKeyDown(KeyCode.Escape))
            EventManager.Trigger(EventEnum.Pause);
    }
}