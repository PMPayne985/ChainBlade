using System;
using UnityEngine;

namespace Zer0
{
    public class Collection : MonoBehaviour
    {
        public static Collection Instance { get; private set; }
        
        private int _numCollected;
        public event Action<GameObject> OnCollectedLink;

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
            OnCollectedLink?.Invoke(this.gameObject);
            collected.SetActive(false);

            print($"Links collected: {_numCollected}");
        }
    }
}
