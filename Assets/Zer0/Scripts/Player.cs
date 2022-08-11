using UnityEngine;
using Random = UnityEngine.Random;

namespace Zer0
{
    public class Player : Character
    {
        private Animator _animator;
        private ChainKnife _chainKnife;

        private UISetUp _UI;
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

            _knifeCollider = _chainKnife.transform.parent.GetComponentInChildren<Collider>();
            if (!_knifeCollider)
                Debug.LogError("Chain Knife is missing a collider component.");

            _UI = FindObjectOfType<UISetUp>();
        }

        private void Start()
        {
            base.Start();
            _knifeCollider.enabled = false;
            _UI.UpdateHealthUI(_health, maxHealth);
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

        public void EndAttack()
        {
            _knifeCollider.enabled = false;
        }

        public override void TakeDamage(float damageTaken)
        {
            base.TakeDamage(damageTaken);
            _UI.UpdateHealthUI(_health, maxHealth);
        }
    }
}
