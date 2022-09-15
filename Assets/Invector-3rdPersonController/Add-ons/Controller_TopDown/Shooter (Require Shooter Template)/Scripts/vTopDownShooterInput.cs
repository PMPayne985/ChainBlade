using UnityEngine;
using System.Collections;

namespace Invector.vCharacterController.TopDownShooter
{
    using vShooter;
    [vClassHeader("TopDown Shooter Input")]
    public class vTopDownShooterInput : vShooterMeleeInput
    {
        [vEditorToolbar("TopDown")]
        public bool alwaysAimForward;
        public float aimMinDistance = 2f;
        [Tooltip("Press and hold the Mouse Middle Button and rotate it to rotate the Camera")]
        public bool rotateCamera = true;

        private vTopDownController _topDown;

        public virtual vTopDownController topDownController
        {
            get
            {
                if (cc && cc is vTopDownController && _topDown == null) _topDown = cc as vTopDownController;
                return _topDown;
            }
        }

        protected override void Update()
        {
            CameraRotation(cc.rotateTarget);
            base.Update();
        }

        protected override void UpdateAimPosition()
        {
            if (!topDownController)
            {
                base.UpdateAimPosition();
                return;
            }
            var aimPoint = topDownController.lookPosition;
            if (Vector3.Distance(cc._capsuleCollider.bounds.center, aimPoint) < cc.colliderRadius + aimMinDistance)
            {
                aimPoint = transform.position + transform.forward * (cc.colliderRadius + aimMinDistance);
                if (!alwaysAimForward)
                {
                    aimPoint.y = transform.position.y;
                    aimPoint += Vector3.up * Vector3.Distance(transform.position, rightUpperArm.position);
                }
            }
            if (alwaysAimForward)
            {
                aimPoint.y = transform.position.y;
                aimPoint += Vector3.up * Vector3.Distance(transform.position, rightUpperArm.position);
            }
            AimPosition = aimPoint;
        }

        protected override void CheckAimConditions()
        {
            if (!shooterManager || CurrentActiveWeapon == null) return;

            var _ray = new Ray(rightUpperArm.position, AimPosition - (rightUpperArm.position));
            RaycastHit hit;
            if (Physics.SphereCast(_ray, shooterManager.checkAimRadius, out hit, shooterManager.checkAimOffsetZ, shooterManager.blockAimLayer))
            {
                aimConditions = false;
            }
            else
                aimConditions = true;
            aimWeight = Mathf.Lerp(aimWeight, aimConditions ? 1 : 0, 1 * Time.deltaTime);
        }

        protected override void UpdateHeadTrackLookPoint()
        {
            if (IsAiming && !isUsingScopeView)
            {
                headTrack.SetTemporaryLookPoint(AimPosition, 0.1f);
            }
        }

        protected override Vector3 targetArmAligmentDirection
        {
            get
            {
                return transform.forward;
            }
        }

        protected override Vector3 targetArmAlignmentPosition
        {
            get
            {
                return AimPosition;
            }
        }

        protected void CameraRotation(Transform cameraTransform)
        {
            if (!rotateCamera) return;
            // press middle mouse button and rotate the mouse to rotate the camera
            if (Input.GetMouseButton(2) && rotateCameraXInput.GetAxis() > 0.1f)
            {
                tpCamera.lerpState.fixedAngle.x += 2f;
            }
            else if (Input.GetMouseButton(2) && rotateCameraXInput.GetAxis() < -0.1f)
            {
                tpCamera.lerpState.fixedAngle.x += -2f;
            }
        }
    }
}