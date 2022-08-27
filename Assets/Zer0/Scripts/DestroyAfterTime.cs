using UnityEngine;

namespace Zer0
{
    public class DestroyAfterTime : MonoBehaviour
    {
        [SerializeField]
        private float decayTime;

        private void Update()
        {
            decayTime -= Time.deltaTime;

            if (decayTime <= 0) DestroyImmediate(gameObject);
        }

        public void SetDecay(float duration)
        {
            decayTime = duration;
        }
    }
}
