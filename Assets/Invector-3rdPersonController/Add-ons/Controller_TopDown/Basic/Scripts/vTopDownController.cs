using UnityEngine;

namespace Invector.vCharacterController.TopDownShooter
{
    [vClassHeader("TOPDOWN CONTROLLER")]
    public class vTopDownController : vThirdPersonController
    {
        [vEditorToolbar("Layers")]
        public LayerMask mouseLayerMask = 1 << 0;

        internal Collider lookCollider;
    
        public virtual Vector3 lookPosition
        {
            get; protected set;
        }

        public override void ControlRotationType()
        {
            ///Rotate character to Mouse world position
            if (isStrafing)
            {
                lookPosition = vMousePositionHandler.Instance.WorldMousePosition(mouseLayerMask,out lookCollider);                
                Vector3 mouseDirection = (lookPosition - transform.position).normalized;
                Debug.DrawRay(transform.position + Vector3.up, mouseDirection);
                mouseDirection.y = 0;
                Vector3 desiredForward = Vector3.RotateTowards(transform.forward, mouseDirection, strafeSpeed.rotationSpeed * Time.fixedDeltaTime, 0f);             
                RotateToDirection(desiredForward);
               
                return;
            }
            ///Use default rotation system
            base.ControlRotationType();
            lookPosition = transform.position + transform.forward * 100f;
        }
    }
}