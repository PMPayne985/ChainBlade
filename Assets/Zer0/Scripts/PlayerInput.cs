using System;
using UnityEngine;

namespace Zer0
{
    public class PlayerInput : MonoBehaviour
    {
        public static bool Attack() => Input.GetButtonDown("Fire1");

        public static bool ChainAttack() => Input.GetButtonDown("Fire2");

        public static bool Sprint() => Input.GetButton("Sprint");

        public static bool PauseMenu() => Input.GetButtonUp("Cancel");

        public static bool UpgradeMenu() => Input.GetButtonUp("Upgrade");
        
        public static float Horizontal() => Input.GetAxis("Horizontal");

        public static float Vertical() => Input.GetAxis("Vertical");

        public static float Rotation() => Input.GetAxis("Mouse X");
    }
}
