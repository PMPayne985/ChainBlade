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
        private TMP_Text lightningChainsCostText;
        [SerializeField, Tooltip("Text field to display the cost of the next Slow spell enhancement.")]
        private TMP_Text shacklesCostText;
        [SerializeField, Tooltip("Text field to display the cost of the next Shackles enhancement.")]
        private TMP_Text disarmCostText;
        [SerializeField, Tooltip("Text field to display the cost of the next Refresh enhancement.")]
        private TMP_Text refreshCostText;
        [SerializeField, Tooltip("Text field to display the cost of the next Iron Web spell enhancement.")]
        private TMP_Text ironWebCostText;
        [SerializeField, Tooltip("Text field to display the cost of the next spell point enhancement.")]
        private TMP_Text spellPointCostText;

        [Header("Description Text Fields")]
        [SerializeField, Tooltip("Text field to display the description for the Lightning Chains Spell.")]
        private TMP_Text lightingChainsDescriptionText;
        [SerializeField, Tooltip("Text field to display the description for the Slow Spell.")]
        private TMP_Text shacklesDescriptionText;
        [SerializeField, Tooltip("Text field to display the description for the Shackles Spell.")]
        private TMP_Text disarmDescriptionText;
        [SerializeField, Tooltip("Text field to display the description for the Refresh Spell.")]
        private TMP_Text refreshDescriptionText;
        [SerializeField, Tooltip("Text field to display the description for the Iron Web Spell.")]
        private TMP_Text ironWebDescriptionText;
        [SerializeField, Tooltip("Text field to display the current maximum spell points for the player.")]
        private TMP_Text spellPointsMaxText;

        [Header("Spell Cost Fields")]
        [SerializeField, Tooltip("The cost multiplier for each level of Lightning Chains enhancement.")]
        private float lightningChainsCostMultiplier = 1;
        [SerializeField, Tooltip("This number will be added to the Lightning Chains cost after the multiplier is applied.")]
        private int lightningChainsCostAddition = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of the Shackles enhancement.")]
        private float shacklesCostMultiplier = 1;
        [SerializeField, Tooltip("This number will be added to the Shackles cost after the multiplier is applied.")]
        private int shacklesCostAddition = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of the Disarm enhancement.")]
        private float disarmCostMultiplier = 1;
        [SerializeField, Tooltip("This number will be added to the Refresh cost after the multiplier is applied.")]
        private int disarmCostAddition = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of the Refresh enhancement.")]
        private float refreshCostMultiplier = 1;
        [SerializeField, Tooltip("This number will be added to the Refresh cost after the multiplier is applied.")]
        private int refreshCostAddition = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of the Iron Web enhancement.")]
        private float ironWebCostMultiplier = 1;
        [SerializeField, Tooltip("This number will be added to the Iron Web cost after the multiplier is applied.")]
        private int ironWebCostAddition = 1;
        [SerializeField, Tooltip("The cost multiplier for each level of the spell point enhancement.")]
        private float spellPointsMultiplier = 1;
        [SerializeField, Tooltip("This number will be added to the spell point cost after the multiplier is applied.")]
        private int spellPointsCostAddition = 1;
        
        [Header("Spell Value Fields")]
        [SerializeField, Tooltip("The time in seconds each upgrade will increase Lightning Chains duration.")]
        private float lightningChainsDurationValue = 2;
        [SerializeField, Tooltip("The amount each 3rd upgrade will increase Lightning Chains damage.")]
        private int lightningChainsDamageValue = 3;
        [SerializeField, Tooltip("The time in seconds each upgrade will increase Slow duration.")]
        private float shacklesDurationValue = 1;
        [SerializeField, Tooltip("The amount each 3rd upgrade will decrease the speed of a slowed target.")]
        private float shacklesMagnitudeValue = 0.5f;
        [SerializeField, Tooltip("The time in seconds each upgrade will increase the Disarm Spell's duration.")]
        private float disarmDurationValue;
        [SerializeField, Tooltip("The time in seconds each upgrade will increase Refreshes duration.")]
        private float refreshDurationValue = 2;
        [SerializeField, Tooltip("The amount each 3rd upgrade will increase Refreshes healing value.")]
        private int refreshHealingValue = 3;
        [SerializeField, Tooltip("The time in seconds each upgrade will increase Iron Webs duration.")]
        private int ironWebDuration = 1;
        [SerializeField, Tooltip("The percent of damage the Iron Web spell reduces each attack by.")]
        private float ironWebProtectionRate = 0.05f;
        [SerializeField, Tooltip("The amount to add to max spell points for each enhancement.")]
        private float pointsToAdd = 2;
        

        [Header("Activate / Deactivate Game Objects")]
        [SerializeField, Tooltip("Buttons that will be deactivated once the Lightning Chains spell is purchased.")] 
        private GameObject[] buyLightningChainsButtons;
        [SerializeField, Tooltip("Buttons that will be activated once the spell Lightning Chains is purchased.")] 
        private GameObject[] enhanceLightningChainsButtons;
        [SerializeField, Tooltip("Buttons that will be deactivated once the Slow spell is purchased.")] 
        private GameObject[] buyShacklesButtons;
        [SerializeField, Tooltip("Buttons that will be activated once the Slow spell is purchased.")] 
        private GameObject[] enhanceShacklesButtons;
        [SerializeField, Tooltip("Buttons that will be deactivated once the Disarm spell is purchased.")] 
        private GameObject[] buyDisarmButtons;
        [SerializeField, Tooltip("Buttons that will be activated once the Disarm spell is purchased.")] 
        private GameObject[] enhanceDisarmButtons;
        [SerializeField, Tooltip("Buttons that will be deactivated once the Refresh spell is purchased.")] 
        private GameObject[] buyRefreshButtons;
        [SerializeField, Tooltip("Buttons that will be activated once the Refresh spell is purchased.")] 
        private GameObject[] enhanceRefreshButtons;
        [SerializeField, Tooltip("Buttons that will be deactivated once the Iron Web spell is purchased.")] 
        private GameObject[] buyIronWebButtons;
        [SerializeField, Tooltip("Buttons that will be activated once the Iron Web spell is purchased.")] 
        private GameObject[] enhanceIronWebButtons;
        
        [Header("Spells")]
        [SerializeField, Tooltip("A list of all spells that can be purchased by the player")]
        private GameObject[] spellList;
        
        private int _currentPoints;
        private int _totalPoints;
        private int _baseLightningChainsCost = 1;
        private int _baseShacklesCost = 1;
        private int _baseDisarmCost = 1;
        private int _baseRefreshCost = 1;
        private int _baseIronWebCost = 1;
        private int _baseSpellPointCost = 1;

        public static event Action<SpellData> OnBuyNewSpell;
        public static event Action<string, float, float, float, int, statusEffectType, bool> OnEnhanceSpell;
        public static event Action<string, Sprite, int, float, float, areaOfEffect> OnChangeSpellParameters;
        public static event Action<float> OnAddSpellPoints;

        private void Start()
        {
            UpgradeTopMenu.OnSpentLink += IncrementCollected;
            gameObject.SetActive(false);
        }
        
        public void OnOpen()
        {

            DescriptionText();
            
            var spCost = Mathf.RoundToInt(_baseSpellPointCost * spellPointsMultiplier);
            spCost += spellPointsCostAddition;

            spellPointCostText.text = $"{spCost}";

            spellPointsMaxText.text = $"Max SP: {(10 - pointsToAdd) + (_baseSpellPointCost * pointsToAdd)}";
        }

        private void DescriptionText()
        {
            #region Lightning Chains
            currentPointsText.text = $"{_currentPoints}";
            totalPointsText.text = $"{_totalPoints}";
            
            var lcCost = Mathf.RoundToInt(_baseLightningChainsCost * lightningChainsCostMultiplier);
            lcCost += lightningChainsCostAddition;

            lightningChainsCostText.text = $"{lcCost}";

            lightingChainsDescriptionText.text = $"A struck target is assaulted by chains for {spellList[0].GetComponent<SpellData>().Magnitude} damage every second for " +
                                                 $"{spellList[0].GetComponent<SpellData>().Duration} seconds. " +
                                                 $"Each point of enhancement increases duration by {lightningChainsDurationValue} seconds. " +
                                                 $"Every 3rd enhancement increases damage by {lightningChainsDamageValue} and the spell's cost by 1.";
            #endregion

            #region Shackles

            var sCost = Mathf.RoundToInt(_baseShacklesCost * shacklesCostMultiplier);
            sCost += shacklesCostAddition;

            shacklesCostText.text = $"{sCost}";

            shacklesDescriptionText.text = $"The Target's move and attack speed are reduced by {spellList[1].GetComponent<SpellData>().Magnitude * 100}% " +
                                       $"for {spellList[1].GetComponent<SpellData>().Duration} seconds " + 
                                       $"Each point of enhancement increases duration by {shacklesDurationValue} seconds. " +
                                       $"Every 3rd enhancement increases the speed penalty by {shacklesMagnitudeValue * 100}% and the spell's cost by 1.";

            #endregion
            
            #region Disarm

            var dCost = Mathf.RoundToInt(_baseDisarmCost * disarmCostMultiplier);
            dCost += disarmCostAddition;

            disarmCostText.text = $"{dCost}";

            disarmDescriptionText.text = $"The target's weapon is bound making it unable to deal damage " +
                                           $"for {spellList[2].GetComponent<SpellData>().Duration} seconds " + 
                                           $"Each point of enhancement increases duration by {disarmDurationValue} seconds. " +
                                           $"Every 3rd enhancement increases the spell's cost by 1.";

            #endregion
            
            #region Refresh

            var rCost = Mathf.RoundToInt(_baseRefreshCost * refreshCostMultiplier);
            rCost += refreshCostAddition;

            refreshCostText.text = $"{rCost}";

            var rData = spellList[3].GetComponent<SpellData>();
            refreshDescriptionText.text =$"Heal yourself {rData.Duration * rData.Magnitude} Health Points " +
                                         $"over {rData.Duration} seconds. " + 
                                         $"Each point of enhancement increases duration by {refreshDurationValue} seconds. " +
                                         $"Every 3rd enhancement increases the healing done per second by {refreshHealingValue} and the spells cost by 1.";

            #endregion
            
            #region Iron Web

            var iCost = Mathf.RoundToInt(_baseIronWebCost * ironWebCostMultiplier);
            iCost += ironWebCostAddition;

            ironWebCostText.text = $"{iCost}";

            var iData = spellList[4].GetComponent<SpellData>();
            ironWebDescriptionText.text =$"Surround yourself in a protection field reducing damage you receive by {iData.Magnitude * 100}% " +
                                         $"for {iData.Duration} seconds. " + 
                                         $"Each point of enhancement increases duration by {ironWebDuration} seconds. " +
                                         $"Every 3rd enhancement increases the damage reduction by {ironWebProtectionRate * 100}% to a maximum of 90% and the spell's cost by 1.";

            #endregion
        }
        
        private void IncrementCollected(UpgradeMenu check, int amount)
        {
            if (check != this) return;

            _currentPoints += amount;
            _totalPoints += amount;
        }
        
        
        #region Lightning Chains
        
        public void BuyLightningChainsSpell()
        {
            DescriptionText();
            
            if (CheckCanUpgrade(_baseLightningChainsCost, lightningChainsCostMultiplier, lightningChainsCostAddition))
            {
                OnBuyNewSpell?.Invoke(spellList[0].GetComponent<SpellData>());
                _baseLightningChainsCost++;

                foreach (var button in buyLightningChainsButtons) button.SetActive(false);
                foreach (var button in enhanceLightningChainsButtons) button.SetActive(true);
            }
            
            DescriptionText();
        }

        public void LightningChainsUpgrade()
        {
            DescriptionText();
            
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
            
            DescriptionText();
        }
        
        #endregion

        #region Shackles

        public void BuyShacklesSpell()
        {
            DescriptionText();
            
            if (CheckCanUpgrade(_baseShacklesCost, shacklesCostMultiplier, shacklesCostAddition))
            {
                OnBuyNewSpell?.Invoke(spellList[1].GetComponent<SpellData>());
                _baseShacklesCost++;

                foreach (var button in buyShacklesButtons) button.SetActive(false);
                foreach (var button in enhanceShacklesButtons) button.SetActive(true);
            }
            
            DescriptionText();
        }
        
        public void ShacklesUpgrade()
        {
            DescriptionText();
            
            if (CheckCanUpgrade(_baseShacklesCost, shacklesCostMultiplier, shacklesCostAddition))
            {
                OnEnhanceSpell?.Invoke("Shackles",shacklesDurationValue, 0, 0, 0, statusEffectType.Slow, false);
                
                if (_baseShacklesCost % 3 == 0)
                {
                    OnChangeSpellParameters?.Invoke("Shackles", null, 1, 0, 0, areaOfEffect.Target);
                    OnEnhanceSpell?.Invoke("Shackles",0, 0, shacklesMagnitudeValue, 0, statusEffectType.Slow, false);
                }
                
                _baseShacklesCost++;
            }

            DescriptionText();
        }

        #endregion
        
        #region Disarm

        public void BuyDisarmSpell()
        {
            DescriptionText();
            
            if (CheckCanUpgrade(_baseDisarmCost, disarmCostMultiplier, disarmCostAddition))
            {
                OnBuyNewSpell?.Invoke(spellList[2].GetComponent<SpellData>());
                _baseDisarmCost++;

                foreach (var button in buyDisarmButtons) button.SetActive(false);
                foreach (var button in enhanceDisarmButtons) button.SetActive(true);
            }
            
            DescriptionText();
        }
        
        public void DisarmUpgrade()
        {
            DescriptionText();
            
            if (CheckCanUpgrade(_baseDisarmCost, disarmCostMultiplier, disarmCostAddition))
            {
                OnEnhanceSpell?.Invoke("Disarm",disarmDurationValue, 0, 0, 0, statusEffectType.Disarm, false);
                
                if (_baseDisarmCost % 3 == 0)
                {
                    OnChangeSpellParameters?.Invoke("Disarm", null, 1, 0, 0, areaOfEffect.Target);
                }
                
                _baseDisarmCost++;
            }

            DescriptionText();
        }

        #endregion
        
        #region Refresh

        public void BuyRefreshSpell()
        {
            DescriptionText();
            
            if (CheckCanUpgrade(_baseRefreshCost, refreshCostMultiplier, refreshCostAddition))
            {
                OnBuyNewSpell?.Invoke(spellList[3].GetComponent<SpellData>());
                _baseRefreshCost++;

                foreach (var button in buyRefreshButtons) button.SetActive(false);
                foreach (var button in enhanceRefreshButtons) button.SetActive(true);
            }
            
            DescriptionText();
        }
        
        public void RefreshUpgrade()
        {
            DescriptionText();
            
            if (CheckCanUpgrade(_baseRefreshCost, refreshCostMultiplier, refreshCostAddition))
            {
                OnEnhanceSpell?.Invoke("Refresh",refreshDurationValue, 0, 0, 0, statusEffectType.Hot, false);
                
                if (_baseLightningChainsCost % 3 == 0)
                {
                    OnChangeSpellParameters?.Invoke("Refresh", null, 1, 0, 0, areaOfEffect.None);
                    OnEnhanceSpell?.Invoke("Refresh",0, 0, refreshHealingValue, 0, statusEffectType.Hot, false);
                }
                
                _baseRefreshCost++;
            }

            DescriptionText();;
        }

        #endregion
        
        #region Iron Web

        public void BuyIronWebSpell()
        {
            DescriptionText();
            
            if (CheckCanUpgrade(_baseIronWebCost, ironWebCostMultiplier, ironWebCostAddition))
            {
                OnBuyNewSpell?.Invoke(spellList[4].GetComponent<SpellData>());
                _baseIronWebCost++;

                foreach (var button in buyIronWebButtons) button.SetActive(false);
                foreach (var button in enhanceIronWebButtons) button.SetActive(true);
            }
            
            DescriptionText();
        }
        
        public void IronWebUpgrade()
        {
            DescriptionText();
            
            if (CheckCanUpgrade(_baseIronWebCost, ironWebCostMultiplier, ironWebCostAddition))
            {
                OnEnhanceSpell?.Invoke("Iron Web",ironWebDuration, 0, 0, 0, statusEffectType.Protect, false);
                
                if (_baseLightningChainsCost % 3 == 0)
                {
                    OnChangeSpellParameters?.Invoke("Iron Web", null, 1, 0, 0, areaOfEffect.None);
                    OnEnhanceSpell?.Invoke("Iron Web",0, 0, ironWebProtectionRate, 0, statusEffectType.Protect, false);
                }
                
                _baseIronWebCost++;
            }

            DescriptionText();
        }

        #endregion

        public void IncreaseSpellPoints()
        {
            var iCost = Mathf.RoundToInt(_baseSpellPointCost * spellPointsMultiplier);
            iCost += spellPointsCostAddition;

            spellPointCostText.text = $"{iCost}";
            spellPointsMaxText.text = $"Max SP: {(10 - pointsToAdd) + (_baseSpellPointCost * pointsToAdd)}";
            
            if (CheckCanUpgrade(_baseSpellPointCost, spellPointsMultiplier, spellPointsCostAddition))
            {
                OnAddSpellPoints?.Invoke(pointsToAdd);
                _baseSpellPointCost++;
            }
            
            iCost = Mathf.RoundToInt(_baseSpellPointCost * spellPointsMultiplier);
            iCost += spellPointsCostAddition;

            spellPointCostText.text = $"{iCost}";
            spellPointsMaxText.text = $"Max SP: {(10 - pointsToAdd) + (_baseSpellPointCost * pointsToAdd)}";
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
