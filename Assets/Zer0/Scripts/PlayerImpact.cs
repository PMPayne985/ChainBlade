using EmeraldAI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Zer0
{
    public class PlayerImpact : MonoBehaviour
    {
        [SerializeField, Tooltip("Force applied to objects struck")]
        private float force = 1f;
        [SerializeField] 
        private int damage = 1;
        [SerializeField, Tooltip("The maximum number of abilities this weapon can have.")]
        private int maxAbilities = 1;
        [SerializeField, Tooltip("The type of this weapon.")]
        private weaponType type;
        [SerializeField, Tooltip("Particle effect displayed when an object is struck")]
        private ParticleSystem smokeSystem;
        [SerializeField, Tooltip("Check if this weapon should push pushable objects.")]
        private bool canPush;
        [SerializeField, Tooltip("Check if this weapon should drag draggable objects.")]
        private bool canDrag;
        [SerializeField, Tooltip("Check if this weapon should deal damage to damagable objects.")]
        private bool canDamage;
        [SerializeField] 
        private bool lifeLeech;
        [SerializeField, Tooltip("The Chain Knife script that will be used with this blade.")]
        private ChainKnife chainKnife;
        [SerializeField, Tooltip("A list of sounds that can play on impact")]
        private AudioClip[] impactSounds;

        private int _enhancmentStep;
        private int _lifeStealAmount;

        private Player _player;
        private AudioSource _audio;
        
        private statusEffectType[] _effects;
        private float[] _durations;
        private float[] _frequencies;
        private float[] _magnitudes;

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();

            _effects = new statusEffectType[maxAbilities];
            _durations = new float[maxAbilities];
            _frequencies = new float[maxAbilities];
            _magnitudes = new float[maxAbilities];
        }

        private void Start()
        {
            UpgradeBladeMenu.OnDamageUpgrade += UpgradeDamage;
            UpgradeBladeMenu.OnAddStatusEffect += SetStatusEffects;
            UpgradeBladeMenu.OnAddStatusEffect += SetEffectParameters;
            UpgradeBladeMenu.OnChainPullUpgrade += ChangeDrag;
            UpgradeBladeMenu.OnUpgradeLifeLeech += SetLifeLeech;

        }

        public void SetChainKnifeDependencies(ChainKnife newKnife)
        {
            chainKnife = newKnife;
            
            _player = chainKnife.transform.root.GetComponent<Player>();
        }

        public void ChangeDrag(weaponType checkType, bool status)
        {
            if (type == checkType)
                canDrag = status;
        }

        public void StealHealth()
        {
            if (lifeLeech)
                _player.RestoreHealth(_lifeStealAmount);
        }

        public void SetLifeLeech(weaponType typeWeapon, bool canLeech, int leechValue)
        {
            if (type == typeWeapon)
            {
                lifeLeech = canLeech;
                _lifeStealAmount = leechValue;
            }
        }
        
        public void ChangePush(bool status) => canPush = status;

        
        public void ChangeDamage(bool status, int startingDamage)
        {
            canDamage = status;
            damage = startingDamage;
        }
        
        public void SetStatusEffects(statusEffectType newEffectType, weaponType weapon, float parm1, float parm2, float parm3)
        {
            if (type != weapon) return;
            
            var isKnown = false;
            for (var i = 0; i < _effects.Length; i++)
            {
                if (_effects[i] == newEffectType)
                {
                    isKnown = true;
                    break;
                }
            }
            
            if (isKnown) return;

            for (var i = 0; i < _effects.Length; i++)
            {
                if (_effects[i] == statusEffectType.None)
                    _effects[i] = newEffectType;
            }
        }

        public void SetEffectParameters(statusEffectType effectTypeToChange, weaponType weapon, float duration, float frequency, float magnitude)
        {
            if (type != weapon) return;
            
            for (int i = 0; i < _effects.Length; i++)
            {
                if (_effects[i] == effectTypeToChange)
                {
                    _durations[i] += duration;
                    _frequencies[i] += frequency;
                    _magnitudes[i]  += magnitude;
                }
            }
        }
        
        private void OnTriggerEnter(Collider col)
        {
            PlayImpactSound();
            chainKnife.EndExtension();
            smokeSystem.Play();

            if (canPush && col.TryGetComponent(out IPushable pushable))
            { 
                pushable.Push(transform, force);
            }
            else if (canDrag && col.TryGetComponent(out IDraggable draggable))
            { 
                draggable.Drag(transform);
            }

            if (col.TryGetComponent(out EmeraldAISystem ai))
            {
                if (canDamage)
                {
                    ai.Damage(damage, EmeraldAISystem.TargetType.Player, transform, 400);
                }
            }
        }

        private void PlayImpactSound()
        {
            if (impactSounds.Length < 1) return;
            
            var random = Random.Range(0, impactSounds.Length);
            
            _audio.PlayOneShot(impactSounds[random]);
        }
        
        private void UpgradeDamage(weaponType weapon, int newDamage)
        {
            if (weapon == type && canDamage)
            {
                damage += newDamage;
            }
        }

        private void ApplyStatusEffectOnImpact(StatusEffects affected)
        {
            for (var i = 0; i < _effects.Length; i++)
            {
                affected.AddActiveEffect(_effects[i], _durations[i], _frequencies[i], _magnitudes[i]);
            }
        }
    }
}
