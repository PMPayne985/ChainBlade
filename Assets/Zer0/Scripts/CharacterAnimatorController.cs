using UnityEngine;

namespace Zer0
{
    public class CharacterAnimatorController : MonoBehaviour
    {

        private Animator _animator;
        private CharacterController _controller;

        private float _attackIndex;
        
        [SerializeField] private float directionDampTime = 0.25f;
        [SerializeField] private float gravity = -9.31f;

        //[SerializeField] private float rotationSpeed = 5;
        public float rotationSpeed;

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

        private void Update()
        {
            Movement();
        }

        private void Movement()
        {
            var speed = Vector3.zero;

            if (!_controller.isGrounded)
                speed = new Vector3(0, gravity, 0);

            _controller.Move(speed);
            
            if (!_animator)
                return;
            
            var h = Input.GetAxis("Horizontal");
            
            var v = Input.GetAxis("Vertical");

            _animator.SetFloat(Speed, v, directionDampTime, Time.deltaTime);
            _animator.SetFloat(Direction, h, directionDampTime, Time.deltaTime);

            var rotateAngle = rotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
            var rotating = new Vector3(0, rotateAngle, 0);

            transform.Rotate(rotating);
        }
    }
}
