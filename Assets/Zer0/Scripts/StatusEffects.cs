using System;
using System.Collections.Generic;
using EmeraldAI;
using Invector.vCharacterController;
using Invector.vMelee;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Zer0
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(Animator))]
    public class StatusEffects : MonoBehaviour
    {
        [SerializeField] private GameObject dotEffect;
        [SerializeField] private GameObject hotEffect;
        [SerializeField] private GameObject disarmEffect;
        [SerializeField] private GameObject protectEffect;
        [SerializeField] private GameObject slowEffect;
        [SerializeField] private GameObject stunEffect;
        
        private Character _character;
        private Animator _animator;
        private EmeraldAISystem _enemy;
        private List<statusEffectInfo> _activeEffects;

        private bool _incapacitated;
        private bool _slowed;
        private bool _dotted;
        private bool _hotted;
        private bool _disarmed;
        private bool _protected;
        private bool _dead;

        [SerializeField, Tooltip("The rate at which this unit resists negative status effects.")]
        private int resistance;

        public static event Action<bool, Image, float> OnAddStatusEffect;

        private void Awake()
        {
            _character = GetComponent<Character>();
            _animator = GetComponent<Animator>();
            _activeEffects = new List<statusEffectInfo>();

        }

        private void Start()
        {
            UpgradeArmorMenu.OnIncreaseResistance += SetResistance;
        }

        private void OnEnable()
        {
            _dead = false;
        }

        private void Update()
        {
            ActivateEffects();
        }

        public void AddActiveEffect(statusEffectType newEffectType, float duration,float frequency, float magnitude)
        {
            if (newEffectType != statusEffectType.Dot && newEffectType != statusEffectType.Protect)
            {
                var chance = Random.Range(0, 100);
                if (chance < resistance) return;
            }
            
            if (_activeEffects.Count > 0)
            {
                foreach (var effect in _activeEffects)
                {
                    if (effect.EffectType != newEffectType) continue;
                    
                    effect.duration = duration;
                    return;
                }
            }
            
            var thisEffect = new statusEffectInfo(newEffectType, duration, frequency, magnitude, frequency);
            _activeEffects.Add(thisEffect);
        }

        public void RemoveActiveEffect(statusEffectType effectType)
        {
            if (_activeEffects.Count <= 0) return;

            foreach (var effect in _activeEffects)
            {
                if (effect.EffectType == effectType)
                {
                    _activeEffects.Remove(effect);
                    return;
                }
            }
        }

        public void ClearAllEffects()
        {
            _activeEffects.Clear();
        }
        
        private void ActivateEffects()
        {
            if (_dead)
            {
                ClearAllEffects();
                return;
            }
            if (_activeEffects.Count <= 0) return;

            for (var i = 0; i < _activeEffects.Count; i++)
            {
                if (i > _activeEffects.Count) return;

                switch (_activeEffects[i].EffectType)
                {
                    case statusEffectType.None:
                        break;
                    case statusEffectType.Dot:
                        DamageOverTime(_activeEffects[i]);
                        break;
                    case statusEffectType.Stun:
                        Stun(_activeEffects[i]);
                        break;
                    case statusEffectType.Slow:
                        SLow(_activeEffects[i]);
                        break;
                    case statusEffectType.Disarm:
                        Disarm(_activeEffects[i]);
                        break;
                    case statusEffectType.Protect:
                        Protect(_activeEffects[i]);
                        break;
                    case statusEffectType.Hot:
                        HealOverTime(_activeEffects[i]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        private void DamageOverTime(statusEffectInfo effect)
        {
            effect.duration -= Time.deltaTime;
            effect.tick -= Time.deltaTime;

            if (!_dotted)
            {
                _dotted = true;
                if (dotEffect)
                {
                    dotEffect.SetActive(true);
                    OnAddStatusEffect?.Invoke(_character.isPlayer, dotEffect.GetComponent<Image>(), effect.duration);
                }
            }

            if (effect.tick <= 0)
            {
                _character.TakeDamage((int)effect.magnitude, transform);
                
                effect.tick = effect.frequency;
            }

            if (effect.duration <= 0 && _dotted)
            {
                _dotted = false;
                if (dotEffect) dotEffect.SetActive(false);
                _activeEffects.Remove(effect);
            }
        }

        private void Stun(statusEffectInfo effect)
        {
            var curSpeed = _animator.speed;
            effect.duration -= Time.deltaTime;
            
            if (!_incapacitated)
            {
                _incapacitated = true;
                if (stunEffect)
                {
                    OnAddStatusEffect?.Invoke(_character.isPlayer, stunEffect.GetComponent<Image>(), effect.duration);
                    stunEffect.SetActive(true);
                }
                _animator.speed -= curSpeed;
            }
            

            if (effect.duration <= 0 && _incapacitated)
            {
                _incapacitated = false;
                if (stunEffect) stunEffect.SetActive(false);
                _animator.speed += curSpeed;
                _activeEffects.Remove(effect);
            }
        }

        private void SLow(statusEffectInfo effect)
        {
            effect.duration -= Time.deltaTime;
            
            if (!_slowed && !_incapacitated)
            {
                _slowed = true;
                var newSpeed = effect.magnitude - 1;
                if (newSpeed > .7f)
                    newSpeed = .7f;
                _animator.speed -= newSpeed;
                if (_character.isPlayer)
                {
                    GetComponent<vThirdPersonController>().speedMultiplier -= effect.magnitude;
                }
                if (slowEffect)
                {
                    OnAddStatusEffect?.Invoke(_character.isPlayer, slowEffect.GetComponent<Image>(), effect.duration);
                    slowEffect.SetActive(true);
                }
            }
            

            if (effect.duration <= 0 && _slowed)
            {
                _slowed = false;
                var newSpeed = effect.magnitude - 1;
                if (newSpeed > .7f)
                    newSpeed = .7f;
                _animator.speed += newSpeed;
                if (_character.isPlayer)
                {
                    GetComponent<vThirdPersonController>().speedMultiplier += effect.magnitude;
                }
                if (slowEffect) slowEffect.SetActive(false);
                _activeEffects.Remove(effect);
            }
        }
        
        private void Disarm(statusEffectInfo effect)
        {
            effect.duration -= Time.deltaTime;
            
            if (!_disarmed)

            {
                _disarmed = true;
                _character.Disarm((int)effect.duration);
                if (disarmEffect) disarmEffect.SetActive(true);
            }

            if (effect.duration <= 0 && _disarmed)
            {
                _disarmed = false;
                _character.RemoveDisarm();
                if (slowEffect)
                {
                    OnAddStatusEffect?.Invoke(_character.isPlayer, slowEffect.GetComponent<Image>(), effect.duration);
                    slowEffect.SetActive(false);
                }
                _activeEffects.Remove(effect);
            }
        }

        private void Protect(statusEffectInfo effect)
        {
            effect.duration -= Time.deltaTime;

            if (!_protected)
            {
                _protected = true;
                if (_character.isPlayer)
                {
                    if (protectEffect)
                    {
                        OnAddStatusEffect?.Invoke(_character.isPlayer, protectEffect.GetComponent<Image>(), effect.duration);
                        protectEffect.SetActive(true);
                    }
                    GetComponent<Player>().Protecting((int)effect.magnitude);
                }
            }

            if (effect.duration <= 0 && _protected)
            {
                _protected = false;
                if (_character.isPlayer)
                {
                    GetComponent<Player>().EndProtecting();
                    if (protectEffect) protectEffect.SetActive(false);
                }
            }
        }

        private void HealOverTime(statusEffectInfo effect)
        {
            effect.duration -= Time.deltaTime;
            effect.tick -= Time.deltaTime;

            if (!_hotted)
            {
                _hotted = true;
                if (hotEffect) hotEffect.SetActive(true);
            }
            
            if (effect.tick <= 0)
            {
                _character.RestoreHealth((int)effect.magnitude);
                effect.tick = effect.frequency;
            }

            if (effect.duration <= 0 && _hotted)
            {
                _hotted = false;
                if (hotEffect)
                {
                    OnAddStatusEffect?.Invoke(_character.isPlayer, hotEffect.GetComponent<Image>(), effect.duration);
                    hotEffect.SetActive(false);
                }
                _activeEffects.Remove(effect);
            }
        }

        public void SetResistance(int rate)
        {
            resistance += rate;
        }
        
        public void SetDeathStatus(bool deathStatus)
        {
            _dead = deathStatus;
        }

        private class statusEffectInfo
        {
            public statusEffectType EffectType;
            public float duration;
            public float frequency;
            public float magnitude;
            public float tick;

            public statusEffectInfo(statusEffectType thisEffectType, float thisDuration, float thisFrequency, float thisMagnitude, float thisTick)
            {
                EffectType = thisEffectType;
                duration = thisDuration;
                frequency = thisFrequency;
                magnitude = thisMagnitude;
                tick = thisTick;
            }
        }
    }
}
