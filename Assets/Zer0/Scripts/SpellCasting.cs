using System.Collections.Generic;
using UnityEngine;

namespace Zer0
{
    public class SpellCasting : MonoBehaviour, ISaveable
    {
        private List<SpellData> _spells;
        private Animator _animator;
        private UISetUp _ui;
        private Character _character;

        private float _coolDownCounter;
        private bool _onCoolDown;
        private SpellData _activeSpell;
        private int _activeSpellIndex;

        [SerializeField] private float maxSpellPoints = 10;
        private float _spellPoints;
        [SerializeField] private float spellRechargeRate = 0.1f;
        [SerializeField] private Transform launchPoint;
        [SerializeField] private GameObject spellTemplatePrefab;

        private static readonly int Cast = Animator.StringToHash("CastSpell");
        private static readonly int CastID = Animator.StringToHash("CastID");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (!_animator) Debug.LogError("SpellCasting is missing an Animator Component.");
            _ui = FindObjectOfType<UISetUp>();
            _character = GetComponent<Character>();
        }

        private void Start()
        {
            _spells = new List<SpellData>();
            _spellPoints = maxSpellPoints;

            DebugMenu.OnRefillSpellPointsCommand += RecoverSpellPoints;
            UpgradeSpellMenu.OnBuyNewSpell += AddSpell;
            UpgradeSpellMenu.OnEnhanceSpell += EnhanceSpell;
            UpgradeSpellMenu.OnChangeSpellParameters += ChangeSpellParams;
            UpgradeSpellMenu.OnAddSpellPoints += IncreaseMaxSpellPoints;

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
                _animator.SetFloat(CastID, _activeSpell.CastOnSelf ? 1 : 0);
                _animator.SetTrigger(Cast);
            }

            if (PlayerInput.NextSpell()) NextSpell();
        }

        public void AddSpell(SpellData newSpell)
        {
            var addSpell = gameObject.AddComponent<SpellData>();
            addSpell.SetSpellEffect(newSpell.Duration, newSpell.Frequency, newSpell.Magnitude, newSpell.ImpactDamage, newSpell.EffectToAdd, newSpell.EffectStationary);
            addSpell.SetSpellVariables(newSpell.Name, newSpell.Icon,newSpell.Cost, newSpell.CoolDown, newSpell.Range, newSpell.AOE);
            addSpell.SetSpellParams(newSpell.CastOnSelf, newSpell.VisualEffect, newSpell.ExplosionSpeed, newSpell.TrailEffect);
            _spells.Add(addSpell);
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
            if (!_character.isPlayer) return;
            
            _spellPoints += amount;
            
            if (_spellPoints > maxSpellPoints)
                _spellPoints = maxSpellPoints;
        }

        private void IncreaseMaxSpellPoints(float amount)
        {
            if (!_character.isPlayer) return;
            
            maxSpellPoints += amount;
            RecoverSpellPoints(amount);
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

            if (_activeSpell.CastOnSelf)
            {
                _character.ApplyStatusEffects(_activeSpell.EffectToAdd, _activeSpell.Duration, _activeSpell.Frequency, _activeSpell.Magnitude);
                if (_activeSpell.VisualEffect) _activeSpell.VisualEffect.SetActive(true);
            }
            else
            {
                var cast = CreateSpell();
                cast.GetComponent<Rigidbody>().AddForce(launchPoint.forward * 5, ForceMode.Impulse);
            }
            
            _ui.UpdateMagicSlider(maxSpellPoints, _spellPoints);
        }

        private GameObject CreateSpell()
        {
            var cast = Instantiate(spellTemplatePrefab, launchPoint.position, launchPoint.rotation);
            var data = cast.GetComponent<Spell>();
            data.SetSpellEffect(_activeSpell.Duration, _activeSpell.Frequency, _activeSpell.Magnitude, _activeSpell.ImpactDamage, _activeSpell.EffectToAdd, _activeSpell.EffectStationary);
            data.SetSpellVariables(_activeSpell.Name, _activeSpell.Icon,_activeSpell.Cost, _activeSpell.CoolDown, _activeSpell.Range, _activeSpell.AOE);
            data.SetSpellParams(_activeSpell.CastOnSelf, _activeSpell.VisualEffect, _activeSpell.ExplosionSpeed, _activeSpell.TrailEffect);
            Instantiate(data.trailEffect, cast.transform.position, cast.transform.rotation, cast.transform);

            return cast;
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

        public void SaveData()
        {
            SavedStats.Instance.maxSpellPoints = maxSpellPoints;
            SavedStats.Instance.currentSpellPoints = _spellPoints;

            SavedStats.Instance.spells.Clear();
            
            foreach (var spell in _spells)
            {
                SavedStats.Instance.spells.Add(spell);
            }

            SavedStats.Instance.spellIndex = _activeSpellIndex;
        }

        public void LoadData()
        {
            maxSpellPoints = SavedStats.Instance.maxSpellPoints;
            _spellPoints = SavedStats.Instance.currentSpellPoints;

            foreach (var spell in SavedStats.Instance.spells)
            {
                AddSpell(spell);
            }

            _activeSpellIndex = SavedStats.Instance.spellIndex;
            _activeSpell = _spells[_activeSpellIndex];
        }
    }
}
