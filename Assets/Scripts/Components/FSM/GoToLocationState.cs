using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToLocationState : States
{
    private Transform _agentLocation;
    private LayerMask _obstacleMask;

    private Vector3 _targetPos;

    public GoToLocationState(Transform agentTransform, LayerMask obstacleMask)
    {
        _agentLocation = agentTransform;
        _obstacleMask = obstacleMask;
    }

    public override void OnStart(params object[] parameters)
    {
        Debug.Log(_agentLocation.name + "Estoy en GoToLocation");
        _targetPos = (Vector3)parameters[0];
    }

    public override void Update() 
    {
        if (Tools.InLineOfSight(_agentLocation.position, _targetPos, _obstacleMask))
            finiteStateMach.ChangeState(StatesEnum.Persuit);
        else
            finiteStateMach.ChangeState(StatesEnum.PathFinding, _targetPos, true);
    }


    public override void OnStop() { }
}