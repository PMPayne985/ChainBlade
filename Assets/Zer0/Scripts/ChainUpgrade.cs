using System;
using UnityEngine;

namespace Zer0
{
    public class ChainUpgrade : MonoBehaviour
    {
        [SerializeField, Tooltip("The 'Pause Menu' script that will control game pause function.")] 
        private PauseMenu pauseMenu;
        
        [SerializeField, Tooltip("The backdrop panel that will darken the screen behind the open menu.")] 
        private GameObject screenDarken;
        [SerializeField, Tooltip("The top menu that will open when bringing up the upgrade menu.")] 
        private UpgradeTopMenu upgradeTopMenu;
        [SerializeField, Tooltip("The menu that will open when bringing up the Armor Upgrade page.")] 
        private UpgradeArmorMenu upgradeArmorMenu;
        [SerializeField, Tooltip("The menu that will open when bringing up the Blade Upgrade page.")] 
        private UpgradeBladeMenu upgradeBladeMenu;
        [SerializeField, Tooltip("The menu that will open when bringing up the Spell Book page.")] 
        private UpgradeSpellMenu upgradeSpellMenu;
        
        public UpgradeMenu UpgradeTopMenu => upgradeTopMenu;
        public UpgradeMenu UpgradeArmorMenu => upgradeArmorMenu;
        public UpgradeMenu UpgradeBladeMenu => upgradeBladeMenu;
        public UpgradeMenu UpgradeSpellMenu => upgradeSpellMenu;
        
        public static bool EnhancementOpen { get; private set; }

        private void Awake()
        {
            OpenAllMenus();
        }

        private void Update()
        {
            if (PlayerInput.UpgradeMenu())
                ToggleUpgradeMenu();
        }

        public void ToggleUpgradeMenu()
        {
            if (PauseMenu.Paused) return;
            if (DebugMenu.Instance)
                if (DebugMenu.Instance.MenuOn) return;
            
            EnhancementOpen = !EnhancementOpen;

            var knives = FindObjectsOfType<ChainKnife>();
            
            if (EnhancementOpen)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                foreach (var knife in knives)
                {
                    knife.WakeUpAllKnives();
                }
                OpenTopMenu();
                screenDarken.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                foreach (var knife in knives)
                {
                    knife.ResetAllBlades();
                }
                screenDarken.SetActive(false);
                CloseAllMenus();
            }
        }

        public void OpenTopMenu()
        {
            upgradeTopMenu.gameObject.SetActive(true);
            upgradeArmorMenu.gameObject.SetActive(false);
            upgradeBladeMenu.gameObject.SetActive(false);
            upgradeSpellMenu.gameObject.SetActive(false);
            
            upgradeTopMenu.OnOpen();
        }

        public void OpenArmorMenu()
        {
            upgradeTopMenu.gameObject.SetActive(false);
            upgradeArmorMenu.gameObject.SetActive(true);
            upgradeBladeMenu.gameObject.SetActive(false);
            upgradeSpellMenu.gameObject.SetActive(false);
            
            upgradeArmorMenu.OnOpen();
        }

        public void OpenBladeMenu()
        {
            upgradeTopMenu.gameObject.SetActive(false);
            upgradeArmorMenu.gameObject.SetActive(false);
            upgradeBladeMenu.gameObject.SetActive(true);
            upgradeSpellMenu.gameObject.SetActive(false);
            
            upgradeBladeMenu.OnOpen();
        }

        public void OpenSpellMenu()
        {
            upgradeTopMenu.gameObject.SetActive(false);
            upgradeArmorMenu.gameObject.SetActive(false);
            upgradeBladeMenu.gameObject.SetActive(false);
            upgradeSpellMenu.gameObject.SetActive(true);
            
            upgradeSpellMenu.OnOpen();
        }

        public void OpenAllMenus()
        {
            upgradeTopMenu.gameObject.SetActive(true);
            upgradeArmorMenu.gameObject.SetActive(true);
            upgradeBladeMenu.gameObject.SetActive(true);
            upgradeSpellMenu.gameObject.SetActive(true);
            
            upgradeTopMenu.OnOpen();
            upgradeArmorMenu.OnOpen();
            upgradeBladeMenu.OnOpen();
            upgradeSpellMenu.OnOpen();
        }
        
        public void CloseAllMenus()
        {
            upgradeTopMenu.gameObject.SetActive(false);
            upgradeArmorMenu.gameObject.SetActive(false);
            upgradeBladeMenu.gameObject.SetActive(false);
            upgradeSpellMenu.gameObject.SetActive(false);
        }
    }
}
