using UnityEngine;

namespace Zer0
{
    public class EnemyImpact : MonoBehaviour
    {
        [SerializeField, Tooltip("Particle effect displayed when an object is struck")]
        private ParticleSystem smokeSystem;
        [SerializeField, Tooltip("Check if this weapon should deal damage to damagable objects.")]
        private bool canDamage;

        private void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Enemy")) return;
            
            print($"Impacted {col.name}");
            smokeSystem.Play();

            if (canDamage && col.TryGetComponent(out IDamagable target))
                target.TakeDamage(1);
        }
    }
}
