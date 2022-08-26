using UnityEngine;

namespace Zer0
{
    public class Spell
    {
        public string SpellName { get; private set; }
        public Sprite Icon { get; private set; }
        public int Cost { get; private set; }
        public float CoolDown { get; private set; }
        public float Range { get; private set; }
        public int spellIndex { get; private set; }
        public float Duration { get; private set; }
        public float Frequency { get; private set; }
        public float Magnitude { get; private set; }
        
        public statusEffectType EffectToAdd { get; private set; }

        public Spell(string newName, Sprite newIcon, int newCost, float newCoolDown, float newRange, int newIndex, 
            float newDuration, float newFrequency, float newMagnitude, statusEffectType newEffect)
            { SpellName = newName; Icon = newIcon; Cost = newCost; CoolDown = newCoolDown; Range = newRange; 
                spellIndex = newIndex; Duration = newDuration; Frequency = newFrequency; Magnitude = newMagnitude; EffectToAdd = newEffect; }

    }
}
