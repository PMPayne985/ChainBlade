using System;
using TMPro;
using UnityEngine;

namespace Zer0
{
    public class UpgradeArmorMenu : UpgradeMenu
    {
        [Header("Upgrade Cost Text Fields")]
        [SerializeField, Tooltip("Text field to display the cost of the next health enhancement.")] 
        private TMP_Text healthCostText;
        [SerializeField, Tooltip("Text field to display the cost of the next defence enhancement.")] 
        private TMP_Text defenceCostText;
        [SerializeField, Tooltip("Text field to display the cost of the next speed enhancement.")] 
        private TMP_Text speedCostText;
        [SerializeField, Tooltip("Text field to display the cost of the next resistance enhancement.")] 
        private TMP_Text resistanceCostText;
        [SerializeField, Tooltip("Text field to display the cost of the next retaliation enhancement.")]
        private TMP_Text retaliationCostText;

        [Header("Description Text Fields")] 
        [SerializeField, Tooltip("Text field to display the")]
        private TMP_Text healthDescriptionText;
        [SerializeField, Tooltip("Text field to display the")]
        private TMP_Text defenceDescriptionText;
        [SerializeField, Tooltip("Text field to display the")]
        private TMP_Text speedDescriptionText;
        [SerializeField, Tooltip("Text field to display the")]
        private TMP_Text resistanceDescriptionText;
        [SerializeField, Tooltip("Text field to display the")]
        private TMP_Text retaliationDescriptionText;
        
        [Header("Upgrade Points Text Fields")]
        [SerializeField, Tooltip("Text field to display the current number of points available.")] 
        private TMP_Text currentPointsText;
        [SerializeField, Tooltip("Text field to display the total number of points invested.")]
        private TMP_Text totalPointsText;

        [Header("Upgrade Cost Values")]
        [SerializeField, Tooltip("The cost multiplier for each level of health enhancement")] 
        private float healthMultiplier = 1;
        [SerializeField, Tooltip("")] 
        private int healthAddition = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of defence enhancement")] 
        private float defenceMultiplier = 1;
        [SerializeField, Tooltip("")] 
        private int defenceAddition = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of speed enhancement")] 
        private float speedMultiplier = 1;
        [SerializeField, Tooltip("")] 
        private int speedAddition = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of resistance enhancement")] 
        private float resistanceMultiplier = 1;
        [SerializeField, Tooltip("")] 
        private int resistanceAddition = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of retaliation enhancement")] 
        private float retaliationMultiplier = 1;
        [SerializeField, Tooltip("")] 
        private int retaliationAddition = 1;
        
        [Header("Upgrade Values")]
        [SerializeField, Tooltip("The amount of health added to the players max health with each upgrade.")]
        private int healthToAdd= 2;
        [SerializeField, Tooltip("")] 
        private int defenceToAdd = 1;
        [SerializeField, Tooltip("")] 
        private float speedToAdd = 0.05f;
        [SerializeField, Tooltip("")] 
        private int resistanceRate = 5;
        [SerializeField, Tooltip("")] 
        private int retaliationRate = 5;
        [SerializeField, Tooltip("")] 
        private int retaliationDamage = 1;

        private int _baseHealthCost = 1;
        private int _baseDefenceCost = 1;
        private int _baseSpeedCost = 1;
        private int _baseResistanceCost = 1;
        private int _baseRetaliationCost = 1;

        private int _currentPoints;
        private int _totalPoints;

        public static event Action<int> OnMaxHealthUpgrade;
        public static event Action<int> OnDefenceUpgrade;
        public static event Action<float> OnSpeedUpgrade;
        public static event Action<int> OnIncreaseResistance;
        public static event Action<int, int> OnIncreaseRetaliation;

        private void Start()
        {
            UpgradeTopMenu.OnSpentLink += IncrementCollected;
            gameObject.SetActive(false);
        }

        public void OnOpen()
        {
            DescriptionText();
            
            currentPointsText.text = $"{_currentPoints}";
            totalPointsText.text = $"{_totalPoints}";
            
            var uCost = Mathf.RoundToInt(_baseHealthCost * healthMultiplier);
            uCost += healthAddition;
            healthCostText.text = $"{uCost}";
            var dCost = Mathf.RoundToInt(_baseDefenceCost * defenceMultiplier);
            dCost += defenceAddition;
            defenceCostText.text = $"{dCost}";
            var sCost = Mathf.RoundToInt(_baseSpeedCost * speedMultiplier);
            sCost += speedAddition;
            speedCostText.text = $"{sCost}";
            var resCost = Mathf.RoundToInt(_baseResistanceCost * resistanceMultiplier);
            resCost += resistanceAddition;
            resistanceCostText.text = $"{resCost}";
            var retCost = Mathf.RoundToInt(_baseRetaliationCost * retaliationMultiplier);
            retCost += retaliationAddition;
            retaliationCostText.text = $"{retCost}";
        }

