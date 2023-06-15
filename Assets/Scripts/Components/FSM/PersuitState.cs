//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using UnityEngine;

public class PersuitState : States
{
    private AiAgent _agent;

    private LayerMask _playerMask;

    public PersuitState(AiAgent a)
    {
        _agent = a;
    }

    #region Builder

    public PersuitState SetAgent(AiAgent agent)
    {
        //_agent = agent;
        return this;
    }
    public PersuitState SetPlayerMask(LayerMask playerMask)
    {
        _playerMask = playerMask;
        return this;
    }

    #endregion

    public override void OnStart(params object[] parameters)
    {
        Debug.Log(_agent.name + "Estoy en Persuit");

        EventManager.Trigger(EventEnum.PlayerLocated, _agent.GetTarget());
    }

    public override void Update()
    {
        if (Tools.FieldOfView(_agent.transform.position, _agent.transform.forward, _agent.GetTarget(), FlyWeightPointer.EnemiesAtributs.viewRadius, FlyWeightPointer.EnemiesAtributs.viewAngle, _playerMask))
        {
            EventManager.Trigger(EventEnum.PlayerLocated, _agent.GetTarget());
            _agent.ApplyForce(_agent.Seek(_agent.GetTarget()));
        }
        else
            finiteStateMach.ChangeState(StatesEnum.Patrol);
    }

    public override void OnStop() { }

}