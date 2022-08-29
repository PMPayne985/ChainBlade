using TMPro;
using UnityEngine;

namespace Zer0
{
    public class UpgradeSpellMenu : UpgradeMenu
    {
        [SerializeField, Tooltip("Text field to display the current number of points available.")] 
        private TMP_Text currentPointsText;
        [SerializeField, Tooltip("Text field to display the total number of points invested.")]
        private TMP_Text totalPointsText;
        
        private int _currentPoints;
        private int _totalPoints;
        
        private void Start()
        {
            UpgradeTopMenu.OnSpentLink += IncrementCollected;
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
    }
}
