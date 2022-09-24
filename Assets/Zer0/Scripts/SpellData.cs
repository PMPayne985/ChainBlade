using System;
using EmeraldAI;
using UnityEngine;

namespace Zer0
{
    public class SpellData : MonoBehaviour
    {
        [SerializeField, Tooltip("The name of this Spell")]
        private string spellName;
        [SerializeField, Tooltip("The Icon to be displayed when this is the active spell.")]
        private Sprite icon;
        [SerializeField, Tooltip("The effect that will be displayed when the spell strikes a target")]
        private GameObject visualEffect;
        [SerializeField, Tooltip("The damage the spell does on impact. (set to 0 if no damage is desired)")]
        private int impactDamage;
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
        [SerializeField, Tooltip("Check this if the effect should stay in place and not stay with the target struck.")]
        private bool effectStationary;
        public GameObject trailEffect;
        [SerializeField] private bool PrebuiltSpell;
        
        public string Name { get; private set; }
        public Sprite Icon { get; private set; }
        public GameObject VisualEffect { get; private set; }
        public int ImpactDamage { get; private set; }
        public int Cost { get; private set; }
        public float CoolDown { get; private set; }
        public float Range { get; private set; }
        public areaOfEffect AOE { get; private set; }
        public float ExplosionSpeed { get; private set; }
        public float Duration { get; private set; }
        public float Frequency { get; private set; }
        public float Magnitude { get; private set; }
        public statusEffectType EffectToAdd { get; private set; }
        public bool EffectStationary { get; private set; }
        public GameObject TrailEffect { get; private set; }

        private void Awake()
        {
            if (!PrebuiltSpell) return;
            print("Spell Setup");
            
            Name = spellName;
            Icon = icon;
            VisualEffect = visualEffect;
            ImpactDamage = impactDamage;
            Cost = cost;
            CoolDown = coolDown;
            Range = range;
            AOE = aoe;
            ExplosionSpeed = explosionSpeed;
            Duration = duration;
            Frequency = frequency;
            Magnitude = magnitude;
            EffectToAdd = effectToAdd;
            EffectStationary = effectStationary;
            TrailEffect = trailEffect;
        }

        public void SetSpellVariables(string newName, Sprite newIcon, int newCost, float newCooldown, float newRange,
            areaOfEffect newAoe)
        {
            if (newName != string.Empty) Name = newName;
            if (newIcon) Icon = newIcon;
            Cost += newCost;
            CoolDown += newCooldown;
            Range += newRange;
            if (newAoe != areaOfEffect.None) AOE = newAoe;
        }

        public void SetSpellEffect(float newDuration, float newFrequency, float newMagnitude, int newImpactDamage, statusEffectType newEffect, bool stationary)
        {
            EffectStationary = stationary;
            Duration += newDuration;
            Frequency += newFrequency;
            Magnitude += newMagnitude;
            ImpactDamage += newImpactDamage;
            if (newEffect != statusEffectType.None) EffectToAdd = newEffect;
        }

        public void SetSpellParams(GameObject newVisualEffect, float newExplosionSpeed, GameObject newTrail)
        {
            VisualEffect = newVisualEffect;
            ExplosionSpeed = newExplosionSpeed;
            TrailEffect = newTrail;
        }
    }
}
