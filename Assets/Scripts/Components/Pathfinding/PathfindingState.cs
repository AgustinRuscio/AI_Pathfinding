//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathfindingState : States
{
    private List<Vector3> _path;

    private Node startNode;
    private Node goalNode;

    private AiAgent _agent;

    private LayerMask _nodeLayer;

    private LayerMask _playerLayer;
    private LayerMask _obstacleMask;

    #region Builder

    public PathfindingState SetAgent(AiAgent agent)
    {
        this._agent = agent;
        _agent.StatesDestructor += UnSuscribeEvents;

        return this;
    }
    public PathfindingState SetGoalNode(Node gNode) //Por si no se usa en la maquina de estados
    {
        goalNode = gNode;
        return this;
    }

    public PathfindingState SetLayers(LayerMask nodeMask, LayerMask obstacles)
    {
        _nodeLayer = nodeMask;
        _obstacleMask = obstacles;
        return this;
    }
    public PathfindingState SetPlayerLayer(LayerMask playerMask)
    {
        _playerLayer = playerMask;
        return this;
    }
    #endregion

    public override void OnStart(params object[] parameters)
    {
        goalNode = !(bool)parameters[1] ? (Node)parameters[0] : GetNode((Vector3)parameters[0]);

        startNode = GetNode(_agent.transform.position);

        //Hacer que node swe calcule segun la distancia entre nodos.
        //Overlap sphere o conseguir todos en el start
        //El indice es siemopre el goal

        _path = AStar(startNode, goalNode);

        _path.Reverse();
    }

    public Node GetNode(Vector3 initPos)
    {
        var nearNode = Physics.OverlapSphere(initPos, FlyWeightPointer.EnemiesAtributs.viewRadius, _nodeLayer);

        Node nearestNode = null;

        float distance = 900000;

        for (int i = 0; i < nearNode.Length; i++)
        {
            if (Tools.InLineOfSight(initPos, nearNode[i].transform.position, _obstacleMask))
            {
                RaycastHit hit;

                Vector3 dir = nearNode[i].transform.position - initPos;


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

        return nearestNode;
    }

    public override void Update()
    {
        if (Tools.FieldOfView(_agent.transform.position, _agent.transform.forward, _agent.GetTarget(), FlyWeightPointer.EnemiesAtributs.viewRadius, FlyWeightPointer.EnemiesAtributs.viewAngle, _playerLayer))
            finiteStateMach.ChangeState(StatesEnum.Persuit);

        Debug.Log("Estoy en pathfinding");


        if (Tools.InLineOfSight(_agent.transform.position, goalNode.transform.position, _obstacleMask))
            finiteStateMach.ChangeState(StatesEnum.Patrol);

        

        if (_path.Count > 0)
        {
            if (Vector3.Distance(_agent.transform.position, _path[0]) >= FlyWeightPointer.EnemiesAtributs.minPathfindingRecalculate)
            {
                Debug.Log("Recalculated");
                var newStart = GetNode(_agent.transform.position);
                _path = AStar(newStart, goalNode);
                _path.Reverse();
            }
            else
            {
                MovethroughNodes();
            }
        }
        else
            finiteStateMach.ChangeState(StatesEnum.Patrol);
    }

    private void MovethroughNodes()
    {
        _agent.ApplyForce(_agent.Seek(_path[0]));

        if (Vector3.Distance(_agent.transform.position, _path[0]) <= FlyWeightPointer.EnemiesAtributs.nodeDistance)
            _path.RemoveAt(0);
    }

    private void PlayerLocated(params object[] parameters)
    {
        Debug.Log("Parameter pos: "+ (Vector3)parameters[0]);
        finiteStateMach.ChangeState(StatesEnum.GoToLocation, (Vector3)parameters[0]);
    }

    public List<Vector3> AStar(Node startingNode, Node endNode)
    {
        if(startingNode == null || endNode == null) return new List<Vector3>();

        PriorityQueue<Node> frontier = new PriorityQueue<Node>();

        frontier.Enqueue(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();
        costSoFar.Add(startingNode, 0);

        Node current = null;

        while(frontier.Count != 0)
        {
            current = frontier.Dequeue();

            if (current == endNode) break;

            foreach(var next in current.GetNeighbors())
            {
                int newCost = costSoFar[current] + next.Cost;

                if (!costSoFar.ContainsKey(next))
                {
                    frontier.Enqueue(next, newCost + Heuristic(next, endNode));
                    costSoFar.Add(next, newCost);
                    cameFrom.Add(next, current);
                }
                else if(newCost < costSoFar[current])
                {
                    frontier.Enqueue(next, newCost + Heuristic(next, endNode));
                    costSoFar[next] = newCost;
                    cameFrom[next] = current;
                }
            }
        }

        if (current != endNode) return new List<Vector3>();

        List<Vector3> path = new List<Vector3>();

        while(current != startingNode)
        {
            path.Add(current.transform.position);
            current = cameFrom[current];
        }

        path.Add(startingNode.transform.position); 

        return path;
    }

    private float Heuristic(Node start, Node End) => (End.transform.position - start.transform.position).sqrMagnitude;
    

    private void UnSuscribeEvents() => EventManager.Unsubscribe(EventEnum.PlayerLocated, PlayerLocated);

    public override void OnStop() { }

   
}