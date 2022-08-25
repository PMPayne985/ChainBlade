using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zer0
{
    [RequireComponent(typeof(Character))]
    public class StatusEffects : MonoBehaviour
    {
        private IDamagable _damagable;
        private List<statusEffectInfo> _activeEffects;

        private void Awake()
        {
            _damagable = GetComponent<IDamagable>();
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
                        Stun();
                        if (_activeEffects.Count <= 0) return;
                        break;
                    case statusEffectType.Slow:
                        SLow();
                        if (_activeEffects.Count <= 0) return;
                        break;
                    case statusEffectType.Disarm:
                        Disarm();
                        if (_activeEffects.Count <= 0) return;
                        break;
                    case statusEffectType.Protect:
                        Protect();
                        if (_activeEffects.Count <= 0) return;
                        break;
                    case statusEffectType.Hot: ;
                        HealOverTime();
                        if (_activeEffects.Count <= 0) return;
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

            if (effect.tick <= 0)
            {
                print("Damage");
                _damagable.TakeDamage(effect.magnitude);
                    effect.tick = effect.frequency;
            }

            if (effect.duration <= 0)
                _activeEffects.Remove(effect);
        }

        private void Stun()
        {
            
        }

        private void SLow()
        {
            
        }
        
        private void Disarm()
        {
            
        }

        private void Protect()
        {
            
        }

        private void HealOverTime()
        {
            
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

    public enum statusEffectType
    {
        None,
        Dot,
        Stun,
        Slow,
        Disarm,
        Protect,
        Hot
    };
}
