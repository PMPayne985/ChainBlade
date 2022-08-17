using System;
using TMPro;
using UnityEngine;

public class UISetUp : MonoBehaviour
{
    [SerializeField, Tooltip("The text field where health is displayed.")]
    private TMP_Text healthText;

    public void UpdateHealthUI(float newHealth, float maxHealth)
    {
        healthText.text = $"{newHealth} / {maxHealth}";
    }
}
