using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiAgent : MonoBehaviour
{
    //----Componets
    //protected Pathfinding _pathFindingSystem = new Pathfinding();
    protected FiniteStateMachine _fsm = new FiniteStateMachine();


    //-----Atributs
    protected Vector3 _velocity;

    [Header("Movement")]
    [SerializeField] [Range(1, 10)]
    protected float _maxForce;

    [Header("Patrol variable")]

    [SerializeField]
    protected Node[] _patrolNodes;

    [SerializeField]
    protected float _waypointsViewRadius;

    protected Node _currentNode;
    protected int _nodeArrayIndex;

    [Header("FOV & LOS")]
    
    [SerializeField]
    protected LayerMask _obstaclesMask;
    //View radius y view angle en el Flyweight

    public int n = 0;

    protected virtual void Update()
    {
        Move();
    }

    public void Move()
    {
        transform.position += _velocity * Time.deltaTime;
        transform.forward = _velocity;
    }

    public Vector3 Seek(Vector3 target)
    {
        Vector3 desired = target - transform.position;

        desired.Normalize();

        desired *= FlyWeightPointer.EntityStates.speed;

        return CalculateStreering(desired);
    }

    protected Vector3 CalculateStreering(Vector3 desired)
    {
        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce * Time.deltaTime);

        return steering;
    }

    public void ApplyForce(Vector3 force)
    {
        force.y = 0;
        _velocity = Vector3.ClampMagnitude(_velocity + force, FlyWeightPointer.EntityStates.speed);
    }

    public Node SetEndNode()
    {
        int carlos = _nodeArrayIndex + 1;

        if (carlos > _patrolNodes.Length)
        {
            _nodeArrayIndex = 0;
        }

        Node goalNode = _patrolNodes[carlos];

        return goalNode;
    }

    public Node GetStartNode()
    {
        return _currentNode;
    }


    public void SetCurrentNode(Node pos, int index)
    {
        _currentNode = pos;
        _nodeArrayIndex = index;
    }

    Vector3 DirFromAngel(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _waypointsViewRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, FlyWeightPointer.EntityStates.viewRadius);

        Vector3 dirA = DirFromAngel(FlyWeightPointer.EntityStates.viewAngle / 2 + transform.eulerAngles.y);
        Vector3 dirB = DirFromAngel(-FlyWeightPointer.EntityStates.viewAngle / 2 + transform.eulerAngles.y);

        Gizmos.DrawLine(transform.position, transform.position + dirA.normalized * FlyWeightPointer.EntityStates.viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + dirB.normalized * FlyWeightPointer.EntityStates.viewRadius);
    }
}