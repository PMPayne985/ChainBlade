using System;
using System.Collections;
using System.Collections.Generic;
using Polarith.AI.Move;
using UnityEngine;

namespace Zer0
{
    public class AIAnimationController : MonoBehaviour
    {
        private Animator _animator;
        private AIMContext _context;

        [SerializeField] 
        private float rotationSpeed = 1;
        
        [SerializeField]
        private float movementSpeed = .5f;
        
        [SerializeField, TargetObjective(true)]
        private int objectiveAsSpeed;
        
        private static readonly int Speed = Animator.StringToHash("Speed");

        private void OnEnable()
        {
            _animator = GetComponent<Animator>();
            _context = GetComponent<AIMContext>();
        }

        private void Update()
        {
            var targetDirection = _context.DecidedDirection;
            var step = rotationSpeed * Time.deltaTime;
            var newDirection = Vector3.RotateTowards(transform.forward, targetDirection, step, 0);
            
            transform.rotation = Quaternion.LookRotation(newDirection);

            float speedMultiplier = 1;
            if (Vector3.Angle(targetDirection, transform.forward) > 50)
                speedMultiplier = 0;
            
            
            if (objectiveAsSpeed >= 0 && objectiveAsSpeed < _context.DecidedValues.Count)
            {
                float magnitude = _context.DecidedValues[objectiveAsSpeed];
                magnitude = magnitude > movementSpeed ? movementSpeed : magnitude;
                _animator.SetFloat(Speed, magnitude * speedMultiplier);
            }
            else
                _animator.SetFloat(Speed,  movementSpeed * speedMultiplier);
        }
    }
}
