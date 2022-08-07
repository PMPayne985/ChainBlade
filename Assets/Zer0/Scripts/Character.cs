using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zer0
{
    public class Character : MonoBehaviour, IDamagable
    {
        protected float _health;
        
        [SerializeField, Tooltip("Character's maximum health.")]
        protected float maxHealth;
        
        public void TakeDamage(float damageTaken)
        {
            if (damageTaken > 0)
                _health -= damageTaken;
            
            if (_health <= 0)
                Death();
                
        }

        public void RecoverHealth(float healingDone)
        {
            if (healingDone > 0)
                _health += healingDone;

            if (_health > maxHealth)
                _health = maxHealth;
        }

        public void Death()
        {
            if (gameObject.CompareTag("Player"))
            {
                var thisScene = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(thisScene);
            }
            else
                gameObject.SetActive(false);
        }
    }
}
