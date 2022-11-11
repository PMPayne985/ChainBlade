using System;
using UnityEngine;

namespace Zer0
{
    public class RunningNPC : MonoBehaviour
    {
        [SerializeField] private float distance;
        private Vector3 _startingPoint;
        
        private Animator _animator;
        private static readonly int Idle = Animator.StringToHash("Idle");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _startingPoint = transform.position;
        }

        private void Update()
        {
            if (Vector3.Distance(_startingPoint, transform.position) >= distance)
                _animator.SetBool(Idle, true);
        }
    }
}
