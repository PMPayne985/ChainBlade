using System;
using Invector.vCharacterController;
using UnityEngine;

namespace Zer0
{
    public class RotateAround : MonoBehaviour
    {
        [SerializeField]
        private Transform target;
        [SerializeField]
        private vMeleeCombatInput input;
        [SerializeField]
        private float rotateSpeed;

        private float _rotateAngle;
        

        private void Update()
        {
            _rotateAngle = rotateSpeed * Time.deltaTime * input.rotateCameraXInput.GetAxis();
        }

        private void FixedUpdate()
        {
            transform.RotateAround(target.position, Vector3.up,  _rotateAngle);
        }
    }
}