        private void DescriptionText()
        {
            healthDescriptionText.text = 
                $"Increase your maximum health by {healthToAdd} points.";
            defenceDescriptionText.text = 
                $"Increase the amount of damage your armor absorbs from every attack by {defenceToAdd} points.";
            speedDescriptionText.text = 
                $"Increase your maximum movement speed by {speedToAdd * 100}%.";
            resistanceDescriptionText.text =
                $"Reduce the chance of being affected by a negative status effect by {resistanceRate}%";
            retaliationDescriptionText.text =
                $"Increase your chance to deal {retaliationDamage} damage to any attacker within melee range by {retaliationRate}%. " +
                $"Every 3rd enhancment will increase the retaliation damage by {retaliationDamage}.";
            
        }
        
        public void UpgradeMaxHealth()
        {
            var uCost = Mathf.RoundToInt(_baseHealthCost * healthMultiplier);
            uCost += healthAddition;

            healthCostText.text = $"{uCost}";
            
            if (CheckCanUpgrade(_baseHealthCost, healthMultiplier, healthAddition))
            {
                OnMaxHealthUpgrade?.Invoke(healthToAdd);
                _baseHealthCost++;
            }
            
            uCost = Mathf.RoundToInt(_baseHealthCost * healthMultiplier);
            uCost += healthAddition;

            healthCostText.text = $"{uCost}";
        }
        
        public void UpgradeDefence()
        {
            var dCost = Mathf.RoundToInt(_baseDefenceCost * defenceMultiplier);
            dCost += defenceAddition;
            defenceCostText.text = $"{dCost}";
            
            if (CheckCanUpgrade(_baseDefenceCost, defenceMultiplier, defenceAddition))
            {
                OnDefenceUpgrade?.Invoke(defenceToAdd);
                _baseDefenceCost++;
            }
            
            dCost = Mathf.RoundToInt(_baseDefenceCost * defenceMultiplier);
            dCost += defenceAddition;
            defenceCostText.text = $"{dCost}";
        }
        
        public void UpgradeSpeed()
        {
            var sCost = Mathf.RoundToInt(_baseSpeedCost * speedMultiplier);
            sCost += speedAddition;
            speedCostText.text = $"{sCost}";
            
            if (CheckCanUpgrade(_baseSpeedCost, speedMultiplier, speedAddition))
            {
                OnSpeedUpgrade?.Invoke(speedToAdd);
                _baseSpeedCost++;
            }
            
            sCost = Mathf.RoundToInt(_baseSpeedCost * speedMultiplier);
            sCost += speedAddition;
            speedCostText.text = $"{sCost}";
        }
        
        public void UpgradeResistance()
        {
            var resCost = Mathf.RoundToInt(_baseResistanceCost * resistanceMultiplier);
            resCost += resistanceAddition;
            resistanceCostText.text = $"{resCost}";
            
            if (CheckCanUpgrade(_baseResistanceCost, resistanceMultiplier, resistanceAddition))
            {
                OnIncreaseResistance?.Invoke(resistanceRate);
                _baseResistanceCost++;
            }
            
            resCost = Mathf.RoundToInt(_baseResistanceCost * resistanceMultiplier);
            resCost += resistanceAddition;
            resistanceCostText.text = $"{resCost}";
        }
        
        public void UpgradeRetaliation()
        {
            var retCost = Mathf.RoundToInt(_baseRetaliationCost * retaliationMultiplier);
            retCost += retaliationAddition;
            retaliationCostText.text = $"{retCost}";
            
            if (CheckCanUpgrade(_baseRetaliationCost, retaliationMultiplier, retaliationAddition))
            {
                OnIncreaseRetaliation?.Invoke(0, retaliationRate);
                _baseRetaliationCost++;
                
                if (_baseRetaliationCost % 3 == 0)
                    OnIncreaseRetaliation?.Invoke(retaliationDamage, 0);
            }
            
            retCost = Mathf.RoundToInt(_baseRetaliationCost * retaliationMultiplier);
            retCost += retaliationAddition;
            retaliationCostText.text = $"{retCost}";
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
    }
}
