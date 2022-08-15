using UnityEngine;

namespace Zer0
{
    public class AITargeting : MonoBehaviour
    {
        private Player _player;
        private AIMotor _motor;

        private void Awake()
        {
            _player = FindObjectOfType<Player>();
            _motor = GetComponent<AIMotor>();
        }

        private void Start()
        {
            AssignTarget();
        }

        public void AssignTarget()
        {
            Transform target = null;
            float distance = 0;
            int space = 0;
            
            for (var i = 0; i < _player.TargetSpaces.Length; i++)
            {
                if (!_player.TargetSpacesOccupied[i])
                {
                    target = _player.TargetSpaces[i];
                    distance = 0;
                    space = i;
                    _player.TargetSpacesOccupied[i] = true;
                    break;
                }
            }

            if (!target)
            {
                var randomTarget = UnityEngine.Random.Range(0, _player.TargetSpaces.Length);

                target = _player.TargetSpaces[randomTarget];
                distance = 6;
                space = 30;
            }
            
            _motor.SetTarget(target, distance, space);
        }

        public void RemoveEnemy(int space)
        {
            if (space < _player.TargetSpaces.Length)
                _player.TargetSpacesOccupied[space] = false;
        }
    }
}
