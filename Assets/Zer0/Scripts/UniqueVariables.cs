using UnityEngine;

namespace Zer0
{
    public class UniqueVariables : MonoBehaviour
    {

    }
    
    public enum weaponType
    {
        chainEnd,
        knifeBlade
    };
    
    public enum statusEffectType
    {
        None,
        Dot,
        Stun,
        Slow,
        Disarm,
        Protect,
        Hot
    };

    public enum areaOfEffect
    {
        None,
        Target,
        Small,
        Medium,
        Large
    }
}
