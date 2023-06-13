//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class PathfindingState : States
{

    private List<Vector3> _path;

    private Node startNode;
    private Node goalNode;

    private AiAgent _agent;

    private Transform _transform;

    private LayerMask _nodeLayer;

    private LayerMask _playerLayer;
    private LayerMask _obstacleMask;

    #region Builder

    public PathfindingState SetAgent(AiAgent agent)
    {
        this._agent = agent;
        _transform = this._agent.transform;
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

    public override void OnStart()
    {
        goalNode = _agent.SetEndNode();
        startNode = GetStartNode();

        //Hacer que node swe calcule segun la distancia entre nodos.
        //Overlap sphere o conseguir todos en el start
        //El indice es siemopre el goal
        UnityEngine.Debug.Log("Start node: " + startNode.name + "  "+ startNode.transform.position);
        UnityEngine.Debug.Log("Goal node: " + goalNode.name + "  "+ goalNode.transform.position);
        UnityEngine.Debug.Log("hago pathfinding");

        _path = AStar(startNode, goalNode);

        _path.Reverse();

        UnityEngine.Debug.Log("path.count: " + _path.Count);
    }

    public Node GetStartNode()
    {
        var nearNode = Physics.OverlapSphere(_agent.transform.position, FlyWeightPointer.EnemiesAtributs.viewRadius, _nodeLayer);

        Node nearestNode = null;

        float distance = 900000;

        for (int i = 0; i < nearNode.Length; i++)
        {
            if (Tools.InLineOfSight(_agent.transform.position, nearNode[i].transform.position, _obstacleMask))
            {
                RaycastHit hit;

                Vector3 dir = nearNode[i].transform.position - _agent.transform.position;

                //UnityEngine.Debug.Log(nearNode[i].name);

                if (Physics.Raycast(_agent.transform.position, dir, out hit))
                {
                    if (hit.distance < distance)
                    {
                        distance = hit.distance;
                        nearestNode = nearNode[i].gameObject.GetComponent<Node>();
                    }
                }
            }
        }
        //UnityEngine.Debug.Log( "El nodo mas cercano es"+ nearestNode.name);

        return nearestNode;
    }

    public override void Update()
    {
        if (Tools.FieldOfView(_agent.transform.position, _agent.transform.forward, _agent.GetTarget(), FlyWeightPointer.EnemiesAtributs.viewRadius, FlyWeightPointer.EnemiesAtributs.viewAngle, _playerLayer))
            finiteStateMach.ChangeState(StatesEnum.Persuit);

        UnityEngine.Debug.Log("Estoy en pathfinding");


        if (Tools.InLineOfSight(_agent.transform.position, goalNode.transform.position, _obstacleMask))
            finiteStateMach.ChangeState(StatesEnum.Patrol);


        MovethroughNodes();
    }

    private void MovethroughNodes()
    {
        _agent.ApplyForce(_agent.Seek(_path[0]));

        if (Vector3.Distance(_agent.transform.position, _path[0]) <= 2f)
        {
            _path.RemoveAt(0);
        }
    }
    public override void OnStop() { }
    
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

    private float Heuristic(Node start, Node End)
    {
        return (End.transform.position - start.transform.position).sqrMagnitude;
    }
}