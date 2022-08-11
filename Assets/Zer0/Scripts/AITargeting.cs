using System.Collections.Generic;
using UnityEngine;

namespace Zer0
{
    public class AITargeting : MonoBehaviour
    {
        public List<AIMotor> _allEnemies = new List<AIMotor>();
        private Player _player;

        private void Awake()
        {
            _player = GetComponent<Player>();
            print("I am Awake!");
        }

        public void AssignTargets()
        {
            if (_allEnemies.Count == 0) return;
            
            for (var e = 0; e < _allEnemies.Count; e++)
            {
                if (_allEnemies[e].target) continue;
                
                if (e >= _player.TargetSpaces.Length)
                    _allEnemies[e].SetTarget(_player.TargetSpaces[UnityEngine.Random.Range(0, _player.TargetSpaces.Length - 1)], 6, _player.TargetSpaces.Length + 10);
                else
                {
                    for (var s = 0; s < _player.TargetSpacesOccupied.Length; s++)
                    {
                        if (_player.TargetSpacesOccupied[s]) continue;
                        
                        _allEnemies[e].SetTarget(_player.TargetSpaces[s], 0, s);
                        _player.TargetSpacesOccupied[s] = true;
                        break;
                    }
                }
            }
        }

        public void RemoveEnemy(AIMotor enemy, int space)
        {
            if (space < _player.TargetSpaces.Length)
                _player.TargetSpacesOccupied[space] = false;
            _allEnemies.Remove(enemy);
            
            AssignTargets();
        }

        public void CreateEnemyList()
        {
            _allEnemies.Clear();
            
            var enemies = FindObjectsOfType<AIMotor>();

            foreach (var enemy in enemies)
            {
                _allEnemies.Add(enemy);
            }
            
            AssignTargets();
        }

        public void AddEnemy(AIMotor enemy)
        {
            _allEnemies.Add(enemy);
            
            AssignTargets();
        }
    }
}
