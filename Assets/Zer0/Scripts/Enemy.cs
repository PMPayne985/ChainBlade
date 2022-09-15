using EmeraldAI;
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
        private ScoreUI _scoreUI;

        private EnemySpawner _spawner;

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
        }

        public void SetSpawner(EnemySpawner spawner)
        {
            _spawner = spawner;
            print(spawner.gameObject.name);
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
            UpdateScore();
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
