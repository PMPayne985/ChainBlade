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
        [SerializeField, Tooltip("Text field to display the cost of the next Lightning Chains enhancement.")] 
        private TMP_Text lightningChainscostText;
        [SerializeField, Tooltip("Text field to display the cost of the next Slow spell enhancement.")]
        private TMP_Text slowCostText;

        [Header("Description Text Fields")]
        [SerializeField, Tooltip("Text field to display the description for the Lightning Chains Spell.")]
        private TMP_Text lightingChainsDescriptionText;
        [SerializeField, Tooltip("Text field to display the description for the Slow Spell.")]
        private TMP_Text slowDescriptionText;

        [Header("Spell Cost Fields")]
        [SerializeField, Tooltip("The cost multiplier for each level of Lightning Chains enhancement.")]
        private float lightningChainsCostMultiplier = 1;
        [SerializeField, Tooltip("This number will be added to the Lightning Chains cost after the multiplier is applied.")]
        private int lightningChainsCostAddition = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of the Slow enhancement.")]
        private float slowCostMultiplier = 1;
        [SerializeField, Tooltip("This number will be added to the Slow cost after the multiplier is applied.")]
        private int slowCostAddition = 1;

        [Header("Spell Value Fields")]
        [SerializeField, Tooltip("The time in seconds each upgrade will increase Lightning Chains duration.")]
        private float lightningChainsDurationValue = 2;
        [SerializeField, Tooltip("The amount each 3rd upgrade will increase Lightning Chains damage.")]
        private int lightningChainsDamageValue = 3;
        [SerializeField, Tooltip("The time in seconds each upgrade will increase Slow duration.")]
        private float slowDurationValue = 1;
        [SerializeField, Tooltip("The amount each 3rd upgrade will decrease the speed of a slowed target.")]
        private float slowMagnitudeValue = 0.5f;
        

        [Header("Activate / Deactivate Game Objects")]
        [SerializeField, Tooltip("Buttons that will be deactivated once the Lightning Chains spell is purchased.")] 
        private GameObject[] buyLightningChainsButtons;
        [SerializeField, Tooltip("Buttons that will be activated once the spell Lightning Chains is purchased.")] 
        private GameObject[] enhanceLightningChainsButtons;
        [SerializeField, Tooltip("Buttons that will be deactivated once the Slow spell is purchased.")] 
        private GameObject[] buySlowButtons;
        [SerializeField, Tooltip("Buttons that will be activated once the Slow spell is purchased.")] 
        private GameObject[] enhanceSlowButtons;
        
        [SerializeField, Tooltip("A list of all spells that can be purchased by the player")]
        private GameObject[] spellList;
        
        private int _currentPoints;
        private int _totalPoints;
        private int _baseLightningChainsCost = 1;
        private int _baseSlowCost = 1;

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
            #region Lightning Chains
            currentPointsText.text = $"{_currentPoints}";
            totalPointsText.text = $"{_totalPoints}";
            
            var lcCost = Mathf.RoundToInt(_baseLightningChainsCost * lightningChainsCostMultiplier);
            lcCost += lightningChainsCostAddition;

            lightningChainscostText.text = $"{lcCost}";

            lightingChainsDescriptionText.text = $"A struck target is assaulted by chains for {spellList[0].GetComponent<SpellData>().Magnitude} damage every second for " +
                                                 $"{spellList[0].GetComponent<SpellData>().Duration} seconds. " +
                                                 $"Each point of enhancement increase duration by {lightningChainsDurationValue} seconds. " +
                                                 $"Every 3rd enhancements increases damage by {lightningChainsDamageValue} and the spells cost by 1.";
#endregion

            #region Slow

            var sCost = Mathf.RoundToInt(_baseSlowCost * slowCostMultiplier);
            sCost += slowCostAddition;

            slowCostText.text = $"{sCost}";

            slowDescriptionText.text = $"The Target's move and attack speed is reduced by {spellList[1].GetComponent<SpellData>().Magnitude * 100}%" +
                                       $" for {spellList[1].GetComponent<SpellData>().Duration} seconds" + 
                                       $"Each point of enhancement increase duration by {slowDurationValue} seconds. " +
                                       $"Every 3rd enhancements increase the speed penalty by {slowMagnitudeValue * 100}% and the spells cost by 1.";

