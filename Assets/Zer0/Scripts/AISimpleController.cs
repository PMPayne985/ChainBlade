using Polarith.Utils;
using UnityEngine;
using Polarith.AI.Move;

namespace Zer0
{
    [AddComponentMenu("Zer0 AI » Move/Character/AI Simple Controller")]
    [DisallowMultipleComponent]
    public sealed class AISimpleController : MonoBehaviour
    {
        [SerializeField, Tooltip("The direction which is used to rotate the forward direction according to the decision made by the " +
                 "'Context'.\n\n" +
                 "This vector needs to be perpendicular to an agent's forward direction, e.g., if the agent moves in the " +
                 "x/z-plane, this vector needs always to be (0, 1, 0).")]
        private Vector3 up = Vector3.up;
        
        [SerializeField, Tooltip("Determines the base value of how fast the character moves.")]
        private float speed = 1f;

        [SerializeField, Tooltip("")] private float rotationSpeed = 3;
        
        [SerializeField, Tooltip("If set equal to or greater than 0, the evaluated AI decision value is multiplied by the 'Speed'.")]
        private int objectiveAsSpeed = -1;
        
        [SerializeField, Tooltip("The AIMContext which provides the next movement direction that is applied to the agent's transform.")]
        private AIMContext context;

        [SerializeField]
        private Animator animator;
        
        private float _velocity;

        [HideInInspector] public bool atTarget;
        
        private static readonly int Speed = Animator.StringToHash("Speed");

        private void Awake()
        {
            if (!context)
                context = GetComponentInChildren<AIMContext>();

            if (!context)
                enabled = false;

            if (!animator)
                animator = GetComponent<Animator>();
            
            if (!animator)
                animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            var targetDirection = context.DecidedDirection;
            var step = rotationSpeed * Time.deltaTime;
            var newDirection = Vector3.RotateTowards(transform.forward, targetDirection, step, 0);
            
            transform.rotation = Quaternion.LookRotation(newDirection);

            float speedMultiplier = 1;
            if (Vector3.Angle(targetDirection, transform.forward) > 50)
                speedMultiplier = 0;

            if (atTarget)
                speedMultiplier = 0;

            if (objectiveAsSpeed >= 0 && objectiveAsSpeed < context.DecidedValues.Count)
            {
                float magnitude = context.DecidedValues[objectiveAsSpeed];
                magnitude = magnitude > speed ? speed : magnitude;
                _velocity = magnitude * speedMultiplier;
            }
            else
                _velocity = speed * speedMultiplier;
            
            var animatorSpeed = _velocity < speed / 2 ? 0.5f : 1;
            animatorSpeed *= speedMultiplier;
            animator.SetFloat(Speed, animatorSpeed);
            
            print(_velocity);
            transform.position += Time.deltaTime * _velocity * context.DecidedDirection;
        }
    }
}
