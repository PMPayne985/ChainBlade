using System;
using TMPro;
using UnityEngine;

namespace Zer0
{
    public class UpgradeBladeMenu : UpgradeMenu, ISaveable
    {
        [Header("Cost Text Fields")]
        [SerializeField, Tooltip("Text field to display the cost of the next knife damage enhancement.")] 
        private TMP_Text knifeDamageCostText;
        [SerializeField, Tooltip("Text field to display the cost of the next chain attack damage enhancement.")] 
        private TMP_Text chainDamageCostText;
        [SerializeField, Tooltip("")]
        private TMP_Text knifeLifeLeechCostText;
        [SerializeField, Tooltip("Text field to display the cost to an enhancement to pull struck enemies toward the player.")]
        private TMP_Text chainPullCostText;
        [SerializeField, Tooltip("Text field to display the cost of the next chain length enhancement.")] 
        private TMP_Text lengthCostText;
        
        [Header("Link info Text Fields")]
        [SerializeField, Tooltip("Text field to display the current number of points available.")] 
        private TMP_Text currentPointsText;
        [SerializeField, Tooltip("Text field to display the total number of points invested.")]
        private TMP_Text totalPointsText;

        [Header("Description Text Fields")]
        [SerializeField, Tooltip("Text field that holds the description of the knife damage upgrade.")]
        private TMP_Text knifeDamageText;
        [SerializeField, Tooltip("Text field that holds the description of the chain damage upgrade.")]
        private TMP_Text chainDamageText;
        [SerializeField, Tooltip("")]
        private TMP_Text knifeLifeLeechText;
        [SerializeField, Tooltip("Text field that holds the description of the chain length upgrade")]
        private TMP_Text chainLengthText;
        [SerializeField, Tooltip("Text field that holds the description of the chain drag upgrade.")]
        private TMP_Text chainDragText;
        
        [Header("Cost Modifiers for each upgrade type")]
        [SerializeField, Tooltip("The cost multiplier for each level of knife damage enhancement.")]
        private float knifeDamageMultiplier = 1;
        [SerializeField, Tooltip("")] 
        private int knifeDamageAddition = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of chain attack damage enhancement.")]
        private float chainDamageMultiplier = 1;
        [SerializeField, Tooltip("")] 
        private int chainDamageAddition = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of Life Leech damage enhancement.")]
        private float knifeLifeLeechMultiplier = 1;
        [SerializeField, Tooltip("")] 
        private int knifeLifeLeechAddition = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of chain length enhancement.")] 
        private float lengthMultiplier = 1;
        [SerializeField, Tooltip("")] 
        private int lengthAddition = 1;
        [SerializeField, Tooltip("The one time cost to add chain pull to the chain strike attack.")]
        private float chainPullMultiplier = 1;
        [SerializeField, Tooltip("")] 
        private int chainPullAddition = 1;
        
        [Header("Amounts For Each Upgrade")]
        [SerializeField, Tooltip("The amount of damage that will be added with each blade damage upgrade.")]
        private int bladeDamageUpgrade = 3;
        [SerializeField, Tooltip("The amount of damage that will be added with each chain damage upgrade.")]
        private int chainDamageUpgrade = 1;
        [SerializeField, Tooltip("")]
        private int knifeLifeLeechUpgrade = 1;
        [SerializeField, Tooltip("The number of chain links that will be added to the chains length with each length upgrade")]
        private int chainLengthUpgrade = 3;

        private int _baseKnifeDamageCost = 1;
        private int _baseChainDamageCost = 1;
        private int _baseLifeLeechCost = 1;
        private int _baseChainPullCost = 1;
        private int _baseLengthCost = 1;
        
        private int _currentPoints;
        private int _totalPoints;
        private bool _chainPullAdded;

        public static event Action<weaponType, int> OnDamageUpgrade;
        public static event Action<statusEffectType, weaponType, float, float, float> OnAddStatusEffect;
        public static event Action<int> OnChainLengthUpgrade;
        public static event Action<weaponType, bool> OnChainPullUpgrade;
        public static event Action<weaponType, bool, int> OnUpgradeLifeLeech;

        private void Start()
        {
            UpgradeTopMenu.OnSpentLink += IncrementCollected;
            gameObject.SetActive(false);
        }
        
        public void OnOpen()
        {
            DescriptionText();

            totalPointsText.text = $"{_totalPoints}";
            currentPointsText.text = $"{_currentPoints}";
        }

