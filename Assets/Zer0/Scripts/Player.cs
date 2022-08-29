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
        private SpellCasting _casting;
        private StatusEffects _statusEffects;

        [SerializeField] private AudioSource combatAudio;
        [SerializeField] private AudioClip[] attackSounds;
        
        [SerializeField, Tooltip("The object that contains the functions to update UI")]
        private UISetUp ui;
        private Collider _knifeCollider;

        private bool _attacking;
        private int _lastAttackIndex;
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int ChainAttackTrigger = Animator.StringToHash("ChainAttack");
        private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
        private static readonly int IsDead = Animator.StringToHash("Dead");
        private static readonly int Cast = Animator.StringToHash("Cast");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (!_animator) Debug.LogError("CharacterBehavior is missing an Animator Component.");

            _chainKnife = GetComponentInChildren<ChainKnife>();
            if (!_chainKnife) Debug.LogError("CharacterBehavior is missing a Chain Knife.");

            _knifeCollider = _chainKnife.transform.parent.GetComponentInChildren<Collider>();
            if (!_knifeCollider)
                Debug.LogError("Chain Knife is missing a collider component.");

            _casting = GetComponent<SpellCasting>();
            if (!_casting)
                Debug.LogWarning("Player can not cast spells.");

            _statusEffects = GetComponent<StatusEffects>();
        }

        protected override void Start()
        {
            base.Start();
            _knifeCollider.enabled = false;
            ui.SetHealthDisplay(Health, maxHealth);

            Cursor.lockState = CursorLockMode.Locked;
            
            HealthCollectible.OnCollectedHealth += HealFromCollectible;
            UpgradeArmorMenu.OnMaxHealthUpgrade += IncreaseMaxHealth;
        }

        private void Update()
        {
            if (PlayerInput.Attack() && !_attacking) Attack();
            
            if (PlayerInput.ChainAttack()) ChainAttack();
            
            if (_casting)
                if (PlayerInput.NextSpell()) _casting.NextSpell();
           
            if (_casting)
                if (PlayerInput.CastSpell() && _casting.CanCast()) _animator.SetTrigger(Cast);
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
            _statusEffects.ClearAllEffects();
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
            _animator.SetBool(IsDead, true);
            _statusEffects.SetDeathStatus(dead);
        }

        public override void TakeDamage(float damageTaken)
        {
            base.TakeDamage(damageTaken);
            ui.SetHealthDisplay(Health, maxHealth);
        }

        public override void RecoverHealth(float healingDone)
        {
            base.RecoverHealth(healingDone);
            ui.SetHealthDisplay(Health, maxHealth);
        }

        public override void IncreaseMaxHealth(float healthToAdd)
        {
            base.IncreaseMaxHealth(healthToAdd);
            ui.SetHealthDisplay(Health, maxHealth);
        }

        private void HealFromCollectible(HealthCollectible collectible) 
            => RecoverHealth(collectible.HealthToRestore);
    }
}
