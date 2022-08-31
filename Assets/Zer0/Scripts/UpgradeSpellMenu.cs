using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zer0
{
    public class UpgradeSpellMenu : UpgradeMenu
    {
        [SerializeField, Tooltip("Text field to display the current number of points available.")] 
        private TMP_Text currentPointsText;
        [SerializeField, Tooltip("Text field to display the total number of points invested.")]
        private TMP_Text totalPointsText;
        [SerializeField, Tooltip("Text field to display the cost of the next DoT duration enhancement.")] 
        private TMP_Text dotDurationCostText;
        [SerializeField, Tooltip("Text field to display the cost of the next DoT damage enhancement.")] 
        private TMP_Text dotDamageCostText;
        
        [SerializeField, Tooltip("The cost multiplier for each level of DoT duration enhancement.")]
        private int dotDurationCostMultiplier = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of DoT damage enhancement.")]
        private int dotDamageCostMultiplier = 1;

        [SerializeField] private GameObject buyDotButton;
        [SerializeField] private GameObject[] enhanceDotButtons;
        
        [SerializeField, Tooltip("A list of all spells that can be purchased by the player")]
        private GameObject[] spellList;
        
        private int _currentPoints;
        private int _totalPoints;
        private int _baseDotCost = 1; 

        public static event Action<Spell> OnBuyNewSpell;
        public static event Action<string, float, float, float, float, statusEffectType, bool> OnEnhanceSpell;
        public static event Action<string, Sprite, int, float, float, areaOfEffect> OnChangeSpellParameters;

        private void Start()
        {
            UpgradeTopMenu.OnSpentLink += IncrementCollected;
            gameObject.SetActive(false);
        }
        
        public void OnOpen()
        {
            currentPointsText.text = $"{_currentPoints}";
            totalPointsText.text = $"{_totalPoints}";
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

        public void BuyDotSpell()
        {
            if (CheckCanUpgrade(_baseDotCost, dotDurationCostMultiplier))
            {
                OnBuyNewSpell?.Invoke(spellList[0].GetComponent<Spell>());
                _baseDotCost++;
                dotDurationCostText.text = $"{_baseDotCost * dotDurationCostMultiplier}";
                
                buyDotButton.SetActive(false);
                
                foreach (var button in enhanceDotButtons) button.SetActive(true);
                
                dotDurationCostText.text = $"{_baseDotCost * dotDurationCostMultiplier}";
                dotDamageCostText.text = $"{_baseDotCost * dotDamageCostMultiplier}";
            }
        }
        
        public void DotDurationUp()
        {
            if (CheckCanUpgrade(_baseDotCost, dotDurationCostMultiplier))
            {
                dotDurationCostText.text = $"{_baseDotCost * dotDurationCostMultiplier}";
                EnhanceDotSpell("Lightning Chains",2, 0, 0, 0, statusEffectType.Dot, false);
            }
        }

        public void DotDamagePerTick()
        {
            if (CheckCanUpgrade(_baseDotCost, dotDamageCostMultiplier))
            {
                dotDamageCostText.text = $"{_baseDotCost * dotDamageCostMultiplier}";
                EnhanceDotSpell("Lightning Chains",0, 0, 1, 1, statusEffectType.Dot, false);
            }
        }
        
        private void EnhanceDotSpell(string spellName, float newDuration, float newFrequency, float newMagnitude, float newImpactDamage, statusEffectType newEffect, bool stationary)
        {
            if (_baseDotCost % 3 == 0)
            {
                OnChangeSpellParameters?.Invoke(spellName, null, 1, 0, 0, areaOfEffect.Target);
            }   
            OnEnhanceSpell?.Invoke(spellName, newDuration, newFrequency, newMagnitude, newImpactDamage, newEffect, stationary);
            _baseDotCost++;
        }
    }
}
