//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : AiAgent
{
    //------Componets
    private EnemyView _enemyView = new EnemyView();

    private Animator _animator;

    [SerializeField]
    private LayerMask _nodeMask;

    private bool _search;

    private Vector3 _playerPos;

    private List<Vector3> path = new List<Vector3>();


    private void Awake()
    {
        //------Events
        EventManager.Subscribe(EventEnum.PlayerLocated, SeachPlayer);

        //------SetComponents
        _animator = GetComponent<Animator>();

        _target = FindObjectOfType<PlayerModel>().gameObject.transform;

        _enemyView.SetAnimator(_animator);

        _pathFindingSystem = new PathfindingState().SetAgent(this).SetLayers(_nodeMask, _obstaclesMask).SetPlayerLayer(_playerMask);

        //------Finite State machine States
        _fsm.AddState(StatesEnum.Patrol, new PatrolState(this, _obstaclesMask, _playerMask).SetPatrolAgentTransform(transform).SetWayPoints(_patrolNodes, _nodeArrayIndex).SetWaypointRadius(FlyWeightPointer.EnemiesAtributs.waypointRadius));

        _fsm.AddState(StatesEnum.PathFinding, new PathfindingState().SetAgent(this).SetLayers(_nodeMask, _obstaclesMask).SetPlayerLayer(_playerMask));
        
        _fsm.AddState(StatesEnum.Persuit , new PersuitState(this).SetAgent(this).SetPlayerMask(_playerMask));

        _fsm.AddState(StatesEnum.GoToLocation, new GoToLocationState(transform, _obstaclesMask));
    }

    protected override void Update()
    {
        base.Update();

        
       if (!_search)
        _fsm.Update();
       else //Pasar a un estado -> Esta en GoToLocation pero no lo puedo hacer andar
       {
           if(Tools.InLineOfSight(transform.position, _playerPos, _obstaclesMask))
           {
               MoveToPlayerPos();
           }
           else
           {
               if (path.Count > 0)
                   MovethroughNodes();
               else
               {
                   //_pathFindingSystem.SetGoalNode(GetNode(_playerPos));
                   Node start = GetNode(transform.position);
                   Node end = GetNode(_playerPos);
       
                   Debug.Log(this.name + " " + "Start " + start.name + " " + start);
                   Debug.Log(this.name + " " + "Goal " + end.name + " " + end);
       
                   path = _pathFindingSystem.AStar(start, end);
       
                   Debug.Log("Search path count: " + path.Count);
                   Debug.Log(this.name + " " + "Search path count: " + path.Count);
               }
       
           }
       
           if (Vector3.Distance(transform.position, _playerPos) < 5)
               _search = false;
       }
    }
    private Node GetNode(Vector3 initPos)
    {
        var nearNode = Physics.OverlapSphere(initPos, FlyWeightPointer.EnemiesAtributs.viewRadius, _nodeMask);

        Node nearestNode = null;

        float distance = 900000;

        for (int i = 0; i < nearNode.Length; i++)
        {
            if (Tools.InLineOfSight(initPos, nearNode[i].transform.position, _obstaclesMask))
            {
                RaycastHit hit;

                Vector3 dir = nearNode[i].transform.position - initPos;

                UnityEngine.Debug.Log(nearNode[i].name);

                if (Physics.Raycast(initPos, dir, out hit))
                {
                    if (hit.distance < distance)
                    {
                        distance = hit.distance;
                        nearestNode = nearNode[i].gameObject.GetComponent<Node>();
                    }
                }
            }
        }
        UnityEngine.Debug.Log("El nodo mas cercano es" + nearestNode.name);

        return nearestNode;
    }

    private void MovethroughNodes()
    {
        ApplyForce(Seek(path[0]));

        if (Vector3.Distance(transform.position, path[0]) <= 2f)
            path.RemoveAt(0);
    }

    private void SeachPlayer(params object[] parameters)
    {
        _search = true;
        _playerPos = (Vector3)parameters[0];
    }

    private void MoveToPlayerPos() => ApplyForce(Seek(_playerPos));

    private void OnDestroy() => EventManager.Unsubscribe(EventEnum.PlayerLocated, SeachPlayer);
    
}