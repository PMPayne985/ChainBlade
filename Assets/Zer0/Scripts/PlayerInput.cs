using System;
using UnityEngine;

namespace Zer0
{
    public class PlayerInput : MonoBehaviour
    {
        public static bool ChainPreview() => Input.GetKeyDown(KeyCode.LeftControl);
        public static bool EndChainPreview() => Input.GetKeyUp(KeyCode.LeftControl);

        public static bool PauseMenu() => Input.GetButtonUp("Cancel");

        public static bool UpgradeMenu() => Input.GetButtonUp("Upgrade");
        public static bool NextSpell() => Input.GetButtonUp("NextSpell");
        public static bool CastSpell() => Input.GetKeyUp(KeyCode.Space);

        public static bool Debug() => Input.GetButtonUp("Debug");
    }
}
