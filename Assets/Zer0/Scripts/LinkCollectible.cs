using System;
using UnityEngine;

namespace Zer0
{
    public class LinkCollectible : Collectible
    {
        public static int NumCollected { get; private set; }
        
        public static event Action<LinkCollectible> OnCollectedLink;

        private void IncrementCollected(int increment) 
            => NumCollected += increment;

        public override void Collect(Collider other)
        {
            IncrementCollected(1);
            OnCollectedLink?.Invoke(this);
            base.Collect(other);
        }
    }
}
