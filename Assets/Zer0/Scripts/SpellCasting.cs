using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zer0
{
    public class SpellCasting : MonoBehaviour
    {
        private List<Spell> _spells;
        private Animator _animator;
        private UISetUp _ui;
        private Character _character;

        private float _coolDownCounter;
        private bool _onCoolDown;
        private Spell _activeSpell;
        private int _activeSpellIndex;

        [SerializeField] private float maxSpellPoints = 10;
        [SerializeField] private float _spellPoints;
        [SerializeField] private float spellRechargeRate = 0.1f;
        [SerializeField] private Transform launchPoint;

        private static readonly int Cast = Animator.StringToHash("CastSpell");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (!_animator) Debug.LogError("SpellCasting is missing an Animator Component.");
            _ui = FindObjectOfType<UISetUp>();
            _character = GetComponent<Character>();
        }

        private void Start()
        {
            _spells = new List<Spell>();
            _spellPoints = maxSpellPoints;

            DebugMenu.OnRefillSpellPointsCommand += RecoverSpellPoints;
            UpgradeSpellMenu.OnBuyNewSpell += AddSpell;
            UpgradeSpellMenu.OnEnhanceSpell += EnhanceSpell;
            UpgradeSpellMenu.OnChangeSpellParameters += ChangeSpellParams;

            _ui.UpdateMagicSlider(maxSpellPoints, _spellPoints);
            _ui.SetSliderMax(maxSpellPoints);
        }

        private void Update()
        {
            if (_character.isPlayer)
                PlayerCasting();
        }

        private void PlayerCasting()
        {
            if (_onCoolDown)
            {
                _coolDownCounter -= Time.deltaTime;
                _ui.SetSpellCoolDown($"{_coolDownCounter}");
                if (_coolDownCounter <= 0)
                {
                    _onCoolDown = false;
                    _ui.SetSpellCoolDown(string.Empty);
                }
            }

            if (_spellPoints < maxSpellPoints)
            {
                _spellPoints += (spellRechargeRate * Time.deltaTime);
                

                if (_spellPoints > maxSpellPoints)
                {
                    _spellPoints = maxSpellPoints;
                }
                _ui.UpdateMagicSlider(maxSpellPoints, _spellPoints);
            }
            
            if (PlayerInput.CastSpell() && CanCast())
            {
                _animator.SetTrigger(Cast);
            }
        }
        
        public void AddSpell(Spell newSpell)
        {
            _spells.Add(newSpell);
            if (_spells.Count == 1)
                NextSpell();
        }

        public void EnhanceSpell(string spellName, float newDuration, float newFrequency, float newMagnitude,
            int newImpactDamage, statusEffectType newEffect, bool stationary)
        {
            foreach (var thisSpell in _spells)
            {
                if (thisSpell.Name == spellName)
                {
                    thisSpell.SetSpellEffect(newDuration, newFrequency, newMagnitude, newImpactDamage, newEffect, stationary);
                    return;
                }
            }
        }

        public void ChangeSpellParams(string newName, Sprite newIcon, int newCost, float newCooldown, float newRange,
            areaOfEffect newAoe)
        {
            foreach (var thisSpell in _spells)
            {
                if (thisSpell.Name == newName)
                {
                    thisSpell.SetSpellVariables(newName, newIcon, newCost, newCooldown, newRange, newAoe);
                    return;
                }
            }
        }
        
        public void RemoveSpell(string spellName)
        {
            foreach (var thisSpell in _spells)
            {
                if (thisSpell.Name == spellName)
                {
                    _spells.Remove(thisSpell);
                    return;
                }
            }
        }

        public void RecoverSpellPoints(float amount)
        {
            _spellPoints += amount;
            
            if (_spellPoints > maxSpellPoints)
                _spellPoints = maxSpellPoints;
        }
        
        public void NextSpell()
        {
            if (_spells.Count <=  0 || _onCoolDown) return;

            _activeSpellIndex++;

            if (_activeSpellIndex >= _spells.Count)
                _activeSpellIndex = 0;

            _activeSpell = _spells[_activeSpellIndex];
            _ui.SetCurrentSpellInfo(_activeSpell.Name, _activeSpell.Icon);
        }
        
        public void CastSpell()
        {
            if (_character.isPlayer)
                Casting();
        }
        
        private void Casting()
        {
            _onCoolDown = true;
            _coolDownCounter = _activeSpell.CoolDown;
            
            _spellPoints -= _activeSpell.Cost;

            var cast = Instantiate(_activeSpell, launchPoint.position, launchPoint.rotation);
            cast.GetComponent<Rigidbody>().AddForce(launchPoint.forward * 5, ForceMode.Impulse);
            
            _ui.UpdateMagicSlider(maxSpellPoints, _spellPoints);
        }

        public bool CanCast()
        {
            if (_spells.Count <= 0)
            {
                Logging.LogMessage(errorLevel.Log, Color.blue, "You don't know any spells!");
                return false;
            }
            
            if (_onCoolDown)
            {
                Logging.LogMessage(errorLevel.Log, Color.blue, "On cooldown!");
                return false;
            }
            
            if (_activeSpell.Cost > _spellPoints)
            {
                Logging.LogMessage(errorLevel.Log, Color.blue, "Not Enough spell points!");
                return false;
            }

            return true;
        }
    }
}
