using UnityEngine;

namespace Zer0
{
    [CreateAssetMenu]
    public class Quest : ScriptableObject
    {
        public int questStage;
        [TextArea(2, 100)] public string[] questStageDescriptions;
    }
}
