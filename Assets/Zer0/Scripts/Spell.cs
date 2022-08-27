using System;
using UnityEngine;

namespace Zer0
{
    public class Spell : MonoBehaviour
    {
        [SerializeField, Tooltip("The name of this Spell")]
        private string spellName;
        [SerializeField, Tooltip("The Icon to be displayed when this is the active spell.")]
        private Sprite icon;
        [SerializeField, Tooltip("The effect that will be displayed when the spell strikes a target")]
        private GameObject visualEffect;
        [SerializeField, Tooltip("The number of Spell Points used to cast this spell.")]
        private int cost;
        [SerializeField, Tooltip("The amount of time in seconds all spells will be unavailable after casting this spell.")]
        private float coolDown;
        [SerializeField, Tooltip("The maximum distance this spell will travel without hitting a target before activating its effect.")]
        private float range;
        [SerializeField, Tooltip("The maximum size in meters of this spells area of effect. \n" +
                                 "(This coupled with Explode Speed will determine how long an effect remains active on the battlefield \n" +
                                 "and thus able to affect enemies and possibly the player.)")] 
        private areaOfEffect aoe;
        
        [SerializeField, Tooltip("How quickly the area of effect will expand in meters per second after activating until reaching maximum size. \n" +
                                 "(This coupled with Aoe will determine how long an effect remains active on the battlefield \n" +
                                 "and thus able to affect enemies and possibly the player.)")] 
        private float explosionSpeed = 1;
        [SerializeField, Tooltip("How long any ongoing effects will apply to affected targets.")]
        private float duration;
        [SerializeField, Tooltip("How frequently ongoing effects will apply to affected targets.")]
        private float frequency;
        [SerializeField, Tooltip("How powerful each tick of an ongoing effect applied to affected targets will be.")]
        private float magnitude;
        [SerializeField, Tooltip("Status effect to be applied to an affected target.")]
        private statusEffectType effectToAdd;
        [SerializeField, Tooltip("The area that will be affected by this spell once it strikes a target or reaches is maximum range. \n" +
                                 "(This should be set as a trigger.)")]
        private SphereCollider areaOfEffectTriggerField;
        [SerializeField, Tooltip("The collider used to start the spells effect.")]
        private SphereCollider collisionDetection;
        
        
        private float _maxSize;
        private float _explosionTimer;
        private bool _explode;
        private Vector3 _startPosition;

        private void OnEnable()
        {
            SetRadius();
            _explode = false;
            _explosionTimer = .5f;
            _startPosition = transform.position;
            collisionDetection.enabled = true;
            areaOfEffectTriggerField.enabled = false;
        }

        private void Update()
        {
            Explode();
        }

        private void OnCollisionEnter(Collision collision)
        {
            _explode = true;
            Instantiate(visualEffect, collision.contacts[0].point, Quaternion.identity);
            collisionDetection.enabled = false;
            areaOfEffectTriggerField.enabled = true;
        }

        private void ExplodeAtMaxRange()
        {
            var distance = Vector3.Distance(_startPosition, transform.position);

            if (distance >= range)
            {
                _explode = true;
                Instantiate(visualEffect, transform.position, Quaternion.identity);
                collisionDetection.enabled = false;
                areaOfEffectTriggerField.enabled = true;
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out StatusEffects target)) return;
            
            ApplyStatusEffects(target);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out StatusEffects target))
                UpdateStatusEffects(target);
        }

        private void SetRadius()
        {
            _maxSize = aoe switch
            {
                areaOfEffect.None => .5f,
                areaOfEffect.Target => .25f,
                areaOfEffect.Small => 2,
                areaOfEffect.Medium => 4,
                areaOfEffect.Large => 6,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void ApplyStatusEffects(StatusEffects target)
        {
            target.AddActiveEffect(effectToAdd, duration, frequency, magnitude);
        }

        private void UpdateStatusEffects(StatusEffects target)
        {
            target.AddActiveEffect(effectToAdd, duration, 0, 0);
        }
        
        private void Explode()
        {
            if (!_explode) return;
            while (_explosionTimer < _maxSize)
                _explosionTimer += Time.deltaTime * explosionSpeed;

            areaOfEffectTriggerField.radius = _explosionTimer;
            if (_explosionTimer > _maxSize)
            {
                gameObject.SetActive(false);
            }
        }
        
        public void SetSpellVariables(string newName, Sprite newIcon, int newCost, float newCooldown, float newRange,
            areaOfEffect newAoe)
        {
            if (newName != string.Empty) spellName = newName;
            if (newIcon) icon = newIcon;
            cost += newCost;
            coolDown += newCooldown;
            range += newRange;
            if (newAoe != areaOfEffect.None) aoe = newAoe;
        }

        public void SetSpellEffect(float newDuration, float newFrequency, float newMagnitude, statusEffectType newEffect)
        {
            duration += newDuration;
            frequency += newFrequency;
            magnitude += newMagnitude;
            if (newEffect != statusEffectType.None) effectToAdd = newEffect;
        }
    }
}
