using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zer0
{
    public class Character : MonoBehaviour, IDamagable
    {
        protected float _health;
        
        [SerializeField, Tooltip("Character's maximum health.")]
        protected float maxHealth = 3;

        protected void Start()
        {
            _health = maxHealth;
            
            LogMessage(Color.blue, $"{gameObject.name} starting health: {_health}");
        }

        public void TakeDamage(float damageTaken)
        {
            if (damageTaken > 0)
                _health -= damageTaken;
            
            if (_health <= 0)
                Death();
            
            LogMessage(Color.red, $"{gameObject.name} current health: {_health}");
        }

        public void RecoverHealth(float healingDone)
        {
            if (healingDone > 0)
                _health += healingDone;

            if (_health > maxHealth)
                _health = maxHealth;
            
            LogMessage(Color.green, $"{gameObject.name} current health: {_health}");
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

        private void LogMessage(Color color, string message)
        {
            Debug.Log ($"<color=#{(byte)(color.r * 255f):X2}{(byte)(color.g * 255f):X2}{(byte)(color.b * 255f):X2}>{message}</color>");
        }
    }
}
