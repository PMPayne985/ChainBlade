using System;
using UnityEngine;

namespace Zer0
{
    public class PlayerInput : MonoBehaviour
    {

        public static PlayerInput Instance;

        private void Awake()
        {
            if (Instance)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public bool Attack()
        {
            return Input.GetButtonDown("Fire1");
        }

        public bool ChainAttack()
        {
            return Input.GetButtonDown("Fire2");
        }
        
        public bool PauseMenu()
        {
            return Input.GetButtonUp("Cancel");
        }

        public bool UpgradeMenu()
        {
            return Input.GetButtonUp("Upgrade");
        }
    }
}
