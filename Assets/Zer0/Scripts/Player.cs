using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Zer0
{
    public class Player : Character
    {
        private Animator _animator;
        private ChainKnife _chainKnife;
        private Collider _knifeCollider;
        
        private bool cursorLock;
        private bool _attacking;
        private int _lastAttackIndex;
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int ChainAttackTrigger = Animator.StringToHash("ChainAttack");
        private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (!_animator) Debug.LogError("CharacterBehavior is missing an Animator Component.");

            _chainKnife = GetComponentInChildren<ChainKnife>();
            if (!_chainKnife) Debug.LogError("CharacterBehavior is missing a Chain Knife.");

            _knifeCollider = _chainKnife.GetComponent<Collider>();
        }

        private void Start()
        {
            _knifeCollider.enabled = false;
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
            
            if (Input.GetKeyDown(KeyCode.Mouse0) && !_attacking) Attack();
            
            if (Input.GetKeyDown(KeyCode.Mouse1)) ChainAttack();
        }

        private void Attack()
        {
            _attacking = true;
            _knifeCollider.enabled = true;
            
            var randomAttackIndex = RandomAttackIndex();
            
            while (randomAttackIndex == _lastAttackIndex) 
                randomAttackIndex = RandomAttackIndex();
            
            _animator.SetTrigger(AttackTrigger);
            _animator.SetFloat(AttackIndex, randomAttackIndex);
            _lastAttackIndex = randomAttackIndex;
        }

        public void SendDamage(int amount)
        {
            _attacking = false;
            _knifeCollider.enabled = false;
        }

        private int RandomAttackIndex()
        {
            return Random.Range(0, 3);
        }
        
        private void ChainAttack()
        {
            _animator.SetTrigger(ChainAttackTrigger);
        }

        public void LaunchChain()
        {
            _chainKnife.LaunchChain();
        }
    }
}
