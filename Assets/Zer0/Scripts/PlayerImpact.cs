using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Zer0
{
    public class PlayerImpact : MonoBehaviour
    {
        [SerializeField, Tooltip("Force applied to objects struck")]
        private float force = 1f;
        [SerializeField] private float damage = 1;
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

        private Player _player;
        private AudioSource _audio;

        private void Awake()
        {
            _player = transform.root.GetComponent<Player>();
            _audio = GetComponent<AudioSource>();
        }

        private void Start()
        {
            ChainUpgrade.Instance.OnKnifeDamageUpgrade += UpgradeDamage;
        }

        public void SetChainKnife(ChainKnife newKnife)
        {
            chainKnife = newKnife;
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
        }

        private void PlayImpactSound()
        {
            var random = Random.Range(0, impactSounds.Length);
            
            _audio.PlayOneShot(impactSounds[random]);
        }
        
        private void UpgradeDamage(float newDamage)
        {
            damage += newDamage;
        }
    }
}
