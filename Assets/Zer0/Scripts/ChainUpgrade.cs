using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Zer0
{
    public class ChainUpgrade : MonoBehaviour
    {
        private int _linksCollected;
        private int _chainLength;
        private int _chainKnifeUpgrades;
        public bool EnhancementOpen { get; private set; }

        [SerializeField] private int lengthMultiplier = 1;
        private int _baseLengthCost = 1;
        [SerializeField] private int damageMultiplier = 1;
        private int _baseDamageCost = 1;
        [SerializeField] private int healthMultiplier = 1;
        private int _baseHealthCost = 1;

        [SerializeField] private GameObject screenDarken;
        [SerializeField] private GameObject upgradeMenu;
        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] private TMP_Text linkText;
        [SerializeField] private TMP_Text lengthCostText;
        [SerializeField] private TMP_Text damageCostText;
        [SerializeField] private TMP_Text healthCostText;

        public static event Action<int> OnChainLengthUpgrade;
        public static event Action<float> OnKnifeDamageUpgrade;
        public static event Action<float> OnMaxHealthUpgrade; 

        private void Update()
        {
            if (PlayerInput.UpgradeMenu())
                ToggleUpgradeMenu();
        }

        private void Start()
        {
            LinkCollectible.OnCollectedLink += IncrementCollected;
        }

        public void ToggleUpgradeMenu()
        {
            if (pauseMenu.Paused) return;
            
            EnhancementOpen = !EnhancementOpen;

            if (EnhancementOpen)
            {
                Cursor.lockState = CursorLockMode.Confined;
                FindObjectOfType<ChainKnife>().WakeUpAllKnives();
                upgradeMenu.SetActive(true);
                screenDarken.SetActive(true);
                linkText.text = $"Current Links: {_linksCollected}";
                lengthCostText.text = $"{lengthMultiplier * _baseLengthCost}";
                damageCostText.text = $"{damageMultiplier * _baseDamageCost}";
                healthCostText.text = $"{healthMultiplier * _baseHealthCost}";
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                FindObjectOfType<ChainKnife>().ResetAllBlades();
                screenDarken.SetActive(false);
                upgradeMenu.SetActive(false);
            }
        }
        
        private void IncrementCollected(Collectible collected) 
            => _linksCollected++;

        public void UpgradeChainLength()
        {
            if (CheckCanUpgrade(_baseLengthCost, lengthMultiplier))
            {
                OnChainLengthUpgrade?.Invoke(3);
                _baseLengthCost++;
                lengthCostText.text = $"{lengthMultiplier * _baseLengthCost}";
            }
        }

        public void UpgradeKnifeDamage()
        {
            if (CheckCanUpgrade(_baseDamageCost, damageMultiplier))
            {
                OnKnifeDamageUpgrade?.Invoke(1);
                _baseDamageCost++;
                damageCostText.text = $"{damageMultiplier * _baseDamageCost}";
            }
        }

        public void UpgradeMaxHealth()
        {
            if (CheckCanUpgrade(_baseHealthCost, healthMultiplier))
            {
                OnMaxHealthUpgrade?.Invoke(1);
                _baseHealthCost++;
                healthCostText.text = $"{healthMultiplier * _baseHealthCost}";
            }
        }

        private bool CheckCanUpgrade(int upgradeCost, int upgradeMultiplier)
        {
            if (_linksCollected >= upgradeCost * upgradeMultiplier)
            {
                _linksCollected -= upgradeCost * upgradeMultiplier;
                linkText.text = $"Current Links: {_linksCollected}";
                return true;
            }

            return false;
        }
    }
}
