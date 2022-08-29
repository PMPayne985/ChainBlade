using TMPro;
using System;
using UnityEngine;

namespace Zer0
{
    public class UpgradeTopMenu : UpgradeMenu
    {
        [SerializeField, Tooltip("The 'ChainUpgrade' script that will control this menu.")]
        private ChainUpgrade upgradeMenu;
        
        [SerializeField, Tooltip("Text field to display the current number of links collected.")] 
        private TMP_Text currentLinkText;
        [SerializeField, Tooltip("Text field to display the total number of links collected.")]
        private TMP_Text totalLinkText;
        
        [SerializeField, Tooltip("Text field to display the total number of links spent on Armor enhancements.")]
        private TMP_Text armorLinksSpentText;
        [SerializeField, Tooltip("Text field to display the total number of links spent on Blade enhancements.")]
        private TMP_Text bladeLinksSpentText;
        [SerializeField, Tooltip("Text field to display the total number of links spent on Spell enhancements.")]
        private TMP_Text spellLinksSpentText;

        [SerializeField, Tooltip("The number of upgrade points each spent link will buy.")]
        private int upgradePointsPerLink = 2;
        
        private int _currentLinks;
        private int _totalLinks;
        
        private int _armorUpgrades;
        private int _bladeUpgrades;
        private int _spellUpgrades;
        
        public static event Action<UpgradeMenu, int> OnSpentLink;

        private void Start()
        {
            LinkCollectible.OnCollectedLink += IncrementCollected;
        }
        
        public void OnOpen()
        {
            currentLinkText.text = $"{_currentLinks}";
            totalLinkText.text = $"{_totalLinks}";
            armorLinksSpentText.text = $"{_armorUpgrades}";
            bladeLinksSpentText.text = $"{_bladeUpgrades}";
            spellLinksSpentText.text = $"{_spellUpgrades}";
        }

        public void BuyArmorPoints()
        {
            OnSpentLink?.Invoke(upgradeMenu.UpgradeArmorMenu, upgradePointsPerLink);
            _currentLinks--;
            currentLinkText.text = $"{_currentLinks}";
            armorLinksSpentText.text = $"{_armorUpgrades}";
        }
        
        public void BuyBladePoints()
        {
            OnSpentLink?.Invoke(upgradeMenu.UpgradeBladeMenu, upgradePointsPerLink);
            _currentLinks--;
            currentLinkText.text = $"{_currentLinks}";
            bladeLinksSpentText.text = $"{_bladeUpgrades}";
        }
        
        public void BuySpellPoints()
        {
            OnSpentLink?.Invoke(upgradeMenu.UpgradeSpellMenu, upgradePointsPerLink);
            _currentLinks--;
            currentLinkText.text = $"{_currentLinks}";
            spellLinksSpentText.text = $"{_spellUpgrades}";
        }
        
        private void IncrementCollected(Collectible collected)
        {
            _currentLinks++;
            _totalLinks++;
        }
        
        private bool CheckCanUpgrade(int upgradeCost, int upgradeMultiplier)
        {
            if (_currentLinks >= upgradeCost * upgradeMultiplier)
            {
                _currentLinks -= upgradeCost * upgradeMultiplier;
                currentLinkText.text = $"Current Links: {_currentLinks}";
                return true;
            }

            return false;
        }
    }
}
