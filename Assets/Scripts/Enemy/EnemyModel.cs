//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System;
using UnityEngine;

public class EnemyModel : AiAgent
{
    //------Componets
    public GenericTimer timer;

    private Animator _animator;

    [SerializeField]
    private LayerMask _nodeMask;

    private Vector3 _playerPos;

    private event Action OnPunch;

    [SerializeField]
    private Collider _punchZone;

    private void Awake()
    {
        //------Events
        EventManager.Subscribe(EventEnum.PlayerLocated, SeachPlayer);

        timer = new GenericTimer(FlyWeightPointer.EnemiesAtributs.punchCoolDown);

        //------SetComponents
        _animator = GetComponent<Animator>();

        _target = FindObjectOfType<PlayerModel>().gameObject.transform;

        EnemyView _enemyView = new EnemyView();
        _enemyView.SetAnimator(_animator);
        OnPunch += _enemyView.Punch;

        //------Finite State machine States
        _fsm.AddState(StatesEnum.Patrol, new PatrolState(this, _obstaclesMask, _playerMask).SetPatrolAgentTransform(transform).SetWayPoints(_patrolNodes, _nodeArrayIndex).SetWaypointRadius(FlyWeightPointer.EnemiesAtributs.waypointRadius));

        _fsm.AddState(StatesEnum.PathFinding, new PathfindingState().SetAgent(this).SetLayers(_nodeMask, _obstaclesMask).SetPlayerLayer(_playerMask));
        
        _fsm.AddState(StatesEnum.Persuit , new PersuitState().SetAgent(this, this).SetPlayerMask(_playerMask));

        _fsm.AddState(StatesEnum.GoToLocation, new GoToLocationState(transform, _obstaclesMask));
    }

    protected override void Update()
    {
        base.Update();

        timer.RunTimer();

        _fsm.Update();

        #region old
        // if (!_search)
        //else //Pasar a un estado -> Esta en GoToLocation pero no lo puedo hacer andar
        //{
        //    if(Tools.InLineOfSight(transform.position, _playerPos, _obstaclesMask))
        //    {
        //        MoveToPlayerPos();
        //    }
        //    else
        //    {
        //        if (path.Count > 0)
        //            MovethroughNodes();
        //        else
        //        {
        //            Node start = GetNode(transform.position);
        //            Node end = GetNode(_playerPos);
        //
        //            Debug.Log(this.name + " " + "Start " + start.name + " " + start);
        //            Debug.Log(this.name + " " + "Goal " + end.name + " " + end);
        //
        //            path = _pathFindingSystem.AStar(start, end);
        //            path.Reverse();
        //
        //            Debug.Log("Search path count: " + path.Count);
        //            Debug.Log(this.name + " " + "Search path count: " + path.Count);
        //        }
        //
        //    }
        //
        //    if (Vector3.Distance(transform.position, _playerPos) < FlyWeightPointer.EnemiesAtributs.playerDistance)
        //        _search = false;
        //}
        #endregion
    }

    //private Node GetNode(Vector3 initPos)
    //{
    //    var nearNode = Physics.OverlapSphere(initPos, FlyWeightPointer.EnemiesAtributs.viewRadius, _nodeMask);
    //
    //    Node nearestNode = null;
    //
    //    float distance = 900000;
    //
    //    for (int i = 0; i < nearNode.Length; i++)
    //    {
    //        if (Tools.InLineOfSight(initPos, nearNode[i].transform.position, _obstaclesMask))
    //        {
    //            RaycastHit hit;
    //
    //            Vector3 dir = nearNode[i].transform.position - initPos;
    //
    //            UnityEngine.Debug.Log(nearNode[i].name);
    //
    //            if (Physics.Raycast(initPos, dir, out hit))
    //            {
    //                if (hit.distance < distance)
    //                {
    //                    distance = hit.distance;
    //                    nearestNode = nearNode[i].gameObject.GetComponent<Node>();
    //                }
    //            }
    //        }
    //    }
    //    UnityEngine.Debug.Log("El nodo mas cercano es" + nearestNode.name);
    //
    //    return nearestNode;
    //}

    //private void MovethroughNodes()
    //{
    //    ApplyForce(Seek(path[0]));
    //
    //    if (Vector3.Distance(transform.position, path[0]) <= FlyWeightPointer.EnemiesAtributs.waypointRadius)
    //        path.RemoveAt(0);
    //}

    //private void MoveToPlayerPos() => ApplyForce(Seek(_playerPos));

    private void SeachPlayer(params object[] parameters)
    {
        Vector3 local = (Vector3)parameters[0];

        if(Vector3.Distance(local, _playerPos) > 10)
        {
            if (_fsm.CurrentState() == StatesEnum.Patrol)
                _fsm.ChangeState(StatesEnum.GoToLocation, (Vector3)parameters[0]);
            

            if (_fsm.CurrentState() == StatesEnum.PathFinding)
                _fsm.ChangeState(StatesEnum.GoToLocation, (Vector3)parameters[0]);
            

            _playerPos = local;
        }
    }

    public void PuchLogic()
    {
        OnPunch();
        timer.ResetTimer();
    }

    #region ANIMATION_EVENTS

    private void ActivatePunch() => _punchZone.gameObject.SetActive(true);
    

    private void DeactivatePunch() => _punchZone.gameObject.SetActive(false);
    

    #endregion


    private void OnDestroy() => EventManager.Unsubscribe(EventEnum.PlayerLocated, SeachPlayer);
    
}