using EmeraldAI;
using EmeraldAI.Utility;
using UnityEditor;
using UnityEngine;

namespace Zer0
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Wave Options")]
        [SerializeField, Tooltip("How frequently this spawner will activate an enemy.")]
        private float spawnTime = 3;
        [SerializeField, Tooltip("The maximum number of active enemies at any one time.")]
        private int maxEnemies = 10;
        [SerializeField, Tooltip("The prefab that will be used for this enemy spawner.")]
        private GameObject enemyPrefab;
        
        [Header("Spawn Limiting Options")]
        [SerializeField, Tooltip("Check this if this spawner should spawn up to its max enemies only once. Leave unchecked to spawn endlessly.")]
        private bool spawnOnlyOnce;
        [SerializeField, Tooltip("Check this if this spawner should spawn an enemy as soon as the spawner comes online instead of after the first spawn increment.")]
        private bool spawnOnEnable;
        [SerializeField, Tooltip("Check this if this spawner should wait until a specific kill count is reached before activating")]
        private bool spawnAfterKillCount;
        [SerializeField, Tooltip("The number of kills required before spawning starts.")]
        private int killSpawnDelay = 10;
        [SerializeField, Tooltip("Check this if this spawner should wait until a specific time has elapsed before activating")]
        private bool spawnAfterTime;
        [SerializeField, Tooltip("The time that must pass before spawning starts.")]
        private float spawnTimeDelay = 30;

        [Header("Enemy Wave Options")]
        [SerializeField, Tooltip("Increase AI Stats after this spawner has reached this number of spawned enemies.")]
        private int waveSize = 10;
        [SerializeField, Tooltip("Increase AI health by this amount per wave.")]
        private int healthIncrease = 5;
        [SerializeField, Tooltip("Increase AI damage by this amount per wave.")]
        private int damageIncrease = 3;
        
        private bool _stopSpawning;
        private float _timer;
        private int _activeEnemies;
        private int _totalEnemies;

        private int _currentHealthBoost;
        private int _currentDamageBoost;
        private int _currentLevelBoost;

        private void OnEnable()
        {
            if (spawnOnEnable)
                SpawnEnemy();
        }

        private void Update()
        {
            if (_stopSpawning) return;
            if (spawnAfterKillCount && Enemy.Score < killSpawnDelay) return;

                _timer += Time.deltaTime;
                
            if (spawnAfterTime && _timer < spawnTimeDelay) return;
                
                
            if (_timer >= spawnTime)
            {
                if (spawnAfterTime) spawnAfterTime = false;
                _timer = 0;
                SpawnEnemy();
            }
        }
        
        private void SpawnEnemy()
        {
            if (spawnOnlyOnce && _totalEnemies >= maxEnemies)
                _stopSpawning = true;
            
            if (_stopSpawning) return;
            
            if (_activeEnemies >= maxEnemies) return;

            var newEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation, transform);

            var stats = newEnemy.GetComponent<Enemy>();
            stats.SetSpawner(this);
            stats.IncreaseAIStats(_currentHealthBoost, _currentDamageBoost, _currentLevelBoost);

            _activeEnemies++;
            _totalEnemies++;

            if (_totalEnemies % waveSize == 0)
            {
                _currentLevelBoost++;
                _currentDamageBoost += damageIncrease;
                _currentHealthBoost += healthIncrease;
            }
        }

        public void DespawnEnemy(Enemy thisEnemy)
        {
            _activeEnemies--;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1);
        }
#endif
    }
}
