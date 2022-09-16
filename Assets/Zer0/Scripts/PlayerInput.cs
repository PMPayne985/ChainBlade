using System;
using UnityEngine;

namespace Zer0
{
    public class PlayerInput : MonoBehaviour
    {
        public static bool ChainPreview() => Input.GetKeyDown(KeyCode.LeftControl);
        public static bool EndChainPreview() => Input.GetKeyUp(KeyCode.LeftControl);

        public static bool PauseMenu() => Input.GetKeyDown(KeyCode.P);

        public static bool UpgradeMenu() => Input.GetKeyDown(KeyCode.U);
        public static bool NextSpell() => Input.GetKeyDown(KeyCode.F);
        public static bool CastSpell() => Input.GetKeyUp(KeyCode.Space);

        public static bool Debug() => Input.GetKeyDown(KeyCode.BackQuote);
    }
}