        private void DescriptionText()
        {
            knifeDamageText.text =
                $"Each enhancement in this category will increase the damage of each knife attack by {bladeDamageUpgrade}.";
            chainDamageText.text =
                $"Each enhancement in this category will increase the damage of each chain strike by {chainDamageUpgrade}.";
            knifeLifeLeechText.text = 
                $"Each enhancement in this category will allow the player to steal {knifeLifeLeechUpgrade} points of health from a struck enemy.";
            chainLengthText.text = 
                $"Each enhancement in this category will increase the length of chain strikes by {chainLengthUpgrade * 2} links.";
            chainDragText.text =
                "Add the ability to drag enemies struck by a chain strike back toward the player.";
            
            var kCost = _baseKnifeDamageCost * knifeDamageMultiplier;
            kCost += knifeDamageAddition;
            knifeDamageCostText.text = $"{kCost}";
            var cCost = _baseChainDamageCost * chainDamageMultiplier;
            cCost += chainDamageAddition;
            chainDamageCostText.text = $"{cCost}";
            var pCost = _baseChainPullCost * chainPullMultiplier;
            pCost += chainPullAddition;
            chainPullCostText.text = _chainPullAdded ? "Max" : $"{pCost}";
            var lCost = _baseLifeLeechCost * knifeLifeLeechMultiplier;
            lCost += knifeLifeLeechAddition;
            knifeLifeLeechCostText.text = $"{lCost}";
            var lnCost = _baseLengthCost * lengthMultiplier;
            lnCost += lengthAddition;
            lengthCostText.text = $"{lnCost}";
        }
        
        public void UpgradeKnifeDamage()
        {
            DescriptionText();
            
            if (CheckCanUpgrade(_baseKnifeDamageCost, knifeDamageMultiplier, knifeDamageAddition))
            {
                OnDamageUpgrade?.Invoke(weaponType.knifeBlade, bladeDamageUpgrade);
                _baseKnifeDamageCost++;
            }
            
            DescriptionText();
        }
        
        public void UpgradeChainStrikeDamage()
        {
            DescriptionText();
            
            if (CheckCanUpgrade(_baseChainDamageCost, chainDamageMultiplier, chainDamageAddition))
            {
                OnDamageUpgrade?.Invoke(weaponType.chainEnd, chainDamageUpgrade);
                _baseChainDamageCost++;
            }
            
            DescriptionText();
        }

        public void UpgradeLifeLeech()
        {
            DescriptionText();

            if (CheckCanUpgrade(_baseLifeLeechCost, knifeLifeLeechMultiplier, knifeLifeLeechAddition))
            {
                OnUpgradeLifeLeech?.Invoke(weaponType.knifeBlade, true, knifeLifeLeechUpgrade);
                _baseLifeLeechCost++;
            }
            
            DescriptionText();
        }

        public void UpgradeChainLength()
        {
            DescriptionText();
            
            if (CheckCanUpgrade(_baseLengthCost, lengthMultiplier, lengthAddition))
            {
                OnChainLengthUpgrade?.Invoke(chainLengthUpgrade);
                _baseLengthCost++;
            }
            
            DescriptionText();
        }

        public void AddChainPull()
        {
            DescriptionText();
            
            if (CheckCanUpgrade(_baseChainPullCost, chainPullMultiplier, chainPullAddition))
            {

                OnChainPullUpgrade?.Invoke(weaponType.chainEnd, true);
                _baseChainPullCost = 100000;
                _chainPullAdded = true;
            }
            
            DescriptionText();
        }
        
        private void IncrementCollected(UpgradeMenu check, int amount)
        {
            if (check != this) return;

            _currentPoints += amount;
            _totalPoints += amount;
        }
        
        private bool CheckCanUpgrade(int upgradeCost, float upgradeMultiplier, int upgradeAddition)
        {
            var upgradeFinalCost = Mathf.RoundToInt(upgradeCost * upgradeMultiplier);
            upgradeFinalCost += upgradeAddition;
            
            if (_currentPoints >= upgradeFinalCost)
            {
                _currentPoints -= upgradeFinalCost;
                currentPointsText.text = $"{_currentPoints}";
                return true;
            }
            return false;
        }

        public void SaveData()
        {
            SavedStats.Instance.bLength = _baseLengthCost;
            SavedStats.Instance.bChainDamage = _baseChainDamageCost;
            SavedStats.Instance.bChainPull = _baseChainPullCost;
            SavedStats.Instance.bCurrentPoints = _currentPoints;
            SavedStats.Instance.bKnifeDamage = _baseKnifeDamageCost;
            SavedStats.Instance.bLifeLeech = _baseLifeLeechCost;
            SavedStats.Instance.bTotalPoints = _totalPoints;
            SavedStats.Instance.bChainPullAdded = _chainPullAdded;
        }

        public void LoadData()
        {
            _baseLengthCost = SavedStats.Instance.bLength;
            _baseChainDamageCost = SavedStats.Instance.bChainDamage;
            _baseChainPullCost = SavedStats.Instance.bChainPull;
            _currentPoints = SavedStats.Instance.bCurrentPoints;
            _baseKnifeDamageCost = SavedStats.Instance.bKnifeDamage;
            _baseLifeLeechCost = SavedStats.Instance.bLifeLeech;
            _totalPoints = SavedStats.Instance.bTotalPoints;
            _chainPullAdded = SavedStats.Instance.bChainPullAdded;
        }
    }
}
