using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyModel : AiAgent
{
    //----Componets
    EnemyView _enemyView = new EnemyView();

    private Animator _animator;

    List<Vector3> path = new List<Vector3>();

    bool seaching = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _enemyView.SetAnimator(_animator);

        _fsm.AddState(StatesEnum.Patrol, new PatrolState(this, _obstaclesMask).SetPatrolAgentTransform(transform).SetWayPoints(_patrolNodes, _nodeArrayIndex).SetWaypointRadius(_waypointsViewRadius));

        _fsm.AddState(StatesEnum.PathFinding, new Pathfinding().SetAgent(this));
    }

    protected override void Update()
    {
        base.Update();

        _fsm.Update();

        //  if (!seaching && Tools.FieldOfView(transform.position, transform.forward, _currentNode.transform.position, FlyWeightPointer.EntityStates.viewRadius, FlyWeightPointer.EntityStates.viewAngle, _obstaclesMask))
        //  {
        //      _fsm.Update();
        //  }
        //  else
        //  {
        //       seaching = true;
        //
        //      int index = _nodeArrayIndex + 1;
        //  
        //      if(index > _patrolNodes.Length)
        //      {
        //          _nodeArrayIndex = 0;
        //      }
        //  
        //      Node goalNode = _patrolNodes[index];
        //  
        //      path = _pathFindingSystem.AStar(_currentNode, goalNode);
        //  }
        //  
        //   if (path.Count > 0)
        //   {
        //       MoveToNode();
        //   }
    }

    

    void MoveToNode()
    {  
        Vector3 target = path[0];

        ApplyForce(Seek(target));

        //transform.forward = target - transform.position;
        //transform.position += transform.forward * FlyWeightPointer.EntityStates.speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target) <= 2)
        {
            path.RemoveAt(0);
            if(path.Count == 0) seaching = false;
        }
        
    }

    #region FOV local
    //public bool InLineOfSight(Vector3 posA, Vector3 posB)
    //{
    //    Vector3 dir = posB - posA;
    //    return !Physics.Raycast(posA, dir, dir.magnitude, _obstaclesMask);
    //}
    //bool FOV(Vector3 targetPos)
    //{
    //    Vector3 dir = targetPos - transform.position;
    //
    //    if (dir.sqrMagnitude > FlyWeightPointer.EntityStates.viewRadius * FlyWeightPointer.EntityStates.viewRadius) return false; //sqr Magnitude es mas liviano que magnitud
    //
    //    if (Vector3.Angle(transform.forward, targetPos - transform.position) > FlyWeightPointer.EntityStates.viewAngle / 2) return false;
    //
    //    if (!InLineOfSight(transform.position, targetPos)) return false;
    //
    //    return true;
    //}
    #endregion
}