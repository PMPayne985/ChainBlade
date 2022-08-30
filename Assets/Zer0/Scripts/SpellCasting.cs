using System.Collections.Generic;
using UnityEngine;

namespace Zer0
{
    public class SpellCasting : MonoBehaviour
    {
        private List<Spell> _spells;
        private Targeting _targeting;
        private Animator _animator;
        private UISetUp _ui;

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
            _ui = FindObjectOfType<UISetUp>();
        }

        private void Start()
        {
            _spells = new List<Spell>();
            _spellPoints = maxSpellPoints;
            _ui.SetSpellPointDisplay(_spellPoints, maxSpellPoints);
            AddSpell(testSpell);

            DebugMenu.OnRefillSpellPointsCommand += RecoverSpellPoints;
        }

        private void Update()
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
                _ui.SetSpellPointDisplay(_spellPoints, maxSpellPoints);

                if (_spellPoints > maxSpellPoints)
                {
                    _spellPoints = maxSpellPoints;
                    _ui.SetSpellPointDisplay(_spellPoints, maxSpellPoints);
                }
            }
        }

        public void AddSpell(Spell newSpell)
        {
            _spells.Add(newSpell);
            if (_spells.Count == 1)
                NextSpell();
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
            _ui.SetCurrentSpellInfo(_activeSpell.Name, _activeSpell.Icon);
        }
        
        public void CastSpell()
        {
            if (!CanCast()) return;
            
            _onCoolDown = true;
            _coolDownCounter = _activeSpell.CoolDown;

            _spellPoints -= _activeSpell.Cost;
            _ui.SetSpellPointDisplay(_spellPoints, maxSpellPoints);
            
            var cast = Instantiate(_activeSpell, launchPoint.position, launchPoint.rotation);
            cast.GetComponent<Rigidbody>().AddForce(launchPoint.forward * 5, ForceMode.Impulse);
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
