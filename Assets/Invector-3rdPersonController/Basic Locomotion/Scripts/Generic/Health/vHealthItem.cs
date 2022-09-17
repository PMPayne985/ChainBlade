using UnityEngine;

namespace Invector
{
    public class vHealthItem : Zer0.Collectible
    {
        [Tooltip("How much health will be recovered")]
        public float value;

        public override void Collect(Collider other)
        {
            // access the basic character information
            if (!other.TryGetComponent(out vHealthController healthController)) return;
            // heal only if the character's health isn't full
            if (!(healthController.currentHealth < healthController.maxHealth)) return;
            // limit healing to the max health
            healthController.AddHealth((int)value);
            base.Collect(other);
        }
    }
}