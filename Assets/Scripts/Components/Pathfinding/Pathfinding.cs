//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Pathfinding : States
{
    private int index = 0;

    List<Vector3> path;

    Node startNode;
    Node goalNode;

    AiAgent agent;

    Transform _transform;

    #region Builder

    public Pathfinding SetAgent(AiAgent agent)
    {
        this.agent = agent;
        _transform = this.agent.transform;
        return this;
    }
    #endregion

    public override void OnStart()
    {
        agent.n++;
        UnityEngine.Debug.Log("Entro al pathFinding" + agent.n);
        index = 0;

        goalNode = agent.SetEndNode();
        startNode = agent.GetStartNode();

        path = AStar(startNode, goalNode);
    }

    public override void Update()
    {
        if (index < 0) UnityEngine.Debug.Log("Soy menos de cero");

        if (index >= path.Count)
        {
            UnityEngine.Debug.Log("aaa");
            index = 0; 
            finiteStateMach.ChangeState(StatesEnum.Patrol);
        }

        if(index >= path.Count) 

        if (Vector3.Distance(_transform.position, NextTarget(index)) > 2) //3
            agent.ApplyForce(agent.Seek(NextTarget(index)));
        else
            index++;      
    }

    private Vector3 NextTarget(int i) => path[i]; //4
    
    public override void OnStop()
    {
        agent.n++;
        UnityEngine.Debug.Log("Salgo al pathFinding" + agent.n);

        index = 0;
    }
    public List<Vector3> AStar(Node startingNode, Node goalNode)
    {
        if(startingNode == null || goalNode == null) return new List<Vector3>();

        PriorityQueue<Node> frontier = new PriorityQueue<Node>();

        frontier.Enqueue(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();
        costSoFar.Add(startingNode, 0);

        Node current = null;

        while(frontier.Count != 0)
        {
            current = frontier.Dequeue();

            if (current == goalNode) break;

            foreach(var next in current.GetNeighbors())
            {
                float newCost = costSoFar[current] + next.Cost;

                if (!costSoFar.ContainsKey(next))
                {
                    frontier.Enqueue(next, newCost + Heuristic(next, goalNode));
                    costSoFar.Add(next, newCost);
                    cameFrom.Add(next, current);
                }
                else if(newCost < costSoFar[current])
                {
                    frontier.Enqueue(next, newCost + Heuristic(next, goalNode));
                    costSoFar[next] = newCost;
                    cameFrom[next] = current;
                }
            }
        }

        if (current != goalNode) return new List<Vector3>();

        List<Vector3> path = new List<Vector3>();

        while(current != startingNode)
        {
            path.Add(current.transform.position);
            current = cameFrom[current];
        }

        path.Reverse();

        return path;
    }

    private float Heuristic(Node start, Node End)
    {
        return (start.transform.position - End.transform.position).sqrMagnitude;
    }
}