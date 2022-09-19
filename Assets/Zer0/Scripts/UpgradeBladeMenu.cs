using System;
using TMPro;
using UnityEngine;

namespace Zer0
{
    public class UpgradeBladeMenu : UpgradeMenu
    {
        [Header("Cost Text Fields")]
        [SerializeField, Tooltip("Text field to display the cost of the next knife damage enhancement.")] 
        private TMP_Text knifeDamageCostText;
        [SerializeField, Tooltip("Text field to display the cost of the next chain attack damage enhancement.")] 
        private TMP_Text chainStrikeDamageCostText;
        [SerializeField, Tooltip("Text field to display the cost of the next chain attack dot enhancement.")]
        private TMP_Text chainStrikeDotCostText;
        [SerializeField, Tooltip("Text field to display the cost to an enhancement to pull struck enemies toward the player.")]
        private TMP_Text chainStrikeAddChainPull;
        [SerializeField, Tooltip("Text field to display the cost of the next chain length enhancement.")] 
        private TMP_Text lengthCostText;
        [SerializeField, Tooltip("Text field to display the current number of points available.")] 
        private TMP_Text currentPointsText;
        [SerializeField, Tooltip("Text field to display the total number of points invested.")]
        private TMP_Text totalPointsText;

        [Header("Description Text Fields")]
        [SerializeField, Tooltip("Text field that holds the description of the knife damage upgrade.")]
        private TMP_Text knifeDamageText;
        [SerializeField, Tooltip("Text field that holds the description of the chain damage upgrade.")]
        private TMP_Text chainDamageText;
        [SerializeField, Tooltip("Text field that holds the description of the chain length upgrade")]
        private TMP_Text chainLengthText;
        [SerializeField, Tooltip("Text field that holds the description of the chain dot upgrade.")]
        private TMP_Text chainDotText;
        [SerializeField, Tooltip("Text field that holds the description of the chain drag upgrade.")]
        private TMP_Text chainDragText;
        
        [Header("Cost Multiplier for each upgrade type")]
        [SerializeField, Tooltip("The cost multiplier for each level of chain attack dot enhancement.")]
        private int dotMultiplier = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of chain attack damage enhancement.")]
        private int chainStrikeDamageMultiplier = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of knife damage enhancement.")]
        private int knifeDamageMultiplier = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of chain length enhancement.")] 
        private int lengthMultiplier = 1;
        [SerializeField, Tooltip("The one time cost to add chain pull to the chain strike attack.")]
        private int chainPullMultiplier = 1;

        [Header("Amounts For Each Upgrade")]
        [SerializeField, Tooltip("The amount of damage that will be added with each blade damage upgrade.")]
        private int bladeDamageUpgrade = 3;
        [SerializeField, Tooltip("The amount of damage that will be added with each chain damage upgrade.")]
        private int chainDamageUpgrade = 1;
        [SerializeField, Tooltip("The number of chain links that will be added to the chains length with each length upgrade")]
        private int chainLengthUpgrade = 3;
        [SerializeField, Tooltip("The number of seconds added to the damage over time effect with each chain dot upgrade.")]
        private float chainDotUpgrade = 2;

        private int _baseKnifeDamageCost = 1;
        private int _baseChainStrikeDamageCost = 1;
        private int _baseChainStrikeDotCost = 1;
        private int _baseChainPullCost = 1;
        private int _baseLengthCost = 1;
        
        private int _currentPoints;
        private int _totalPoints;
        private bool _chainPullAdded;

        public static event Action<weaponType, int> OnDamageUpgrade;
        public static event Action<statusEffectType, weaponType, float, float, float> OnAddStatusEffect;
        public static event Action<int> OnChainLengthUpgrade;
        public static event Action<weaponType, bool> OnChainPullUpgrade;

        private void Start()
        {
            UpgradeTopMenu.OnSpentLink += IncrementCollected;
            gameObject.SetActive(false);
        }
        
