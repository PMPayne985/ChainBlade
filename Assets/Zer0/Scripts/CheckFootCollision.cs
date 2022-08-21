using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Zer0
{
    public class CheckFootCollision : MonoBehaviour
    {
        public event Action<int> hitTheGround;

        [SerializeField]
        private PlayerGrounded grounded;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ground"))
            {
                grounded.PlayFootStep();
            }
        }
    }
}
