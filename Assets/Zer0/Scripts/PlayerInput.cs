using System;
using UnityEngine;

namespace Zer0
{
    public class PlayerInput : MonoBehaviour
    {
        public static bool Attack() => Input.GetButtonDown("Fire1") || Input.GetAxis("FireAxis") > .5f;

        public static bool ChainAttack() => Input.GetButtonDown("Fire2") || Input.GetAxis("FireAxis") < -.5f;

        public static bool Sprint() => Input.GetButton("Sprint");

        public static bool PauseMenu() => Input.GetButtonUp("Cancel");

        public static bool UpgradeMenu() => Input.GetButtonUp("Upgrade");
        public static bool NextSpell() => Input.GetButtonUp("NextSpell");
        public static bool CastSpell() => Input.GetButtonUp("CastSpell");
        public static bool NextTarget() => Input.GetButtonUp("NextTarget");
        
        public static float Horizontal() => Input.GetAxis("Horizontal");

        public static float Vertical() => Input.GetAxis("Vertical");

        public static float Rotation() => Input.GetAxis("Mouse X");

        public static bool Debug() => Input.GetButtonUp("Debug");
    }
}
