using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Zer0
{
    public class Player : Character
    {
        private Animator _animator;
        private ChainKnife _chainKnife;

        [SerializeField] private AudioSource combatAudio;
        [SerializeField] private AudioClip[] attackSounds;
        
        [SerializeField, Tooltip("The object that contains the functions to update UI")]
        private UISetUp ui;
        private Collider _knifeCollider;

        public bool[] TargetSpacesOccupied { get; private set; }
        
        [Tooltip("Targetable spaces for the Enemy AI")]
        public Transform[] targetSpaces;
        
        private bool _attacking;
        private int _lastAttackIndex;
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int ChainAttackTrigger = Animator.StringToHash("ChainAttack");
        private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
        private static readonly int Dead = Animator.StringToHash("Dead");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (!_animator) Debug.LogError("CharacterBehavior is missing an Animator Component.");

            _chainKnife = GetComponentInChildren<ChainKnife>();
            if (!_chainKnife) Debug.LogError("CharacterBehavior is missing a Chain Knife.");

            _knifeCollider = _chainKnife.transform.parent.GetComponentInChildren<Collider>();
            if (!_knifeCollider)
                Debug.LogError("Chain Knife is missing a collider component.");
            
            TargetSpacesOccupied = new bool[targetSpaces.Length];
        }

        protected override void Start()
        {
            base.Start();
            _knifeCollider.enabled = false;
            ui.UpdateHealthUI(Health, maxHealth);

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (PlayerInput.Attack() && !_attacking) Attack();
            
            if (PlayerInput.ChainAttack()) ChainAttack();
        }

        private void Attack()
        {
            if (combatAudio)
            {
                var random = Random.Range(0, attackSounds.Length);
                
                combatAudio.PlayOneShot(attackSounds[random]);
            }
            
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

        public override void Death()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

        public override void InitiateDeath()
        {
            base.InitiateDeath();
            GetComponent<PlayerInput>().enabled = false;
            _animator.SetBool(Dead, true);
        }

        public override void TakeDamage(float damageTaken)
        {
            base.TakeDamage(damageTaken);
            ui.UpdateHealthUI(Health, maxHealth);
        }
    }
}
