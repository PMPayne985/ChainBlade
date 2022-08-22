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
        private EnemySpawner _spawner;
        private ScoreUI _scoreUI;

        [SerializeField] private float attackDistance = 2.5f;
        private EnemyImpact _weapon;
        private Collider _weaponCollider;
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
        private float _lastAttackIndex;
        private bool _attacking;

        private static int _score;
        private static readonly int Dead = Animator.StringToHash("Dead");

        private void OnEnable()
        {
            _weapon = GetComponentInChildren<EnemyImpact>();
            if (!_weapon)
                Debug.LogError($"Missing Enemy Impact component.");
            
            _transform = transform;
            _controller = GetComponent<CharacterController>();
            _agent = GetComponent<NavMeshAgent>();
            _motor = GetComponent<AIMotor>();
            _animator = GetComponent<Animator>();
            _weaponCollider = _weapon.GetComponent<Collider>();
            _scoreUI = FindObjectOfType<ScoreUI>();
        }

        protected override void Start()
        {
            base.Start();
            _weaponCollider.enabled = false;
        }

        private void Update()
        {
            TargetDistance();
        }

        public void SetSpawner(EnemySpawner spawner)
        {
            _spawner = spawner;
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
            _target = dead ? null : _motor.target;
            
            if (!_target) return;
            
            var distance = Vector3.Distance(_transform.position, _target.position);

            if (!_attacking && distance <= attackDistance)
                Attack();
        }
        
        private void Attack()
        {
            _attacking = true;
            _weaponCollider.enabled = true;
            
            var randomAttackIndex = RandomAttackIndex();
            
            while (randomAttackIndex == _lastAttackIndex) 
                randomAttackIndex = RandomAttackIndex();
            
            _animator.SetTrigger(AttackTrigger);
            _animator.SetFloat(AttackIndex, randomAttackIndex);
            _lastAttackIndex = randomAttackIndex;
        }

        public void SendDamage(int amount)
        {
            _attacking = false;
            _weaponCollider.enabled = false;
        }

        public override void Death()
        {
            base.Death();
            gameObject.SetActive(false);
            
            if (_spawner)
                _spawner.ReleaseEnemy(gameObject);
            else
                GetComponent<AITargeting>().RemoveEnemy(_motor.targetedSpace);
        }
        
        private int RandomAttackIndex()
        {
            return Random.Range(0, 3);
        }

        public void EndAttack()
        {
            _weaponCollider.enabled = false;
        }

        public override void InitiateDeath()
        {
            base.InitiateDeath();
            _animator.SetBool(Dead, true);
            _score++;

            if (_scoreUI)
                _scoreUI.SetScore(_score);
        }

        public override void Revive()
        {
            base.Revive();
            dead = false;
            Health = maxHealth;
        }

        public static void ResetScore()
        {
            _score = 0;
        }
    }
}
