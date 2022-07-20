using UnityEngine;

namespace Zer0
{
    public class PushableObject : MonoBehaviour, IPushable
    {
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Push(Transform pusher, float force)
        {
            _rigidbody.AddForce(pusher.forward * force, ForceMode.Impulse);
        }
    }
}
