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

        public Spell testSpell;

        [SerializeField] private float maxSpellPoints = 10;
        private float _spellPoints;
        [SerializeField] private float spellRechargeRate = 0.1f;
        [SerializeField] private Transform launchPoint;
        
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
            AddSpell(testSpell);
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

        public void AddSpell(Spell newSpell)
        {
            _spells.Add(newSpell);
            if (_spells.Count == 1)
                _activeSpell = _spells[0];
        }

        public void RemoveSpell(string spellName)
        {
            
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
            var cast = Instantiate(_activeSpell, launchPoint.position, launchPoint.rotation);
            cast.GetComponent<Rigidbody>().AddForce(launchPoint.forward * 5, ForceMode.Impulse);
        }
    }
}
