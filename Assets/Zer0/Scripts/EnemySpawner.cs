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

        private float _timer;
        private int _totalEnemies;

        [SerializeField, Tooltip("The prefab that will be used for this enemy spawner.")]
        private GameObject enemy;

        private List<Enemy> _myEnemies;

        private ObjectPool<GameObject> _enemyPool;

        private void Awake()
        {
            _enemyPool = new ObjectPool<GameObject>(CreateEnemy, OnGetEnemy, OnReleaseEnemy, OnDestroyEnemy, true, 50, 500);
        }

        private void Start()
        {
            _myEnemies = new List<Enemy>();
        }

        private void Update()
        {
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
            if (_totalEnemies >= maxEnemies) return;

            var newEnemy = _enemyPool.Get();

            newEnemy.transform.position = transform.position;
            newEnemy.transform.rotation = transform.rotation;
            newEnemy.GetComponent<Enemy>().Revive();
        }

        private GameObject CreateEnemy()
        {
            var newEnemy = Instantiate(enemy, transform, false);
            newEnemy.GetComponent<Enemy>().SetSpawner(this);
            _myEnemies.Add(newEnemy.GetComponent<Enemy>());
            return newEnemy;
        }

        private void OnGetEnemy(GameObject obj)
        {
            obj.SetActive(true);
            _totalEnemies++;
        }

        private void OnReleaseEnemy(GameObject obj)
        {
            obj.SetActive(false);

            foreach (var thisEnemy in _myEnemies)
            {
                if (thisEnemy.gameObject.activeInHierarchy)
                    thisEnemy.ResetTargeting();
            }
            _totalEnemies--;
        }

        private void OnDestroyEnemy(GameObject obj)
        {
            DestroyImmediate(obj);
        }
    }
}
