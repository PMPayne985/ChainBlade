using System;
using UnityEngine;

namespace Zer0
{
    public class Collectible : MonoBehaviour
    {
        public virtual void Collect(Collider other) 
            => gameObject.SetActive(false);

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                print(player);
                Collect(other);
            }
        }
    }
}
