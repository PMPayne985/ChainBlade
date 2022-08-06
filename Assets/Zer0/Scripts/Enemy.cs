using System;
using UnityEngine;

namespace Zer0
{
    public class Enemy : Character, IDraggable
    {
        private Transform _transform;
        private Quaternion originalRotation;
        private float _snapToHight;
        private CharacterController _controller;
        private float _gravity = -9.31f;

        private void Awake()
        {
            _transform = transform;
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Movement();
        }

        public void Drag(Transform dragger)
        {
            _controller.enabled = false;
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
            _controller.enabled = true;
        }

        private void Movement()
        {
            if (_controller.isGrounded) return;
            
            var speed = Vector3.zero;

            if (!_controller.isGrounded)
                speed = new Vector3(0, _gravity, 0);

            _controller.Move(speed);
        }
        
    }
}
