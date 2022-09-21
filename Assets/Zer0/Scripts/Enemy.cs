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
        [SerializeField] private GameObject mapMarker;

        private EnemySpawner _spawner;
        [SerializeField] private GameObject weapon;

        private static int _score;
        private bool _resetThis;

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
            _score = 0;
        }

        private void RegisterDeath()
        {
            _spawner.DespawnEnemy(this);
            mapMarker.SetActive(false);
            _statusEffects.SetDeathStatus(true);
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
                attack.MinDamage += damageValue;
            }
            
            _aiEventsManager.UpdateHealth(_aiSystem.CurrentHealth + healthValue, _aiSystem.CurrentHealth + healthValue);
            _aiEventsManager.InstantlyRefillAIHealth();
            _aiSystem.AILevel += levelValue;
            _aiSystem.AILevelUI.text = $"   {_aiSystem.AILevel}";
        }
        
        public override void Disarm(int value)
        {
            _aiEventsManager.ClearTarget();
            _aiEventsManager.UpdateAIMeleeAttackSpeed(100,100);
            weapon.SetActive(false);
        }

        public override void RemoveDisarm()
        {
            _aiEventsManager.UpdateAIMeleeAttackSpeed(0,0);
            weapon.SetActive(true);
        }

        public void ResetEnemy()
        {
            
        }
        
        private void UpdateScore()
        {
            if (!_scoreUI) return;
            
            _score++;
            _scoreUI.SetScore(_score);
        }
    }
}
