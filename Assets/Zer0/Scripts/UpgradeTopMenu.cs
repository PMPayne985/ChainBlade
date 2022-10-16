using TMPro;
using System;
using UnityEngine;

namespace Zer0
{
    public class UpgradeTopMenu : UpgradeMenu, ISaveable
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
            if (DebugMenu.Instance)
                DebugMenu.OnAddLinksCommand += AddLinksWithoutCollectible;
            gameObject.SetActive(false);
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
            if (CheckCanUpgrade())
            {
                OnSpentLink?.Invoke(upgradeMenu.UpgradeArmorMenu, upgradePointsPerLink);
                _armorUpgrades++;
                currentLinkText.text = $"{_currentLinks}";
                armorLinksSpentText.text = $"{_armorUpgrades}";
            }
        }
        
        public void BuyBladePoints()
        {
            if (CheckCanUpgrade())
            {
                OnSpentLink?.Invoke(upgradeMenu.UpgradeBladeMenu, upgradePointsPerLink);
                _bladeUpgrades++;
                currentLinkText.text = $"{_currentLinks}";
                bladeLinksSpentText.text = $"{_bladeUpgrades}";
            }
        }
        
        public void BuySpellPoints()
        {
            if (CheckCanUpgrade())
            {
                OnSpentLink?.Invoke(upgradeMenu.UpgradeSpellMenu, upgradePointsPerLink);
                _spellUpgrades++;
                currentLinkText.text = $"{_currentLinks}";
                spellLinksSpentText.text = $"{_spellUpgrades}";
            }
        }
        
        private void IncrementCollected(Collectible collected)
        {
            _currentLinks++;
            _totalLinks++;
            
            currentLinkText.text = $"{_currentLinks}";
            totalLinkText.text = $"{_totalLinks}";
        }

        private void AddLinksWithoutCollectible(int newLinks)
        {
            _currentLinks += newLinks;
            _totalLinks += newLinks;
            
            currentLinkText.text = $"{_currentLinks}";
            totalLinkText.text = $"{_totalLinks}";
        }
        
        private bool CheckCanUpgrade()
        {
            if (_currentLinks > 0)
            {
                _currentLinks--;
                currentLinkText.text = $"{_currentLinks}";
                return true;
            }
            
            Logging.LogMessage(errorLevel.Log, Color.black, "Not enough Links!");
            return false;
        }

        public void SaveData()
        {
            SavedStats.Instance.currentLinks = _currentLinks;
            SavedStats.Instance.maxLinks = _totalLinks;
            SavedStats.Instance.armorUpgrade = _armorUpgrades;
            SavedStats.Instance.bladeUpgrade = _bladeUpgrades;
            SavedStats.Instance.spellUpgrade = _spellUpgrades;
        }

        public void LoadData()
        {
            _currentLinks = SavedStats.Instance.currentLinks;
            _totalLinks = SavedStats.Instance.maxLinks;
            _armorUpgrades = SavedStats.Instance.armorUpgrade;
            _bladeUpgrades = SavedStats.Instance.bladeUpgrade;
            _spellUpgrades = SavedStats.Instance.spellUpgrade;
        }
    }
}
