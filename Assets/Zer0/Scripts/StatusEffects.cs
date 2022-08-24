using System;
using UnityEngine;

namespace Zer0
{
    [RequireComponent(typeof(IDamagable))]
    public class StatusEffects : MonoBehaviour
    {
        private IDamagable _damagable;
        private StatusEffect _activeStatusEffect = StatusEffect.None;

        private float _duration;
        private float _frequency;
        private float _magnitude;

        private float _tick;

        private void Awake()
        {
            _damagable = GetComponent<IDamagable>();
        }

        private void Update()
        {
            ActivateEffect();
        }

        public void SetActiveEffect(StatusEffect newEffect, float duration,float frequency, float magnitude)
        {
            _duration = duration;
            _frequency = frequency;
            _tick = _frequency;
            _magnitude = magnitude;
            _activeStatusEffect = newEffect;
        }

        private void ActivateEffect()
        {
            if (_activeStatusEffect != StatusEffect.None)
            {
                _duration -= Time.deltaTime;
                _tick -= Time.deltaTime;
            }

            switch (_activeStatusEffect)
            {
                case StatusEffect.None:
                    break;
                case StatusEffect.Dot:
                    DamageOverTime();
                    break;
                case StatusEffect.Stun:
                    Stun();
                    break;
                case StatusEffect.Slow:
                    SLow();
                    break;
                case StatusEffect.Disarm:
                    Disarm();
                    break;
                case StatusEffect.Protect:
                    Protect();
                    break;
                case StatusEffect.Hot:
                    HealOverTime();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
        
        private void DamageOverTime()
        {
            if (_tick <= 0)
            {
                _damagable.TakeDamage(_magnitude);
                _tick = _frequency;
            }

            if (_duration <= 0)
                SetActiveEffect(StatusEffect.None, 0, 0, 0);
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
    }

    public enum StatusEffect
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
