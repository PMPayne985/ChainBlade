using System;
using UnityEngine;

namespace Zer0
{
    public class CharacterAnimatorController : MonoBehaviour
    {

        private Animator _animator;
        private CharacterController _controller;

        [SerializeField] private float directionDampTime = 0.25f;
        [SerializeField] private float gravity = -9.31f;
        [SerializeField] private float rotationSpeed = 5;
        
        private float _horizontal;
        private float _vertical;
        private float _attackIndex;
        private float _rotateAngle;
        private float _rotationSpeed;
        
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Direction = Animator.StringToHash("Direction");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (!_animator)
                Debug.LogError("CharacterAnimatorController is missing an Animator component!");
            _controller = GetComponent<CharacterController>();
            if (!_controller)
                Debug.LogError("CharacterAnimatorController is missing a CharacterController component!");
        }

        private void Start()
        {
            _rotationSpeed = rotationSpeed;
        }

        private void Update()
        {
            PlayerInput();
        }

        private void FixedUpdate()
        {
            Movement();
        }

        private void PlayerInput()
        {
            _horizontal = Input.GetAxis("Horizontal");
            _vertical = Input.GetAxis("Vertical");
        }
        
        private void Movement()
        {
            var speed = Vector3.zero;

            if (!_controller.isGrounded)
                speed = new Vector3(0, gravity, 0);

            _controller.Move(speed);
            Rotation();
            
            if (!_animator)
                return;
            
            _animator.SetFloat(Speed, _vertical, directionDampTime, Time.deltaTime);
            _animator.SetFloat(Direction, _horizontal, directionDampTime, Time.deltaTime);
            
            
        }

        public void SetRotationSettings(float value)
        {
            _rotationSpeed = value * rotationSpeed;
        }
        
        private void Rotation()
        {
            _rotateAngle = _rotationSpeed * Input.GetAxis("Mouse X");
            var rotate = new Vector3(0, _rotateAngle, 0);

            transform.Rotate(rotate);
        }
    }
}
