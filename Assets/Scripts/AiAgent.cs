//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using UnityEngine;
using System;

public abstract class AiAgent : MonoBehaviour
{
    //----Componets
    protected PathfindingState _pathFindingSystem ;
    protected FiniteStateMachine _fsm = new FiniteStateMachine();


    //-----Atributs
    protected Vector3 _velocity;

    [Header("Movement")]
    [SerializeField] [Range(1, 10)]
    protected float _maxForce;

    [Header("Patrol variable")]

    [SerializeField]
    protected Node[] _patrolNodes;

    protected Node _currentNode;

    [HideInInspector]
    public int _nodeArrayIndex;

    [Header("FOV & LOS")]
    
    [SerializeField]
    protected LayerMask _obstaclesMask;
    //View radius y view angle en el Flyweight

    [SerializeField]
    protected LayerMask _playerMask;

    protected Transform _target;

    public event Action StatesDestructor;

    protected virtual void Update() => Move();
    
    public void Move()
    {
        transform.position += _velocity * Time.deltaTime;
        transform.forward = _velocity;
    }

    public Vector3 GetTarget() => _target.position;

    public Vector3 Seek(Vector3 target)
    {
        Vector3 desired = target - transform.position;

        desired.Normalize();

        desired *= FlyWeightPointer.EnemiesAtributs.speed;

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
        _velocity = Vector3.ClampMagnitude(_velocity + force, FlyWeightPointer.EnemiesAtributs.speed);
    }

    public Node SetEndNode()
    {
        //int carlos = _nodeArrayIndex + 1;
        //
        //if (carlos > _patrolNodes.Length)
        //{
        //    _nodeArrayIndex = 0;
        //}
        //
        //Node goalNode = _patrolNodes[carlos];
        //
        //return goalNode;

        return _patrolNodes[_nodeArrayIndex];
    }

    public Node GetStartNode() => _currentNode;
    

    public void SetCurrentNode(Node pos, int index)
    {
        _currentNode = pos;
        _nodeArrayIndex = index;
    }

    Vector3 DirFromAngel(float angleInDegrees) => new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));


    private void OnDestroy() => StatesDestructor?.Invoke();
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, FlyWeightPointer.EnemiesAtributs.waypointRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, FlyWeightPointer.EnemiesAtributs.viewRadius);

        Vector3 dirA = DirFromAngel(FlyWeightPointer.EnemiesAtributs.viewAngle / 2 + transform.eulerAngles.y);
        Vector3 dirB = DirFromAngel(-FlyWeightPointer.EnemiesAtributs.viewAngle / 2 + transform.eulerAngles.y);

        Gizmos.DrawLine(transform.position, transform.position + dirA.normalized * FlyWeightPointer.EnemiesAtributs.viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + dirB.normalized * FlyWeightPointer.EnemiesAtributs.viewRadius);
    }
}