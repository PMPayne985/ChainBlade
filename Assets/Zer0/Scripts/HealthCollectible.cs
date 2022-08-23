using System;
using UnityEngine;

namespace Zer0
{
    public class HealthCollectible : Collectible
    {
        [SerializeField] private float healthToRestore = 5;
        public float HealthToRestore => healthToRestore;

        public static event Action<HealthCollectible> OnCollectedHealth;

        public override void Collect()
        {
            OnCollectedHealth?.Invoke(this);
            base.Collect();
        }
    }
}
