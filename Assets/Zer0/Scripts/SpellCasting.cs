using System.Collections.Generic;
using UnityEngine;

namespace Zer0
{
    public class SpellCasting : MonoBehaviour
    {
        private List<Spell> _spells;
        private Targeting _targeting;
        private Animator _animator;

        private float _coolDownCounter;
        private bool _onCoolDown;
        private Spell _activeSpell;
        private int _activeSpellIndex;

        [SerializeField] private float maxSpellPoints = 10;
        private float _spellPoints;
        [SerializeField] private float spellRechargeRate = 0.1f;
        
        private static readonly int Cast = Animator.StringToHash("Cast");

        private void Awake()
        {
            _targeting = GetComponent<PlayerTargeting>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _spells = new List<Spell>();
            _spellPoints = maxSpellPoints;
        }

        private void Update()
        {
            if (_onCoolDown)
            {
                _coolDownCounter -= Time.deltaTime;
                if (_coolDownCounter <= 0)
                    _onCoolDown = false;
            }

            if (_spellPoints < maxSpellPoints)
            {
                _spellPoints += (spellRechargeRate * Time.deltaTime);

                if (_spellPoints > maxSpellPoints)
                    _spellPoints = maxSpellPoints;
            }
        }

        public void AddSpell(string newName, Sprite newIcon, int newCost, float newCoolDown, float newRange, float newDuration, float newFrequency,
            float newMagnitude, statusEffectType newEffect)
        {
            var newSpell = new Spell(newName, newIcon, newCost, newCoolDown, newRange, _spells.Count, newDuration, newFrequency, newMagnitude, newEffect);
            
            _spells.Add(newSpell);
        }

        public void RemoveSpell(string spellName)
        {
            foreach (var spell in _spells)
            {
                if (spell.SpellName == spellName)
                    _spells.Remove(spell);
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
        }
        
        public void CastSpell()
        {
            if (_spells.Count <= 0 || _onCoolDown) return;
            
            if (_spellPoints < _activeSpell.Cost) return;
            
            var affected = _targeting.GetComponent<StatusEffects>();
            
            _coolDownCounter = _activeSpell.CoolDown;
            _onCoolDown = true;
            _spellPoints -= _activeSpell.Cost;
            _animator.SetTrigger(Cast);
            affected.AddActiveEffect(_activeSpell.EffectToAdd, _activeSpell.Duration, _activeSpell.Frequency, _activeSpell.Magnitude);
        }
    }
}
