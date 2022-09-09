using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Zer0
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField, Tooltip("How frequently this spawner will activate an enemy.")]
        private float spawnTime = 3;

        [SerializeField, Tooltip("The maximum number of active enemies at any one time.")]
        private int maxEnemies = 10;

        [SerializeField, Tooltip("The prefab that will be used for this enemy spawner.")]
        private GameObject enemy;

        [SerializeField, Tooltip("Check this if this spawner should spawn up to its max enemies only once. Leave unchecked to spawn endlessly.")]
        private bool spawnOnlyOnce;

        [SerializeField,
         Tooltip("Tick this if you want the first spawn to happen as soon as the spawner come online instead of after the first spawn time delay.")]
        private bool spawnOnEnable;

        private bool _stopSpawning;
        private float _timer;
        private int _activeEnemies;
        private int _totalEnemies;
        
        private List<EnemyAI> _myEnemies;
        private ObjectPool<GameObject> _enemyPool;

        private void Awake()
        {
            _enemyPool = new ObjectPool<GameObject>(CreateEnemy, OnGetEnemy, OnReleaseEnemy, OnDestroyEnemy, true, 50, 500);
        }

        private void Start()
        {
            _myEnemies = new List<EnemyAI>();
        }

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

        public void ReleaseEnemy(GameObject obj)
        {
            _enemyPool.Release(obj);
        }

        private void SpawnEnemy()
        {
            if (spawnOnlyOnce && _totalEnemies >= maxEnemies)
            {
                _stopSpawning = true;
                return;
            }
            
            if (_activeEnemies >= maxEnemies) return;

            var newEnemy = _enemyPool.Get();

            newEnemy.transform.position = transform.position;
            newEnemy.transform.rotation = transform.rotation;
            newEnemy.GetComponent<EnemyAI>().Revive();
        }

        private GameObject CreateEnemy()
        {
            var newEnemy = Instantiate(enemy, transform, false);
            newEnemy.GetComponent<EnemyAI>().SetSpawner(this);
            _myEnemies.Add(newEnemy.GetComponent<EnemyAI>());
            return newEnemy;
        }

        private void OnGetEnemy(GameObject obj)
        {
            obj.SetActive(true);
            _activeEnemies++;
            _totalEnemies++;
        }

        private void OnReleaseEnemy(GameObject obj)
        {
            obj.SetActive(false);
            
            _activeEnemies--;
        }

        private void OnDestroyEnemy(GameObject obj)
        {
            DestroyImmediate(obj);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            
        }
#endif
    }
}
