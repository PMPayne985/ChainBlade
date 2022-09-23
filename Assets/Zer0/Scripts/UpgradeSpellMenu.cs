using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zer0
{
    public class UpgradeSpellMenu : UpgradeMenu
    {
        [Header("Point Display Text Fields")]
        [SerializeField, Tooltip("Text field to display the current number of points available.")] 
        private TMP_Text currentPointsText;
        [SerializeField, Tooltip("Text field to display the total number of points invested.")]
        private TMP_Text totalPointsText;
        [SerializeField, Tooltip("Text field to display the cost of the next DoT duration enhancement.")] 
        private TMP_Text dotDurationCostText;
        [SerializeField, Tooltip("Text field to display the cost of the next DoT damage enhancement.")] 
        private TMP_Text dotDamageCostText;

        [Header("Description Text Fields")]
        [SerializeField, Tooltip("Text field to display the description for the Lightning Chains Spell.")]
        private TMP_Text lightingChainsDescriptionText;

        [Header("Spell Cost Fields")]
        [SerializeField, Tooltip("The cost multiplier for each level of DoT duration enhancement.")]
        private int dotDurationCostMultiplier = 1;
        [SerializeField, Tooltip("This number will be added to the duration upgrade cost after the multiplier is applied.")]
        private int dotDurationCostAddition = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of DoT damage enhancement.")]
        private int dotDamageCostMultiplier = 1;
        [SerializeField, Tooltip("This number will be added to the damage upgrade cost after the multiplier is applied.")]
        private int dotDamageCostAddition = 1;

        [Header("Spell Value Fields")]
        [SerializeField, Tooltip("The time in seconds each upgrade will increase Lightning Chains duration.")]
        private float lightningChainsDurationValue = 2;
        [SerializeField, Tooltip("The amount each upgrade will increase Lightning Chains damage.")]
        private int lightningChainsDamageValue = 3;

        [Header("Activate / Deactivate Game Objects")]
        [SerializeField] private GameObject buyDotButton;
        [SerializeField] private GameObject[] enhanceDotButtons;
        
        [SerializeField, Tooltip("A list of all spells that can be purchased by the player")]
        private GameObject[] spellList;
        
        private int _currentPoints;
        private int _totalPoints;
        private int _baseDotCost = 1; 

        public static event Action<Spell> OnBuyNewSpell;
        public static event Action<string, float, float, float, int, statusEffectType, bool> OnEnhanceSpell;
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
            
            var durationCost = _baseDotCost * dotDurationCostMultiplier;
            var damageCost = _baseDotCost * dotDamageCostMultiplier;
            if (_baseDotCost > 1)
            {
                durationCost += dotDurationCostAddition;
                damageCost += dotDamageCostAddition;
            }
            dotDurationCostText.text = $"{durationCost}";
            dotDamageCostText.text = $"{damageCost}";

            lightingChainsDescriptionText.text = $"A struck target is assaulted by chains for {lightningChainsDamageValue * 2} damage every 2 seconds for " +
                                                 $"{lightningChainsDurationValue * 2} seconds. " +
                                                 $"Each point of duration enhancement increase duration by {lightningChainsDurationValue} seconds, and each " +
                                                 $"point of damage enhancement increase damage by {lightningChainsDamageValue}.";
        }
        
        private void IncrementCollected(UpgradeMenu check, int amount)
        {
            if (check != this) return;

            _currentPoints += amount;
            _totalPoints += amount;
        }
        
        private bool CheckCanUpgrade(int upgradeCost, int upgradeMultiplier, int upgradeAddition)
        {
            var upgradeFinalCost = upgradeCost * upgradeMultiplier;
            if (upgradeCost > 1) upgradeFinalCost += upgradeAddition;
            
            if (_currentPoints >= upgradeFinalCost)
            {
                _currentPoints -= upgradeFinalCost;
                currentPointsText.text = $"{_currentPoints}";
                return true;
            }
            return false;
        }

        public void BuyDotSpell()
        {
            if (CheckCanUpgrade(_baseDotCost, dotDurationCostMultiplier, dotDamageCostAddition))
            {
                OnBuyNewSpell?.Invoke(spellList[0].GetComponent<Spell>());
                _baseDotCost++;

                buyDotButton.SetActive(false);
                EnhanceDotSpell("Lightning Chains",lightningChainsDurationValue * 2, 0, lightningChainsDamageValue * 2, lightningChainsDamageValue * 2, statusEffectType.Dot, false);
                foreach (var button in enhanceDotButtons) button.SetActive(true);
                var durationCost = _baseDotCost * dotDurationCostMultiplier;
                var damageCost = _baseDotCost * dotDamageCostMultiplier;
                if (_baseDotCost > 1)
                {
                    durationCost += dotDurationCostAddition;
                    damageCost += dotDamageCostAddition;
                }
                dotDurationCostText.text = $"{durationCost}";
                dotDamageCostText.text = $"{damageCost}";
            }
        }
        
        public void DotDurationUp()
        {
            if (CheckCanUpgrade(_baseDotCost, dotDurationCostMultiplier, dotDurationCostAddition))
            {
                var durationCost = _baseDotCost * dotDurationCostMultiplier;
                var damageCost = _baseDotCost * dotDamageCostMultiplier;
                if (_baseDotCost > 1)
                {
                    durationCost += dotDurationCostAddition;
                    damageCost += dotDamageCostAddition;
                }
                dotDurationCostText.text = $"{durationCost}";
                dotDamageCostText.text = $"{damageCost}";
                EnhanceDotSpell("Lightning Chains",lightningChainsDurationValue, 0, 0, 0, statusEffectType.Dot, false);
            }
        }

        public void DotDamagePerTick()
        {
            if (CheckCanUpgrade(_baseDotCost, dotDamageCostMultiplier, dotDamageCostAddition))
            {
                var durationCost = _baseDotCost * dotDurationCostMultiplier;
                var damageCost = _baseDotCost * dotDamageCostMultiplier;
                if (_baseDotCost > 1)
                {
                    durationCost += dotDurationCostAddition;
                    damageCost += dotDamageCostAddition;
                }
                dotDurationCostText.text = $"{durationCost}";
                dotDamageCostText.text = $"{damageCost}";
                EnhanceDotSpell("Lightning Chains",0, 0, lightningChainsDamageValue, lightningChainsDamageValue, statusEffectType.Dot, false);
            }
        }
        
        private void EnhanceDotSpell(string spellName, float newDuration, float newFrequency, float newMagnitude, int newImpactDamage, statusEffectType newEffect, bool stationary)
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
