//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class PatrolState : States
{
    private Node[] _waypoints;

    private Transform _transform;

    private int _currentNodeIndex = 0;

    private float _waypointsRadius;

    private AiAgent _patrolAgent;

    private Vector3 _currentWaypointPos;

    LayerMask _nodeLayer;
    LayerMask _playerLayer;

    public PatrolState(AiAgent enemy, LayerMask nodeLayer, LayerMask playerMask)
    {
        _patrolAgent = enemy;
        _nodeLayer = nodeLayer;
        _playerLayer = playerMask;
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

    public override void OnStart()
    {
        _currentNodeIndex = _patrolAgent._nodeArrayIndex;
        _patrolAgent.ApplyForce(_patrolAgent.Seek(Waypoints()));

        ChangeCurrentAiWaypoint();
    }


    public override void Update()
    {
        if (Tools.FieldOfView(_patrolAgent.transform.position, _patrolAgent.transform.forward, _patrolAgent.GetTarget(), FlyWeightPointer.EnemiesAtributs.viewRadius, FlyWeightPointer.EnemiesAtributs.viewAngle, _playerLayer))
            finiteStateMach.ChangeState(StatesEnum.Persuit);

        UnityEngine.Debug.Log("Estoy en patrol");

        if (Tools.InLineOfSight(_transform.position, _waypoints[_currentNodeIndex].transform.position, _nodeLayer))
            _patrolAgent.ApplyForce(_patrolAgent.Seek(Waypoints()));
        else
            finiteStateMach.ChangeState(StatesEnum.PathFinding);
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

    public override void OnStop() 
    {
        _currentNodeIndex = 0;
        _patrolAgent.SetEndNode();
    }
}