        public void OnOpen()
        {
            DescriptionText();
            
            chainStrikeDamageCostText.text = $"{chainStrikeDamageMultiplier * _baseKnifeDamageCost}";
            chainStrikeDotCostText.text = $"{_baseChainStrikeDotCost * dotMultiplier}";
            knifeDamageCostText.text = $"{knifeDamageMultiplier * _baseKnifeDamageCost}";
            lengthCostText.text = $"{lengthMultiplier * _baseLengthCost}";
            currentPointsText.text = $"{_currentPoints}";
            totalPointsText.text = $"{_totalPoints}";

            if (_chainPullAdded)
                chainStrikeAddChainPull.text = "Max";
            else
                chainStrikeAddChainPull.text = $"{_baseChainPullCost * chainPullMultiplier}";
        }

        private void DescriptionText()
        {
            knifeDamageText.text =
                $"Each enhancement in this category will increase the damage of each knife attack by {bladeDamageUpgrade}.";
            chainDamageText.text =
                $"Each enhancement in this category will increase the damage of each knife attack by {chainDamageUpgrade}.";
            chainLengthText.text =
                $"Each enhancement in this category will increase the length of the Chain Strike by {chainLengthUpgrade * 2} chain links.";
            chainDotText.text = _baseChainStrikeDotCost > 1 ? $"Each enhancement in this category will increase the duration of the Damage over time effect by {chainDotUpgrade} seconds." : 
                $"Add a damage over time effect to the Chain Strike attack that deals 2 damage a second for {chainDotUpgrade * 2} seconds.";
            chainDragText.text =
                "Add the ability to drag enemies struck by Chain Strike back toward the player.";
        }
        
        public void UpgradeKnifeDamage()
        {
            if (CheckCanUpgrade(_baseKnifeDamageCost, knifeDamageMultiplier))
            {
                OnDamageUpgrade?.Invoke(weaponType.knifeBlade, bladeDamageUpgrade);
                _baseKnifeDamageCost++;
                knifeDamageCostText.text = $"{knifeDamageMultiplier * _baseKnifeDamageCost}";
            }
        }
        
        public void UpgradeChainStrikeDamage()
        {
            if (CheckCanUpgrade(_baseChainStrikeDamageCost, chainStrikeDamageMultiplier))
            {
                OnDamageUpgrade?.Invoke(weaponType.chainEnd, chainDamageUpgrade);
                _baseChainStrikeDamageCost++;
                chainStrikeDamageCostText.text = $"{chainStrikeDamageMultiplier * _baseChainStrikeDamageCost}";
            }
        }

        public void AddDotEffect()
        {
            if (CheckCanUpgrade(_baseChainStrikeDotCost, dotMultiplier))
            {
                if (_baseChainStrikeDotCost <= 1)
                    OnAddStatusEffect?.Invoke(statusEffectType.Dot, weaponType.chainEnd, chainDotUpgrade * 2, 2, 1);
                else
                    OnAddStatusEffect?.Invoke(statusEffectType.Dot, weaponType.chainEnd, chainDotUpgrade, 0, 0);
                
                _baseChainStrikeDotCost++;
                chainStrikeDotCostText.text = $"{_baseChainStrikeDotCost * dotMultiplier}";
            }
        }
        
        public void UpgradeChainLength()
        {
            if (CheckCanUpgrade(_baseLengthCost, lengthMultiplier))
            {
                OnChainLengthUpgrade?.Invoke(chainLengthUpgrade);
                _baseLengthCost++;
                lengthCostText.text = $"{lengthMultiplier * _baseLengthCost}";
            }
        }

        public void AddChainPull()
        {
            if (CheckCanUpgrade(_baseChainPullCost, chainPullMultiplier))
            {
                OnChainPullUpgrade?.Invoke(weaponType.chainEnd, true);
                _baseChainPullCost = 100000;
                _chainPullAdded = true;
                
                chainStrikeAddChainPull.text = _chainPullAdded ? "Max" : $"{_baseChainPullCost * chainPullMultiplier}";
            }
        }
        
        private void IncrementCollected(UpgradeMenu check, int amount)
        {
            if (check != this) return;

            _currentPoints += amount;
            _totalPoints += amount;
        }
        
        private bool CheckCanUpgrade(int upgradeCost, int upgradeMultiplier)
        {
            if (_currentPoints >= upgradeCost * upgradeMultiplier)
            {
                _currentPoints -= upgradeCost * upgradeMultiplier;
                currentPointsText.text = $"{_currentPoints}";
                return true;
            }

            return false;
        }
    }
}
