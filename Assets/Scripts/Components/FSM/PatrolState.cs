//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class PatrolState : States
{
    private Node[] _waypoints;

    private Transform _transform;

    private int _currentNodeIndex = 0;

    private float _waypointsRadius;

    private AiAgent _patrolAgent;

    private Vector3 _currentWaypointPos;
    LayerMask _layerMask;

    public PatrolState(AiAgent enemy, LayerMask a)
    {
        _patrolAgent = enemy;
        _layerMask = a;
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
        _currentNodeIndex = 0;
        _patrolAgent.ApplyForce(_patrolAgent.Seek(Waypoints()));


        _patrolAgent.n++;
        Debug.Log("Entro al patrol" + _patrolAgent.n);
        ChangeCurrentAiWaypoint();
    }


    public override void Update()
    {
        if (Tools.FieldOfView(_transform.position, _transform.forward, _waypoints[_currentNodeIndex].transform.position, FlyWeightPointer.EntityStates.viewRadius, FlyWeightPointer.EntityStates.viewAngle, _layerMask))
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
        _patrolAgent.n++;
        Debug.Log("salgo al patrol" + _patrolAgent.n);

        _currentNodeIndex = 0;
        _patrolAgent.SetEndNode();
    }
}