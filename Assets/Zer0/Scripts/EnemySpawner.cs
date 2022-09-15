using EmeraldAI;
using EmeraldAI.Utility;
using UnityEditor;
using UnityEngine;

namespace Zer0
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField, Tooltip("How frequently this spawner will activate an enemy.")]
        private float spawnTime = 3;

        [SerializeField, Tooltip("The maximum number of active enemies at any one time.")]
        private int maxEnemies = 10;

        [SerializeField, Tooltip("The prefab that will be used for this enemy spawner.")]
        private GameObject enemyPrefab;

        [SerializeField, Tooltip("Check this if this spawner should spawn up to its max enemies only once. Leave unchecked to spawn endlessly.")]
        private bool spawnOnlyOnce;

        [SerializeField,
         Tooltip("Tick this if you want the first spawn to happen as soon as the spawner come online instead of after the first spawn time delay.")]
        private bool spawnOnEnable;

        private bool _stopSpawning;
        private float _timer;
        private int _activeEnemies;
        private int _totalEnemies;

        private void OnEnable()
        {
            if (spawnOnEnable)
                SpawnEnemy();
        }

        private void Update()
        {
            if (_stopSpawning) return;

                _timer += Time.deltaTime;

            if (_timer >= spawnTime)
            {
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
            
            newEnemy.GetComponent<Enemy>().SetSpawner(this);

            _activeEnemies++;
            _totalEnemies++;
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
