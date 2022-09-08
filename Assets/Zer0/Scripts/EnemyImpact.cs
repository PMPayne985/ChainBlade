using UnityEngine;

namespace Zer0
{
    public class EnemyImpact : MonoBehaviour
    {
        [SerializeField, Tooltip("Particle effect displayed when an object is struck")]
        private ParticleSystem smokeSystem;
        [SerializeField, Tooltip("Check if this weapon should deal damage to damagable objects.")]
        private bool canDamage;

        [SerializeField] private Enemy thisUnit;
        [SerializeField] private EnemyAI this_Unit;

        [SerializeField] private float damage = 1;

        private void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Enemy")) return;
            
            smokeSystem.Play();

            if (canDamage && col.TryGetComponent(out Player target))
            {
                target.TakeDamage(damage);
                if (thisUnit)
                    thisUnit.EndAttack();
                if (this_Unit)
                    this_Unit.EndAttack();
            }
        }
    }
}
