using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Zer0
{
    public class PlayerGrounded : MonoBehaviour
    {
        [SerializeField] private Transform leftFoot;
        [SerializeField] private Transform rightFoot;

        [SerializeField] private AudioClip[] footStepSounds;

        [SerializeField]
        private AudioSource _audio;

        private bool CheckGrounded(Transform checkLocation)
        {
            var ray = new Ray(checkLocation.position, Vector3.down);
            Debug.DrawRay(checkLocation.position, Vector3.down * .1f, Color.red);
            return Physics.Raycast(ray, out var hit, .1f);
        }

        public void PlayFootStep()
        {
            if (!_audio) return;

            var random = Random.Range(0, footStepSounds.Length);
            _audio.PlayOneShot(footStepSounds[random]);
        }

        public bool Grounded()
        {
            if (CheckGrounded(leftFoot)) return true;
            if (CheckGrounded(rightFoot)) return true;

            return false;
        }
    }
}
