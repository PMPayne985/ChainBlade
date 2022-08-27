using System.Collections;
using UnityEngine;

namespace Zer0
{
    public class Character : MonoBehaviour, IDamagable
    {
        protected float Health;
        protected bool dead;

        public Character Target { get; protected set; }
        
        [Tooltip("Targetable spaces for the Enemy AI")]
        public Transform[] targetSpaces;
        public bool[] TargetSpacesOccupied { get; private set; }
        
        [SerializeField, Tooltip("The delay before this character is removed from the scene.")]
        protected float deathDelay;
        [SerializeField, Tooltip("Character's maximum health.")]
        protected float maxHealth = 3;

        protected virtual void Start()
        {
            dead = false;
            Health = maxHealth;
            TargetSpacesOccupied = new bool[targetSpaces.Length];
        }

        public virtual void TakeDamage(float damageTaken)
        {
            if (dead) return;
            
            if (damageTaken > 0)
                Health -= damageTaken;
            
            if (Health <= 0)
                InitiateDeath();
        }

        public virtual void RecoverHealth(float healingDone)
        {
            if (dead) return;
            
            if (healingDone > 0)
                Health += healingDone;

            if (Health > maxHealth)
                Health = maxHealth;
        }

        public virtual void IncreaseMaxHealth(float healthToAdd)
        {
            if (healthToAdd <= 0) return;

            maxHealth += healthToAdd;
            Health += healthToAdd;
        }

        protected IEnumerator SendDeath()
        {
            yield return new WaitForSeconds(deathDelay);
            Death();
        }
        
        public virtual void InitiateDeath()
        {
            dead = true;
            StartCoroutine(SendDeath());
        }

        public virtual void Death()
        {
            
        }
        
        public virtual void Revive()
        {
            
        }
    }
}
