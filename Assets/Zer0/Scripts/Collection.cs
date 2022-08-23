using System;
using UnityEngine;

namespace Zer0
{
    public class Collection : MonoBehaviour
    {
        public static Collection Instance { get; private set; }

        private int _numCollected;

        public event Action<LinkCollectible> OnCollectedLink;
        public event Action<HealthCollectible> OnCollectedHealth;
        
        private void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
                Instance = this;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.TryGetComponent(out Collectible collected))
                CollectLink(collected);
        }

        private void CollectLink(Collectible collected)
        {
            if (TryGetComponent(out LinkCollectible link))
            {
                _numCollected++;
                OnCollectedLink?.Invoke(link);
            }
            else if (TryGetComponent(out HealthCollectible healthUp))
            {
                OnCollectedHealth?.Invoke(healthUp);
            }
            
            collected.Collect();
            collected.gameObject.SetActive(false);
        }
    }
}
