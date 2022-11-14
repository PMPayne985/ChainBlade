using System;
using EmeraldAI;
using EmeraldAI.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Zer0
{
    public class Enemy : Character, IDraggable
    {
        private Transform _transform;
        private Quaternion originalRotation;
        private float _snapToHight;
        private NavMeshAgent _agent;
        private Transform _target;
        private EmeraldAISystem _aiSystem;
        private EmeraldAIEventsManager _aiEventsManager;
        private ScoreUI _scoreUI;
        private StatusEffects _statusEffects;
        private Animator _animator;
        
        [SerializeField] private GameObject mapMarker;

        private EnemySpawner _spawner;
        [SerializeField] private GameObject weapon;

        public static event Action<string> OnDeathQuestUpdate;
        
        public static int Score { get; private set; }
        private bool _resetThis;
        private static readonly int Disarmed = Animator.StringToHash("Disarmed");

        private void Start()
        {
            _aiSystem.DeathEvent.AddListener(RegisterDeath);
        }
        
        private void Awake()
        {
            _transform = transform;
            _agent = GetComponent<NavMeshAgent>();
            _aiSystem = GetComponent<EmeraldAISystem>();
            _scoreUI = FindObjectOfType<ScoreUI>();
            _statusEffects = GetComponent<StatusEffects>();
            _aiEventsManager = GetComponent<EmeraldAIEventsManager>();
            _animator = GetComponent<Animator>();
        }

        public void SetSpawner(EnemySpawner spawner)
        {
            _spawner = spawner;
        }
        
        public void Drag(Transform dragger)
        {
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
            _agent.enabled = true;
        }

        public static void ResetScore()
        {
            Score = 0;
        }

        private void RegisterDeath()
        {
            if (_spawner) _spawner.DespawnEnemy(this);
            mapMarker.SetActive(false);
            _statusEffects.SetDeathStatus(true);
            OnDeathQuestUpdate.Invoke(_aiSystem.AIName);
            UpdateScore();
        }

        public override void TakeDamage(int damage, Transform attacker)
        {
            _aiSystem.Damage(damage, EmeraldAISystem.TargetType.AI, attacker, 400, false);
        }

        public override void RestoreHealth(int value)
        {
            _aiEventsManager.UpdateHealth(_aiSystem.StartingHealth, _aiSystem.CurrentHealth + value);
        }

        public void IncreaseAIStats(int healthValue, int damageValue, int levelValue)
        {
            foreach (var attack in _aiSystem.MeleeAttacks)
            {
                attack.MaxDamage += damageValue;
                attack.MinDamage += damageValue / 2;
            }
            
            _aiEventsManager.UpdateHealth(_aiSystem.CurrentHealth + healthValue, _aiSystem.CurrentHealth + healthValue);
            _aiEventsManager.InstantlyRefillAIHealth();
            _aiSystem.AILevel += levelValue;
            _aiSystem.AILevelUI.text = $"   {_aiSystem.AILevel}";
        }
        
        public override void Disarm(int value)
        {
            _animator.SetBool(Disarmed, true);
            weapon.SetActive(false);
        }

        public override void RemoveDisarm()
        {
            _animator.SetBool(Disarmed, false);
            weapon.SetActive(true);
        }

        public void ResetEnemy()
        {
            
        }
        
        private void UpdateScore()
        {
            if (!_scoreUI) return;
            
            Score++;
            _scoreUI.SetScore(Score);
        }
    }
}
