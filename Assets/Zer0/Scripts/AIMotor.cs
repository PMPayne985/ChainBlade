using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class AIMotor : MonoBehaviour
{
    private Vector3 _worldDeltaPosition = Vector3.zero;
    private Vector3 _position = Vector3.zero;
    private NavMeshAgent _agent;
    private Animator _animator;
    private CharacterController _controller;
    private float _gravity = -9.31f;

    [SerializeField, Tooltip("The distance from the target the AI will stop.")]
    protected float stopDistance = 2.5f;
    
    public Transform target;
    
    private static readonly int Speed = Animator.StringToHash("Speed");

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _agent.updatePosition = false;
    }

    private void Update()
    {
        _worldDeltaPosition = _agent.nextPosition - transform.position;

        if (_worldDeltaPosition.magnitude > _agent.radius) 
        {
            _agent.nextPosition = transform.position + .9f * _worldDeltaPosition;
            _agent.SetDestination(target.position);
        }

        _animator.SetFloat(Speed, Vector3.Distance(transform.position, target.position) > _agent.radius * stopDistance ? 1 : 0);
        
        Movement();
    }

    private void OnAnimatorMove()
    {
        _position = _animator.rootPosition;
        _position.y = _agent.nextPosition.y;
        transform.position = _position;
    }
    
    private void Movement()
    {
        if (_controller.isGrounded) return;
            
        var speed = Vector3.zero;

        if (!_controller.isGrounded)
            speed = new Vector3(0, _gravity, 0);

        _controller.Move(speed);
    }
}
