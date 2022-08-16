using System;
using UnityEngine;

namespace Zer0
{
    public class Collectible : MonoBehaviour
    {
        public static Collectible Instance { get; private set; }
        
        private int _numCollected;
        public event Action<int> OnCollectedLink;

        private void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
                Instance = this;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.CompareTag("collectible"))
                CollectLink(hit.gameObject);
        }

        private void CollectLink(GameObject collected)
        {
            _numCollected++;
            OnCollectedLink?.Invoke(1);
            Destroy(collected);

            print($"Links collected: {_numCollected}");
        }
    }
}
