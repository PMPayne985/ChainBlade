using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISetUp : MonoBehaviour
{
    [SerializeField, Tooltip("The text field where health is displayed.")]
    private TMP_Text healthText;

    [SerializeField, Tooltip("The text field where spell points are displayed.")]
    private TMP_Text spText;

    [SerializeField, Tooltip("The text field where the name of the active spell is displayed.")]
    private TMP_Text spellNameText;
    [SerializeField, Tooltip("The image field where the icon for the active spell is displayed.")]
    private Image spellIconImage;
    [SerializeField, Tooltip("The text field where the current spell cooldown is displayed.")]
    private TMP_Text spellCooldownText;

    public void SetHealthDisplay(float newHealth, float maxHealth)
    {
        healthText.text = $"{newHealth} / {maxHealth}";
    }

    public void SetSpellPointDisplay(float newSp, float maxSP)
    {
        spText.text = $"{newSp:#} / {maxSP}";
    }

    public void SetCurrentSpellInfo(string newName, Sprite newIcon)
    {
        spellNameText.text = newName;
        spellIconImage.sprite = newIcon;
    }

    public void SetSpellCoolDown(string coolDown)
    {
        spellCooldownText.text = coolDown;
    }
}
