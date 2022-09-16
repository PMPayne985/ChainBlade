using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISetUp : MonoBehaviour
{
    [SerializeField, Tooltip("The text field where the name of the active spell is displayed.")]
    private TMP_Text spellNameText;
    [SerializeField, Tooltip("The image field where the icon for the active spell is displayed.")]
    private Image spellIconImage;
    [SerializeField, Tooltip("The text field where the current spell cooldown is displayed.")]
    private TMP_Text spellCooldownText;
    [SerializeField, Tooltip("")] 
    private Slider magicSlider;
    
    public void SetCurrentSpellInfo(string newName, Sprite newIcon)
    {
        spellNameText.text = newName;
        spellIconImage.sprite = newIcon;
    }

    public void SetSpellCoolDown(string coolDown)
    {
        spellCooldownText.text = coolDown;
    }

    public void SetSliderMax(float value)
    {
        magicSlider.maxValue = value;
    }
    
    public void UpdateMagicSlider(float maxSpellPoints, float spellPoints)
    {
        if (maxSpellPoints != magicSlider.maxValue)
        {
            magicSlider.maxValue = Mathf.Lerp(magicSlider.maxValue, maxSpellPoints, 2f * Time.fixedDeltaTime);
            magicSlider.onValueChanged.Invoke(magicSlider.value);
        }
        magicSlider.value = spellPoints;
    }
}
