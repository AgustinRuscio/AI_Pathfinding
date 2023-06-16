//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using UnityEngine;

public class PersuitState : States
{
    private AiAgent _agent;
    private EnemyModel _enemyModel;

    private LayerMask _playerMask;

    Vector3 _fisrtPosition;

    #region Builder

    public PersuitState SetAgent(AiAgent agent, EnemyModel _model)
    {
        _agent = agent;
        _enemyModel = _model;
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
        _fisrtPosition = _agent.GetTarget();
    }

    public override void Update()
    {
        if (Vector3.Distance(_agent.GetTarget(), _agent.transform.position) < 1 && _enemyModel.timer.CheckCoolDown())
            _enemyModel.PuchLogic();

        if (Tools.FieldOfView(_agent.transform.position, _agent.transform.forward, _agent.GetTarget(), FlyWeightPointer.EnemiesAtributs.viewRadius, FlyWeightPointer.EnemiesAtributs.viewAngle, _playerMask))
        {

            if (Vector3.Distance(_fisrtPosition, _agent.GetTarget()) > 5)
            {
                EventManager.Trigger(EventEnum.PlayerLocated, _agent.GetTarget());
                _fisrtPosition = _agent.GetTarget();
                Debug.Log("Pos update");
            }

            _agent.ApplyForce(_agent.Seek(_agent.GetTarget()));
        }
        else
            finiteStateMach.ChangeState(StatesEnum.Patrol);
    }

    public override void OnStop() { }

}