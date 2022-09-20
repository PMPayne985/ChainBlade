using System.Collections;
using EmeraldAI;
using Invector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zer0
{
    public class Player : Character
    {
        [SerializeField, Tooltip("Delay before reload after the character dies.")]
        private float deathDelay = 5;
        
        private ChainKnife[] _chainKnives;
        private SpellCasting _caster;

        private vHealthController _healthController;

        private void Awake()
        {
            _chainKnives = GetComponentsInChildren<ChainKnife>();
            if (_chainKnives.Length <= 0) Debug.LogError("CharacterBehavior is missing a Chain Knife.");
            _caster = GetComponent<SpellCasting>();
            if (!_caster) Debug.LogWarning("CharacterBehavior is missing a Spell Casting Component.");
            _healthController = GetComponent<vHealthController>();
            if (!_healthController) Debug.LogError("CharacterBehavior is missing a Health Controller Component.");
        }

        private void Start()
        {
            if (DebugMenu.Instance)
                DebugMenu.OnRefillHealthCommand += RestoreHealth;

            UpgradeArmorMenu.OnMaxHealthUpgrade += IncreaseMaxHealth;
        }
        
        public void LaunchChain()
        {
            foreach (var knife in _chainKnives)
            {
                knife.LaunchChain();   
            }
        }

        public override void TakeDamage (int DamageAmount, Transform attacker)
        {
            if (TryGetComponent(out Invector.vCharacterController.vCharacter character))
            {
                var _Damage = new Invector.vDamage(DamageAmount);
                _Damage.sender = attacker;
                _Damage.hitPosition = attacker.position;
                
                //Applies damage to Invector and allows its melee weapons to block incoming Emerald AI damage.
                if (TryGetComponent(out Invector.vCharacterController.vMeleeCombatInput PlayerInput))
                {
                    if (PlayerInput.meleeManager)
                    {
                        var MeleeManager = PlayerInput.meleeManager;

                        if (PlayerInput.isBlocking)
                        {
                            var DamageReduction = MeleeManager != null ? MeleeManager.GetDefenseRate() : 0;
                            if (DamageReduction > 0)
                                _Damage.ReduceDamage(DamageReduction);
                        
                            MeleeManager.OnDefense();
                            _Damage.reaction_id = MeleeManager.GetDefenseRecoilID();
                        }
                    }
                }
                
                character.TakeDamage(_Damage);
            }
        }

        public override void RestoreHealth(int value)
        {
            if (!(_healthController.currentHealth < _healthController.maxHealth)) return;
            
            _healthController.AddHealth(value);
        }

        private void IncreaseMaxHealth(int value)
        {
            _healthController.ChangeMaxHealth(value);
        }

        public void Death()
        {
            StartCoroutine(DieAfterSeconds(deathDelay));

            var allEnemies = FindObjectsOfType<EmeraldAISystem>();
            foreach (var enemy in allEnemies)
            {
                enemy.GetComponent<EmeraldAIEventsManager>().ClearTarget();
                enemy.MinMeleeAttackSpeed = 100;
                enemy.MaxMeleeAttackSpeed = 100;
            }
        }

        private IEnumerator DieAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            var thisScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(thisScene);
        }
    }
}
