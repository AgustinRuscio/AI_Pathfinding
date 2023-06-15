//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using UnityEngine;


public class PatrolState : States
{
    private Node[] _waypoints;

    private Transform _transform;

    private int _currentNodeIndex = 0;

    private float _waypointsRadius;

    private AiAgent _patrolAgent;

    LayerMask _nodeLayer;
    LayerMask _playerLayer;

    public PatrolState(AiAgent enemy, LayerMask nodeLayer, LayerMask playerMask)
    {
        //EventManager.Subscribe(EventEnum.PlayerLocated, PlayerLocated);

        _patrolAgent = enemy;
        _nodeLayer = nodeLayer;
        _playerLayer = playerMask;

        _patrolAgent.StatesDestructor += UnSuscribeEvents;
    }

    #region Builder

    public PatrolState SetPatrolAgentTransform(Transform transform)
    {
        _transform = transform;
        return this;
    }
    public PatrolState SetWayPoints(Node[] waypoints, int ariop)
    {
        _waypoints = waypoints;
        _currentNodeIndex = ariop;

        if(_currentNodeIndex > _waypoints.Length) _currentNodeIndex = 0;

        return this;
    }

    public PatrolState SetWaypointRadius(float waypointsRadius)
    {
        _waypointsRadius = waypointsRadius;
        return this;
    }

    #endregion

    public override void OnStart(params object[] parameters)
    {
        _currentNodeIndex = _patrolAgent._nodeArrayIndex;
        _patrolAgent.ApplyForce(_patrolAgent.Seek(Waypoints()));

        ChangeCurrentAiWaypoint();
    }


    public override void Update()
    {
        if (Tools.FieldOfView(_patrolAgent.transform.position, _patrolAgent.transform.forward, _patrolAgent.GetTarget(), FlyWeightPointer.EnemiesAtributs.viewRadius, FlyWeightPointer.EnemiesAtributs.viewAngle, _playerLayer))
            finiteStateMach.ChangeState(StatesEnum.Persuit);

        Debug.Log("Estoy en patrol");

        if (Tools.InLineOfSight(_transform.position, _waypoints[_currentNodeIndex].transform.position, _nodeLayer))
            _patrolAgent.ApplyForce(_patrolAgent.Seek(Waypoints()));
        else
            finiteStateMach.ChangeState(StatesEnum.PathFinding, _patrolAgent.SetEndNode(), false);
    }

    private void ChangeCurrentAiWaypoint()
    {
        _patrolAgent.SetCurrentNode(_waypoints[_currentNodeIndex], _currentNodeIndex);
    }

    private Vector3 Waypoints()
    {
        if (Vector3.Distance(_transform.position, _waypoints[_currentNodeIndex].transform.position) <= _waypointsRadius)
        {
            _currentNodeIndex++;

            if (_currentNodeIndex >= _waypoints.Length)
                _currentNodeIndex = 0;
        }

        ChangeCurrentAiWaypoint();
        return _waypoints[_currentNodeIndex].transform.position;
    }


    private void PlayerLocated(params object[] parameters) => finiteStateMach.ChangeState(StatesEnum.GoToLocation, (Vector3)parameters[0]);
    

    public override void OnStop() 
    {
        _currentNodeIndex = 0;
        _patrolAgent.SetEndNode();
    }

    private void UnSuscribeEvents() => EventManager.Unsubscribe(EventEnum.PlayerLocated, PlayerLocated);
    
}