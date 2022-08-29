using System;
using TMPro;
using UnityEngine;

namespace Zer0
{
    public class UpgradeArmorMenu : UpgradeMenu
    {
        [SerializeField, Tooltip("Text field to display the cost of the next health enhancement.")] 
        private TMP_Text healthCostText;
        [SerializeField, Tooltip("Text field to display the current number of points available.")] 
        private TMP_Text currentPointsText;
        [SerializeField, Tooltip("Text field to display the total number of points invested.")]
        private TMP_Text totalPointsText;

        [SerializeField, Tooltip("The amount of health added to the players max health with each upgrade.")]
        private float healthToAdd= 2;
        [SerializeField, Tooltip("The cost multiplier for each level of health enhancement")] 
        private int healthMultiplier = 1;
        
        private int _baseHealthCost = 1;

        private int _currentPoints;
        private int _totalPoints;

        public static event Action<float> OnMaxHealthUpgrade;

        private void Start()
        {
            UpgradeTopMenu.OnSpentLink += IncrementCollected;
        }

        public void OnOpen()
        {
            healthCostText.text = $"{healthMultiplier * _baseHealthCost}";
            currentPointsText.text = $"{_currentPoints}";
            totalPointsText.text = $"{_totalPoints}";
        }
        
        public void UpgradeMaxHealth()
        {
            if (CheckCanUpgrade(_baseHealthCost, healthMultiplier))
            {
                OnMaxHealthUpgrade?.Invoke(healthToAdd);
                _baseHealthCost++;
                healthCostText.text = $"{healthMultiplier * _baseHealthCost}";
            }
        }
        
        private void IncrementCollected(UpgradeMenu check, int amount)
        {
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
