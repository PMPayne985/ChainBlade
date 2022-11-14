using UnityEngine;

namespace Zer0
{
    public class KillQuest : MonoBehaviour
    {
        [SerializeField] private string enemyName;
        [SerializeField] private Quest currentQuest;
        
        [SerializeField] private bool startQuest;
        [SerializeField] private bool endQuest;
        [SerializeField] private bool updateQuest;
        [SerializeField] private int startQuestIndex;
        [SerializeField] private int endQuestIndex;
        [SerializeField] private int updateQuestIndex;
        [SerializeField] private int updateQuestStage;
        [SerializeField] private GameObject[] activateObjects;
        [SerializeField] private GameObject[] deactivateObjects;

        private void Start()
        {
            Enemy.OnDeathQuestUpdate += QuestUpdate;
        }

        private void QuestUpdate(string value)
        {
            if (value != enemyName) return;
            
            currentQuest.questCount++;

            if (currentQuest.questCount >= currentQuest.questCountRequired)
            {
                if (startQuest)
                    FindObjectOfType<QuestLog>().StartQuest(startQuestIndex);
                if (endQuest)
                    FindObjectOfType<QuestLog>().EndQuest(endQuestIndex);
                if (updateQuest)
                    FindObjectOfType<QuestLog>().UpdateQuestStage(updateQuestIndex, updateQuestStage);

                if (activateObjects.Length > 0)
                {
                    foreach (var obj in activateObjects)
                    {
                        obj.SetActive(true);
                    }
                }

                if (deactivateObjects.Length > 0)
                {
                    foreach (var obj in deactivateObjects)
                    {
                        obj.SetActive(false);
                    }
                }
            }
        }
    }
}
