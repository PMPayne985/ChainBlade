using System;
using UnityEngine;

namespace Zer0
{
    public class Targeting : MonoBehaviour
    {
        public Character Target { get; protected set; }
        protected Transform TargetTransform;

        protected virtual void Awake()
        {
            Target = FindObjectOfType<Player>();
            if (!Target)
                Debug.LogWarning("No Player found.");

            TargetTransform = Target.GetComponent<Transform>();
        }

        public float TargetDistance()
        {
            return Vector3.Distance(transform.position, TargetTransform.position);
        }
    }
}
