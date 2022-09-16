using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zer0
{
    public class SpellCasting : MonoBehaviour
    {
        private List<Spell> _spells;
        private Animator _animator;
        //private UISetUp _ui;

        private float _coolDownCounter;
        private bool _onCoolDown;
        private Spell _activeSpell;
        private int _activeSpellIndex;

        public Spell testSpell;

        [SerializeField] private float maxSpellPoints = 10;
        private float _spellPoints;
        [SerializeField] private Slider magicSlider;
        [SerializeField] private float spellRechargeRate = 0.1f;
        [SerializeField] private Transform launchPoint;
        
        private static readonly int Cast = Animator.StringToHash("CastSpell");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (!_animator) Debug.LogError("SpellCasting is missing an Animator Component.");
            //_ui = FindObjectOfType<UISetUp>();
        }

        private void Start()
        {
            _spells = new List<Spell>();
            _spellPoints = maxSpellPoints;
            //_ui.SetSpellPointDisplay(_spellPoints, maxSpellPoints);

            DebugMenu.OnRefillSpellPointsCommand += RecoverSpellPoints;
            UpgradeSpellMenu.OnBuyNewSpell += AddSpell;
            UpgradeSpellMenu.OnEnhanceSpell += EnhanceSpell;
            UpgradeSpellMenu.OnChangeSpellParameters += ChangeSpellParams;
            
            AddSpell(testSpell);
            
            UpdateMagicSlider();
            magicSlider.maxValue = maxSpellPoints;
        }

        private void Update()
        {
            if (_onCoolDown)
            {
                _coolDownCounter -= Time.deltaTime;
                //_ui.SetSpellCoolDown($"{_coolDownCounter}");
                if (_coolDownCounter <= 0)
                {
                    _onCoolDown = false;
                    //_ui.SetSpellCoolDown(string.Empty);
                }
            }

            if (_spellPoints < maxSpellPoints)
            {
                _spellPoints += (spellRechargeRate * Time.deltaTime);
                UpdateMagicSlider();

                if (_spellPoints > maxSpellPoints)
                {
                    _spellPoints = maxSpellPoints;
                    UpdateMagicSlider();
                }
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
            //_ui.SetCurrentSpellInfo(_activeSpell.Name, _activeSpell.Icon);
        }
        
        public void CastSpell()
        {
            _onCoolDown = true;
            _coolDownCounter = _activeSpell.CoolDown;

            _spellPoints -= _activeSpell.Cost;
            //_ui.SetSpellPointDisplay(_spellPoints, maxSpellPoints);
            
            var cast = Instantiate(_activeSpell, launchPoint.position, launchPoint.rotation);
            cast.GetComponent<Rigidbody>().AddForce(launchPoint.forward * 5, ForceMode.Impulse);
            
            UpdateMagicSlider();
        }

        public void UpdateMagicSlider()
        {
            if (maxSpellPoints != magicSlider.maxValue)
            {
                magicSlider.maxValue = Mathf.Lerp(magicSlider.maxValue, maxSpellPoints, 2f * Time.fixedDeltaTime);
                magicSlider.onValueChanged.Invoke(magicSlider.value);
            }
            magicSlider.value = _spellPoints;
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
