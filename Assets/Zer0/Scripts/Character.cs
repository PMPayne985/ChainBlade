using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zer0
{
    public class Character : MonoBehaviour, IDamagable
    {
        protected float Health;
        protected bool dead;

        [SerializeField, Tooltip("The delay before this character is removed from the scene.")]
        protected float deathDelay;
        [SerializeField, Tooltip("Character's maximum health.")]
        protected float maxHealth = 3;

        protected virtual void Start()
        {
            dead = false;
            Health = maxHealth;
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
        
        private void LogMessage(Color color, string message)
        {
            Debug.Log ($"<color=#{(byte)(color.r * 255f):X2}{(byte)(color.g * 255f):X2}{(byte)(color.b * 255f):X2}>{message}</color>");
        }
    }
}
