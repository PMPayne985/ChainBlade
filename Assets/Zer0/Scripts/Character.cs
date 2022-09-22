using UnityEngine;

namespace Zer0
{
    public class Character : MonoBehaviour
    {
        public virtual void TakeDamage(int damage, Transform attacker) { }

        public virtual void RestoreHealth(int value) { }
        
        public virtual void Disarm(int value) { }
        
        public virtual void RemoveDisarm() {}

        public bool isPlayer;
    }
}
