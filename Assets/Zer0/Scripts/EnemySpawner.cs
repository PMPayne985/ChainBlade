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

        [Header("Spawner Features")] [SerializeField, Tooltip("The method in which enemies will spawn.")]
        private SpawnOrder spawnOrder = SpawnOrder.SingleSpawner;
        [SerializeField, Tooltip("List of all spawners enemies may spawn from.")]
        private Transform[] spawnPoints;
        [SerializeField, Tooltip("The color of the wireframe that marks where this is situated.")]
        private Color gizmoColor = Color.red;
        
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
        [SerializeField, Tooltip("")] 
        private int levelIncrease = 1;
        [SerializeField, Tooltip("All of the AI abilities that this spawner should effect the damage on.")]
        private EmeraldAIAbility[] abilities;
        
        private bool _stopSpawning;
        private float _timer;
        private int _activeEnemies;
        private int _totalEnemies;
        private int _spawnIndex;

        private int _currentHealthBoost;
        private int _currentDamageBoost;
        private int _currentLevelBoost;
        
        private enum SpawnOrder
        {
            SingleSpawner,
            InOrder,
            Random,
        }

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

            var spawnPoint = transform;
            if (spawnOrder == SpawnOrder.InOrder)
            {
                spawnPoint = spawnPoints[_spawnIndex];

                _spawnIndex++;
                if (_spawnIndex >= spawnPoints.Length)
                    _spawnIndex = 0;
            }
            else if (spawnOrder == SpawnOrder.Random)
            {
                var spawnAt = Random.Range(0, spawnPoints.Length);
                spawnPoint = spawnPoints[spawnAt];
            }
            
            var newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation, transform);

            var stats = newEnemy.GetComponent<Enemy>();
            stats.SetSpawner(this);
            stats.IncreaseAIStats(_currentHealthBoost, _currentDamageBoost, _currentLevelBoost);

            _activeEnemies++;
            _totalEnemies++;

            if (_totalEnemies % waveSize == 0)
            {
                _currentLevelBoost += levelIncrease;
                _currentDamageBoost += damageIncrease;
                _currentHealthBoost += healthIncrease;
                
                if (abilities.Length <= 0) return;

                foreach (var ability in abilities)
                {
                    ability.MinAbilityDamage += damageIncrease / 2;
                    ability.MaxAbilityDamage += damageIncrease;
                }
            }
        }

        public void DespawnEnemy(Enemy thisEnemy)
        {
            _activeEnemies--;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, 1);
        }
#endif
    }
}
