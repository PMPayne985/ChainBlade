using UnityEngine;
using System.Collections;

namespace Invector.vShooter
{
    using vCharacterController.TopDownShooter;
    public class vShooterTopDownCursor : MonoBehaviour
    {
        private vTopDownShooterInput shooter;
        public GameObject sprite;
        public LineRenderer lineRender;

        void Start()
        {
            shooter = FindObjectOfType<vTopDownShooterInput>();
        }

        void FixedUpdate()
        {
            if (shooter)
            {
                if (sprite)
                {
                    if (shooter.isAimingByInput && !sprite.gameObject.activeSelf)
                        sprite.gameObject.SetActive(true);
                    else if (!shooter.isAimingByInput && sprite.gameObject.activeSelf)
                        sprite.gameObject.SetActive(false);
                }

                transform.position = shooter.AimPosition;
                var dir = shooter.transform.position - shooter.AimPosition;
                dir.y = 0;

                if (dir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(dir);
                    if (lineRender)
                    {
                        lineRender.SetPosition(0, shooter.topDownController.lookPosition);
                        lineRender.SetPosition(1, shooter.AimPosition);
                        if (shooter.isAimingByInput && !lineRender.enabled)
                            lineRender.enabled = true;
                        else if (!shooter.isAimingByInput && lineRender.enabled)
                            lineRender.enabled = false;
                    }
                }
            }
        }
    }
}

