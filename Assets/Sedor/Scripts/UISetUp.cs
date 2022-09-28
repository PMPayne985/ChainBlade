using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zer0;

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
    [SerializeField, Tooltip("")] 
    private Image[] effectImages;
    [SerializeField]
    private float[] effectTimers;

    private void Start()
    {
        StatusEffects.OnAddStatusEffect += SetEffectImage;
    }

    private void Update()
    {
        RemoveEffectImage();
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

    public void SetSliderMax(float value)
    {
        magicSlider.maxValue = value;
    }

    private void SetEffectImage(bool isPlayer, Image newEffect, float duration)
    {
        if (isPlayer)
        {
            for (int i =0; i < effectImages.Length; i++)
            {
                if (!effectImages[i].sprite)
                {
                    effectImages[i].sprite = newEffect.sprite;
                    effectTimers[i] = duration;
                    return;
                }
            }
        }
    }

    private void RemoveEffectImage()
    {
        for (int i = 0; i < effectTimers.Length; i++)
        {
            if (effectImages[i].sprite)
            {
                effectTimers[i] -= Time.deltaTime;
                if (effectTimers[i] <= 0)
                    effectImages[i].sprite = null;
            }
        }
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
