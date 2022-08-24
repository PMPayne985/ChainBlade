using UnityEngine;
using Random = UnityEngine.Random;

namespace Zer0
{
    public class PlayerImpact : MonoBehaviour
    {
        [SerializeField, Tooltip("Force applied to objects struck")]
        private float force = 1f;
        [SerializeField] private float damage = 1;
        [SerializeField, Tooltip("The number of enhancments needed before this weapon will receive a damage increase.")] 
        private int increaseDamageInterval = 1;
        [SerializeField] 
        private weaponType type;
        [SerializeField, Tooltip("Particle effect displayed when an object is struck")]
        private ParticleSystem smokeSystem;
        [SerializeField, Tooltip("Check if this weapon should push pushable objects.")]
        private bool canPush;
        [SerializeField, Tooltip("Check if this weapon should drag draggable objects.")]
        private bool canDrag;
        [SerializeField, Tooltip("Check if this weapon should deal damage to damagable objects.")]
        private bool canDamage;
        [SerializeField, Tooltip("The Chain Knife script that will be used with this blade.")]
        private ChainKnife chainKnife;
        [SerializeField, Tooltip("A list of sounds that can play on impact")]
        private AudioClip[] impactSounds;

        private int _enhancmentStep;

        private Player _player;
        private AudioSource _audio;

        [SerializeField, Tooltip("The status Effects this impact can apply to a target.")]
        private StatusEffect[] effects = new StatusEffect[1];
        [SerializeField, Tooltip("the duration of each effect from the effects list.")]
        private float[] durations = new float[1];
        [SerializeField, Tooltip("The frequency each effect from the effects list will occur.")]
        private float[] frequencies = new float[1];
        [SerializeField, Tooltip("The magnitude of each effect from the effects list.")]
        private float[] magnitudes = new float[1];

        private void Awake()
        {
            _player = transform.root.GetComponent<Player>();
            _audio = GetComponent<AudioSource>();
        }

        private void Start()
        {
            ChainUpgrade.OnKnifeDamageUpgrade += UpgradeDamage;
            ChainUpgrade.OnAddStatusEffect += SetStatusEffects;
            ChainUpgrade.OnAddStatusEffect += SetEffectParameters;

        }

        public void SetChainKnife(ChainKnife newKnife)
        {
            chainKnife = newKnife;
        }

        public void AddDrag() => canDrag = true;
        public void AddPush() => canPush = true;

        
        public void AddDamage(float startingDamage)
        {
            canDamage = true;
            damage = startingDamage;
        }
        
        public void SetStatusEffects(StatusEffect newEffect, weaponType weapon, float parm1, float parm2, float parm3)
        {
            if (type != weapon) return;
            
            var isKnown = false;
            for (var i = 0; i < effects.Length; i++)
            {
                if (effects[i] == newEffect)
                {
                    isKnown = true;
                    break;
                }
            }
            
            if (isKnown) return;

            for (var i = 0; i < effects.Length; i++)
            {
                if (effects[i] == StatusEffect.None)
                    effects[i] = newEffect;
            }
        }

        public void SetEffectParameters(StatusEffect effectToChange, weaponType weapon, float duration, float frequency, float magnitude)
        {
            if (type != weapon) return;
            
            for (int i = 0; i < effects.Length; i++)
            {
                if (effects[i] == effectToChange)
                {
                    durations[i] += duration;
                    frequencies[i] += frequency;
                    magnitudes[i]  += magnitude;
                }
            }
        }
        
        private void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Player")) return;
         
            PlayImpactSound();
            chainKnife.EndExtension();
            smokeSystem.Play();

            if (canPush && col.TryGetComponent(out IPushable pushable))
                pushable.Push(transform, force);
            else if (canDrag && col.TryGetComponent(out IDraggable draggable))
                draggable.Drag(transform);
            
            if (canDamage && col.TryGetComponent(out IDamagable target))
            {
                _player.EndAttack();
                target.TakeDamage(damage);
            }
            
            if (col.TryGetComponent(out StatusEffects affected))
                ApplyStatusEffectOnImpact(affected);
        }

        private void PlayImpactSound()
        {
            var random = Random.Range(0, impactSounds.Length);
            
            _audio.PlayOneShot(impactSounds[random]);
        }
        
        private void UpgradeDamage(float newDamage)
        {
            _enhancmentStep++;
            if (_enhancmentStep >= increaseDamageInterval)
            {
                damage += newDamage;

                if (_enhancmentStep > increaseDamageInterval)
                    _enhancmentStep = 0;
            }
        }

        private void ApplyStatusEffectOnImpact(StatusEffects affected)
        {
            for (var i = 0; i < effects.Length; i++)
            {
                affected.SetActiveEffect(effects[i], durations[i], frequencies[i], magnitudes[i]);
            }
        }
    }

    public enum weaponType
    {
        chainEnd,
        knifeBlade
    };
}
