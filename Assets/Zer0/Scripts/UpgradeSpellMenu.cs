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
        private TMP_Text lightningChainscostText;

        [Header("Description Text Fields")]
        [SerializeField, Tooltip("Text field to display the description for the Lightning Chains Spell.")]
        private TMP_Text lightingChainsDescriptionText;

        [Header("Spell Cost Fields")]
        [SerializeField, Tooltip("The cost multiplier for each level of DoT duration enhancement.")]
        private int lightningChainsCostMultiplier = 1;
        [SerializeField, Tooltip("This number will be added to the duration upgrade cost after the multiplier is applied.")]
        private int lightningChainsCostAddition = 1;

        [Header("Spell Value Fields")]
        [SerializeField, Tooltip("The time in seconds each upgrade will increase Lightning Chains duration.")]
        private float lightningChainsDurationValue = 2;
        [SerializeField, Tooltip("The amount each 3rd upgrade will increase Lightning Chains damage.")]
        private int lightningChainsDamageValue = 3;

        [Header("Activate / Deactivate Game Objects")]
        [SerializeField] private GameObject buyDotButton;
        [SerializeField] private GameObject[] enhanceDotButtons;
        
        [SerializeField, Tooltip("A list of all spells that can be purchased by the player")]
        private GameObject[] spellList;
        
        private int _currentPoints;
        private int _totalPoints;
        private int _baseDotCost = 1; 

        public static event Action<SpellData> OnBuyNewSpell;
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
            
            var cost = _baseDotCost * lightningChainsCostMultiplier;
            if (_baseDotCost > 1) cost += lightningChainsCostAddition;
            
            lightningChainscostText.text = $"{cost}";

            lightingChainsDescriptionText.text = $"A struck target is assaulted by chains for {spellList[0].GetComponent<SpellData>().Magnitude} damage every second for " +
                                                 $"{spellList[0].GetComponent<SpellData>().Duration} seconds. " +
                                                 $"Each point of enhancement increase duration by {lightningChainsDurationValue} seconds, and every 3rd " +
                                                 $"enhancements increases damage by {lightningChainsDamageValue} and the spells cost by 1.";
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
            var cost = _baseDotCost * lightningChainsCostMultiplier;
            if (_baseDotCost > 1) cost += lightningChainsCostAddition;
            
            lightningChainscostText.text = $"{cost}";
            
            if (CheckCanUpgrade(_baseDotCost, lightningChainsCostMultiplier, lightningChainsCostAddition))
            {
                OnBuyNewSpell?.Invoke(spellList[0].GetComponent<SpellData>());
                _baseDotCost++;

                buyDotButton.SetActive(false);
                foreach (var button in enhanceDotButtons) button.SetActive(true);
            }
        }
        
        public void LightningChainsUpgrade()
        {
            var cost = _baseDotCost * lightningChainsCostMultiplier;
            if (_baseDotCost > 1) cost += lightningChainsCostAddition;
            
            lightningChainscostText.text = $"{cost}";
            
            if (CheckCanUpgrade(_baseDotCost, lightningChainsCostMultiplier, lightningChainsCostAddition))
            {
                OnEnhanceSpell?.Invoke("Lightning Chains",lightningChainsDurationValue, 0, 0, 0, statusEffectType.Dot, false);
                
                if (_baseDotCost % 3 == 0)
                {
                    OnChangeSpellParameters?.Invoke("Lightning Chains", null, 1, 0, 0, areaOfEffect.Target);
                    OnEnhanceSpell?.Invoke("Lightning Chains",0, 0, lightningChainsDamageValue, lightningChainsDamageValue, statusEffectType.Dot, false);
                }
                
                _baseDotCost++;
            }
            
            var costAfter = _baseDotCost * lightningChainsCostMultiplier;
            if (_baseDotCost > 1) costAfter += lightningChainsCostAddition;
            
            lightningChainscostText.text = $"{costAfter}";
        }
    }
}
