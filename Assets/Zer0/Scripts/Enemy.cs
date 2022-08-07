using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Zer0
{
    public class Enemy : Character, IDraggable
    {
        private Transform _transform;
        private Quaternion originalRotation;
        private float _snapToHight;
        private CharacterController _controller;
        private NavMeshAgent _agent;
        private Transform _target;
        private AIMotor _motor;
        private Animator _animator;

        [SerializeField] private float attackDistance = 2.5f;
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
        private float _lastAttackIndex;
        private bool _attacking;

        private void Awake()
        {
            _transform = transform;
            _controller = GetComponent<CharacterController>();
            _agent = GetComponent<NavMeshAgent>();
            _motor = GetComponent<AIMotor>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            TargetDistance();
        }

        public void Drag(Transform dragger)
        {
            _controller.enabled = false;
            _agent.enabled = false;
            originalRotation = _transform.rotation;
            _snapToHight = dragger.transform.position.y + 1;
            _transform.parent = dragger;
        }

        public void ReleaseTarget()
        {
            _transform.parent = null;
            _transform.rotation = originalRotation;
            var pos = _transform.position;
            _transform.position = new Vector3(pos.x, _snapToHight, pos.z);
            _controller.enabled = true;
            _agent.enabled = true;
        }

        private void TargetDistance()
        {
            _target = _motor.target;
            if (!_target) return;
            
            var distance = Vector3.Distance(_transform.position, _target.position);

            if (!_attacking && distance <= attackDistance)
                Attack();
        }
        
        private void Attack()
        {
            _attacking = true;
            
            var randomAttackIndex = RandomAttackIndex();
            
            while (randomAttackIndex == _lastAttackIndex) 
                randomAttackIndex = RandomAttackIndex();
            
            _animator.SetTrigger(AttackTrigger);
            _animator.SetFloat(AttackIndex, randomAttackIndex);
            _lastAttackIndex = randomAttackIndex;
            print("Attacking");
        }

        public void SendDamage(int amount)
        {
            _attacking = false;
            Debug.Log($"Sent {amount} damage!");
        }

        private int RandomAttackIndex()
        {
            return Random.Range(0, 3);
        }
    }
}
