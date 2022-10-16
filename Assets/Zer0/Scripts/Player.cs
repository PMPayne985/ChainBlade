using System.Collections;
using EmeraldAI;
using Invector;
using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zer0
{
    public class Player : Character, ISaveable
    {
        [SerializeField, Tooltip("Delay before reload after the character dies.")]
        private float deathDelay = 5;
        [SerializeField, Tooltip("The max distance a target can be retaliated against.")]
        private float retaliateRange = 1.5f;
        
        [SerializeField] private int _retaliateRate = 0;
        private int _retaliateDamage = 0;

        [SerializeField, Tooltip("All of the locations the player can enter the current scene from.")]
        private Transform[] startLocations;
        
        private ChainKnife[] _chainKnives;
        private SpellCasting _caster;
        private StatusEffects _effects;
        
        private vThirdPersonController vController;
        private vHealthController _healthController;
        
        private Animator _animator;
        private static readonly int Disarmed = Animator.StringToHash("Disarmed");

        public bool IsProtected { get; private set; }
        public float ProtectionRate { get; private set; }
        public int DefenceRate { get; private set; }

        private void Awake()
        {
            _chainKnives = GetComponentsInChildren<ChainKnife>();
            if (_chainKnives.Length <= 0) Debug.LogError("CharacterBehavior is missing a Chain Knife.");
            _caster = GetComponent<SpellCasting>();
            if (!_caster) Debug.LogWarning("CharacterBehavior is missing a Spell Casting Component.");
            _healthController = GetComponent<vHealthController>();
            if (!_healthController) Debug.LogError("CharacterBehavior is missing a Health Controller Component.");
            _effects = GetComponent<StatusEffects>();
            _animator = GetComponent<Animator>();
            vController = GetComponent<vThirdPersonController>();
            
        }

        private void Start()
        {
            if (DebugMenu.Instance)
            {
                DebugMenu.OnRefillHealthCommand += RestoreHealth;
                DebugMenu.OnDamageSelfCommand += TakeDamage;
            }

            UpgradeArmorMenu.OnMaxHealthUpgrade += IncreaseMaxHealth;
            UpgradeArmorMenu.OnDefenceUpgrade += IncreaseDefence;
            UpgradeArmorMenu.OnSpeedUpgrade += IncreaseSpeed;
            UpgradeArmorMenu.OnIncreaseRetaliation += SetRetaliateValues;
        }

        public void SetStartingPosition()
        {
            if (startLocations.Length <= 0) return;
            
            var locationIndex = SavedStats.Instance.linkedIndex;
            
            vController.enabled = false;
            transform.position = startLocations[locationIndex].position;
            transform.rotation = startLocations[locationIndex].rotation;
            vController.enabled = true;
        }

        public void SaveData()
        {
            var mHealth = _healthController.MaxHealth;
            SavedStats.Instance.maxHealth = mHealth;
            var cHealth = _healthController.currentHealth;
            SavedStats.Instance.currentHealth = cHealth;
        }

        public void LoadData()
        {
            var mHealth = SavedStats.Instance.maxHealth;
            _healthController.maxHealth = mHealth;
            var cHealth = SavedStats.Instance.currentHealth;
            _healthController.ResetHealth(cHealth);
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
        
        public override void ApplyStatusEffects(statusEffectType effectToAdd, float duration, float frequency, float magnitude)
        {
            _effects.AddActiveEffect(effectToAdd, duration, frequency, magnitude);
        }

        public void SetRetaliateValues(int dmg, int chance)
        {
            _retaliateDamage += dmg;
            _retaliateRate += chance;
        }
        
        public void Retaliate(vDamage damage)
        {
            var attacker = damage.sender;

            var distance = Vector3.Distance(transform.position, attacker.position);
            
            var chance = Random.Range(0, 100);

            if (chance > _retaliateRate || distance > retaliateRange) return;

            if (attacker.TryGetComponent(out EmeraldAISystem aiSystem))
                aiSystem.Damage(_retaliateDamage, EmeraldAISystem.TargetType.Player, transform, 50);
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

        private void IncreaseDefence(int value)
        {
            DefenceRate += value;
        }

        private void IncreaseSpeed(float value)
        {
            _animator.speed += value;
        }

        public void Protecting(float value)
        {
            IsProtected = true;
            ProtectionRate = value;
        }

        public void EndProtecting()
        {
            IsProtected = false;
            ProtectionRate = 0;
        }

        public override void Disarm(int value)
        {
            _animator.SetBool(Disarmed, true);
            foreach (var knife in _chainKnives)
            {
                knife.gameObject.SetActive(false);
            }
        }

        public override void RemoveDisarm()
        {
            _animator.SetBool(Disarmed, false);
            foreach (var knife in _chainKnives)
            {
                knife.gameObject.SetActive(true);
            }
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