#endregion
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
        
        #region Lightning Chains
        
        public void BuyLightningChainsSpell()
        {
            var cost = Mathf.RoundToInt(_baseLightningChainsCost * lightningChainsCostMultiplier);
            cost += lightningChainsCostAddition;
            
            lightningChainscostText.text = $"{cost}";
            
            if (CheckCanUpgrade(_baseLightningChainsCost, lightningChainsCostMultiplier, lightningChainsCostAddition))
            {
                OnBuyNewSpell?.Invoke(spellList[0].GetComponent<SpellData>());
                _baseLightningChainsCost++;

                foreach (var button in buyLightningChainsButtons) button.SetActive(false);
                foreach (var button in enhanceLightningChainsButtons) button.SetActive(true);
            }
        }

        public void LightningChainsUpgrade()
        {
            var cost = Mathf.RoundToInt(_baseLightningChainsCost * lightningChainsCostMultiplier);
            cost += lightningChainsCostAddition;
            
            lightningChainscostText.text = $"{cost}";
            
            if (CheckCanUpgrade(_baseLightningChainsCost, lightningChainsCostMultiplier, lightningChainsCostAddition))
            {
                OnEnhanceSpell?.Invoke("Lightning Chains",lightningChainsDurationValue, 0, 0, 0, statusEffectType.Dot, false);
                
                if (_baseLightningChainsCost % 3 == 0)
                {
                    OnChangeSpellParameters?.Invoke("Lightning Chains", null, 1, 0, 0, areaOfEffect.Target);
                    OnEnhanceSpell?.Invoke("Lightning Chains",0, 0, lightningChainsDamageValue, lightningChainsDamageValue, statusEffectType.Dot, false);
                }
                
                _baseLightningChainsCost++;
            }
            
            var costAfter = _baseLightningChainsCost * lightningChainsCostMultiplier;
            costAfter += lightningChainsCostAddition;
            
            lightningChainscostText.text = $"{costAfter}";
        }
        
        #endregion

        #region Slow

        public void BuySlowSpell()
        {
            var sCost = Mathf.RoundToInt(_baseSlowCost * slowCostMultiplier);
            sCost += slowCostAddition;

            slowCostText.text = $"{sCost}";
            
            if (CheckCanUpgrade(_baseSlowCost, slowCostMultiplier, slowCostAddition))
            {
                OnBuyNewSpell?.Invoke(spellList[1].GetComponent<SpellData>());
                _baseSlowCost++;

                foreach (var button in buySlowButtons) button.SetActive(false);
                foreach (var button in enhanceSlowButtons) button.SetActive(true);
            }
        }
        
        public void SlowUpgrade()
        {
            var sCost = Mathf.RoundToInt(_baseSlowCost * slowCostMultiplier);
            sCost += slowCostAddition;

            slowCostText.text = $"{sCost}";
            
            if (CheckCanUpgrade(_baseSlowCost, slowCostMultiplier, slowCostAddition))
            {
                OnEnhanceSpell?.Invoke("Slow",slowDurationValue, 0, 0, 0, statusEffectType.Dot, false);
                
                if (_baseLightningChainsCost % 3 == 0)
                {
                    OnChangeSpellParameters?.Invoke("Slow", null, 1, 0, 0, areaOfEffect.Target);
                    OnEnhanceSpell?.Invoke("Slow",0, 0, slowMagnitudeValue, 0, statusEffectType.Dot, false);
                }
                
                _baseSlowCost++;
            }

            var costAfter = Mathf.RoundToInt(_baseSlowCost * slowCostMultiplier);
            costAfter += slowCostAddition;

            slowCostText.text = $"{costAfter}";
        }

        #endregion
    }
}
