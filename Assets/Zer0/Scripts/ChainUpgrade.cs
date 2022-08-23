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

        private int _lengthMultiplyer = 1;
        private int _damageMultiplyer = 1;

        [SerializeField] private GameObject screenDarken;
        [SerializeField] private GameObject upgradeMenu;
        [SerializeField] private PauseMenu _pauseMenu;
        [SerializeField] private TMP_Text linkText;
        [SerializeField] private TMP_Text lengthCostText;
        [SerializeField] private TMP_Text damageCostText;

        public static event Action<int> OnChainLengthUpgrade;
        public static event Action<float> OnKnifeDamageUpgrade; 

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
            if (_pauseMenu.Paused) return;
            
            EnhancementOpen = !EnhancementOpen;

            if (EnhancementOpen)
            {
                Cursor.lockState = CursorLockMode.Confined;
                FindObjectOfType<ChainKnife>().WakeUpAllKnives();
                upgradeMenu.SetActive(true);
                screenDarken.SetActive(true);
                linkText.text = $"Current Links: {_linksCollected}";
                lengthCostText.text = $"{_lengthMultiplyer}";
                damageCostText.text = $"{_damageMultiplyer}";
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
            if (_linksCollected >= _lengthMultiplyer)
            {
                _linksCollected -= _lengthMultiplyer;
                OnChainLengthUpgrade?.Invoke(3);
                _lengthMultiplyer++;
                linkText.text = $"Current Links: {_linksCollected}";
                lengthCostText.text = $"{_lengthMultiplyer}";
            }
        }

        public void UpgradeKnifeDamage()
        {
            if (_linksCollected >= _damageMultiplyer)
            {
                _linksCollected -= _damageMultiplyer;
                OnKnifeDamageUpgrade?.Invoke(1);
                _damageMultiplyer++;
                linkText.text = $"Current Links: {_linksCollected}";
                damageCostText.text = $"{_damageMultiplyer}";
            }
        }
    }
}
