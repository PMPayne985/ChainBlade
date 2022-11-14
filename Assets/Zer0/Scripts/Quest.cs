using UnityEngine;

namespace Zer0
{
    [CreateAssetMenu]
    public class Quest : ScriptableObject
    {
        public int questStage;
        [HideInInspector] public int questCount;
        public int questCountRequired;
        [TextArea(2, 100)] public string[] questStageDescriptions;
    }
}
