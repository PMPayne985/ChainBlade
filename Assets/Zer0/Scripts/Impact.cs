using UnityEngine;

namespace Zer0
{
    public class Impact : MonoBehaviour
    {
        [SerializeField, Tooltip("Force applied to objects struck")]
        private float force = 1f;
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

        public void SetChainKnife(ChainKnife newKnife)
        {
            chainKnife = newKnife;
        }
        
        private void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Player")) return;
            
            print($"Impacted {col.name}");
            chainKnife.EndExtension();
            smokeSystem.Play();

            if (canPush && col.TryGetComponent(out IPushable pushable))
                pushable.Push(transform, force);
            else if (canDrag && col.TryGetComponent(out IDraggable draggable))
                draggable.Drag(transform);
            
            if (canDamage && col.TryGetComponent(out IDamagable target))
                target.TakeDamage(1);
        }
    }
}
