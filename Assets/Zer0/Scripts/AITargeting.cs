using UnityEngine;

namespace Zer0
{
    public class AITargeting : Targeting
    {
        private AIMotor _motor;

        protected override void Awake()
        {
            base.Awake();
            _motor = GetComponent<AIMotor>();
        }

        private void OnEnable()
        {
            AssignTarget();
        }

        public void AssignTarget()
        {
            Transform target = null;
            float distance = 0;
            int space = 0;
            
            for (var i = 0; i < Target.targetSpaces.Length; i++)
            {
                if (!Target.TargetSpacesOccupied[i])
                {
                    target = Target.targetSpaces[i];
                    distance = 0;
                    space = i;
                    Target.TargetSpacesOccupied[i] = true;
                    break;
                }
            }

            if (!target)
            {
                var randomTarget = UnityEngine.Random.Range(0, Target.targetSpaces.Length);

                target = Target.targetSpaces[randomTarget];
                distance = 6;
                space = 30;
            }
            
            _motor.SetTarget(target, distance, space);
        }

        public void RemoveEnemy(int space)
        {
            if (space < Target.targetSpaces.Length)
                Target.TargetSpacesOccupied[space] = false;
        }
    }
}
