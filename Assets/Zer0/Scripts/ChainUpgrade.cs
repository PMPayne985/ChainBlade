using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Zer0
{
    public class ChainUpgrade : MonoBehaviour
    {
        public static ChainUpgrade Instance;
        
        private int _linksCollected;
        private int _chainLength;
        private int _chainKnifeUpgrades;
        private bool _menuOpen;

        private int _lengthMultiplyer = 1;
        private int _damageMultiplyer = 1;

        [SerializeField] private GameObject screenDarken;
        [SerializeField] private GameObject upgradeMenu;
        [SerializeField] private TMP_Text linkText;
        [SerializeField] private TMP_Text lengthCostText;
        [SerializeField] private TMP_Text damageCostText;

        public event Action<int> OnChainLengthUpgrade;
        public event Action<float> OnKnifeDamageUpgrade; 

        private void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
                Instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Tab))
                ToggleUpgradeMenu();
        }

        private void Start()
        {
            Collection.Instance.OnCollectedLink += IncrementCollected;
        }

        public void ToggleUpgradeMenu()
        {
            _menuOpen = !_menuOpen;

            if (_menuOpen)
            {
                Cursor.lockState = CursorLockMode.Confined;
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
                screenDarken.SetActive(false);
                upgradeMenu.SetActive(false);
            }
        }
        
        private void IncrementCollected(GameObject collected)
        {
            _linksCollected ++;
        }

        public void UpgradeChainLength()
        {
            if (_linksCollected >= _lengthMultiplyer)
            {
                _linksCollected -= _lengthMultiplyer;
                OnChainLengthUpgrade?.Invoke(1);
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
