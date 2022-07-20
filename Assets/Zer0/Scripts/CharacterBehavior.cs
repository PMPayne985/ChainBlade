using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Zer0
{
    public class CharacterBehavior : MonoBehaviour
    {
        private Animator _animator;
        
        private bool cursorLock;
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int ChainAttackTrigger = Animator.StringToHash("ChainAttack");
        private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                cursorLock = !cursorLock;

                if (cursorLock)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Mouse0)) Attack();
            if (Input.GetKeyDown(KeyCode.Mouse1)) ChainAttack();
        }

        private void Attack()
        {
            var randomAttackIndex = Random.Range(0, 3);
            _animator.SetTrigger(AttackTrigger);
            _animator.SetFloat(AttackIndex, randomAttackIndex);
        }

        private void ChainAttack()
        {
            _animator.SetTrigger(ChainAttackTrigger);
        }
    }
}
