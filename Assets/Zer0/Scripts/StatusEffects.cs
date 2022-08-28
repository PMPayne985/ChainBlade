using System;
using System.Collections.Generic;
using UnityEngine;

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
        private List<statusEffectInfo> _activeEffects;

        private bool _incapacitated;
        private bool _slowed;
        private bool _dotted;
        private bool _hotted;
        private bool _disarmed;
        private bool _protected;

        private void Awake()
        {
            _character = GetComponent<Character>();
            _animator = GetComponent<Animator>();
            _activeEffects = new List<statusEffectInfo>();
        }

        private void Update()
        {
            ActivateEffects();
        }

        public void AddActiveEffect(statusEffectType newEffectType, float duration,float frequency, float magnitude)
        {
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
            if (_activeEffects.Count <= 0) return;
            
            foreach (var effect in _activeEffects)
            {
                switch (effect.EffectType)
                {
                    case statusEffectType.None:
                        break;
                    case statusEffectType.Dot:
                        DamageOverTime(effect);
                        if (_activeEffects.Count <= 0) return;
                        break;
                    case statusEffectType.Stun:
                        Stun(effect);
                        if (_activeEffects.Count <= 0) return;
                        break;
                    case statusEffectType.Slow:
                        SLow(effect);
                        if (_activeEffects.Count <= 0) return;
                        break;
                    case statusEffectType.Disarm:
                        Disarm(effect);
                        if (_activeEffects.Count <= 0) return;
                        break;
                    case statusEffectType.Protect:
                        Protect(effect);
                        if (_activeEffects.Count <= 0) return;
                        break;
                    case statusEffectType.Hot: ;
                        HealOverTime(effect);
                        if (_activeEffects.Count <= 0) return;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
            }
            }
            
        }
        
        private void DamageOverTime(statusEffectInfo effect)
        {
            if (_activeEffects.Count <= 0) return;
            
            effect.duration -= Time.deltaTime;
            effect.tick -= Time.deltaTime;

            if (!_dotted)
            {
                _dotted = true;
                if (dotEffect) dotEffect.SetActive(true);
            }
            
            if (_activeEffects.Count <= 0) return;
            
            if (effect.tick <= 0)
            {
                _character.TakeDamage(effect.magnitude);
                    effect.tick = effect.frequency;
            }

            if (_activeEffects.Count <= 0) return;
            
            if (effect.duration <= 0)
            {
                _dotted = false;
                if (dotEffect) dotEffect.SetActive(false);
                _activeEffects.Remove(effect);
            }
        }

        private void Stun(statusEffectInfo effect)
        {
            effect.duration -= Time.deltaTime;
            
            if (!_incapacitated)
            {
                _incapacitated = true;
                if (stunEffect) stunEffect.SetActive(true);
                _animator.speed = 0;
            }
            

            if (effect.duration <= 0)
            {
                _incapacitated = false;
                if (stunEffect) stunEffect.SetActive(false);
                _animator.speed = 1;
                _activeEffects.Remove(effect);
            }
        }

        private void SLow(statusEffectInfo effect)
        {
            effect.duration -= Time.deltaTime;
            
            if (!_slowed)
            {
                _slowed = true;
                _animator.speed -= effect.magnitude;
                if (slowEffect) slowEffect.SetActive(true);
            }
            

            if (effect.duration <= 0)
            {
                _slowed = false;
                _animator.speed += effect.magnitude;
                if (slowEffect) slowEffect.SetActive(false);
                _activeEffects.Remove(effect);
            }
        }
        
        private void Disarm(statusEffectInfo effect)
        {
            
        }

        private void Protect(statusEffectInfo effect)
        {
            
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
                _character.RecoverHealth(effect.magnitude);
                effect.tick = effect.frequency;
            }

            if (effect.duration <= 0)
            {
                _hotted = false;
                if (hotEffect) hotEffect.SetActive(false);
                _activeEffects.Remove(effect);
            }
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
