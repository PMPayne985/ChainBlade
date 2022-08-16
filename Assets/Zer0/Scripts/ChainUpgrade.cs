using System;
using UnityEngine;

namespace Zer0
{
    public class ChainUpgrade : MonoBehaviour
    {
        public static ChainUpgrade Instance;
        
        private int _linksCollected;
        private int _chainLength;
        private int _chainKnifeUpgrades;

        public event Action<int> OnChainLengthUpgrade;
        public event Action<float> OnKnifeDamageUpgrade; 

        private void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
                Instance = this;
        }

        private void Start()
        {
            Collectible.Instance.OnCollectedLink += IncrementCollected;
        }

        private void IncrementCollected(int increment)
        {
            _linksCollected += increment;
        }

        public void UpgradeChainLength()
        {
            OnChainLengthUpgrade?.Invoke(1);
        }

        public void UpgradeKnifeDamage()
        {
            OnKnifeDamageUpgrade?.Invoke(1);
        }
    }
}
