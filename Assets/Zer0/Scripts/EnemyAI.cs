using System;
using UnityEngine;

namespace Zer0
{
    public class EnemyAI : CharacterBehavior, IDraggable
    {
        private Transform _transform;
        private Quaternion originalRotation;

        private void Awake()
        {
            _transform = transform;
        }

        public void Drag(Transform dragger)
        {
            originalRotation = _transform.rotation;
            _transform.parent = dragger;
        }

        public void ReleaseTarget()
        {
            _transform.parent = null;
            _transform.rotation = originalRotation;
        }
    }
}
