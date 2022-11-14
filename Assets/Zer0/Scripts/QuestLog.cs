using System;
using UnityEngine;
using UnityEngine.UI;

namespace Zer0
{
    public class QuestLog : MonoBehaviour
    {
        [SerializeField] private int levelIndex;
        [SerializeField] private GameObject journalPanel;
        [SerializeField] private Text[] journalBoxes;
        [SerializeField] private Quest[] quests;

        private bool _isOpen;

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.J))
                OpenAndClose();
        }

        public void OpenAndClose()
        {
            if (_isOpen)
            {
                journalPanel.SetActive(false);
                _isOpen = false;
            }
            else
            {
                journalPanel.SetActive(true);
                UpdateAllQuests();
                _isOpen = true;
            }
        }

        public void StartQuest(int index)
        {
            journalBoxes[index].gameObject.SetActive(true);
            UpdateQuestDescription(index);
            quests[index].questStage = 0;
            quests[index].questCountRequired = 0;
        }

        public void EndQuest(int index)
        {
            journalBoxes[index].gameObject.SetActive(false);
        }
        
        public void UpdateQuestStage(int index, int value)
        {
            quests[index].questStage = value;
            UpdateQuestDescription(index);
        }

        private void UpdateQuestDescription(int index)
        {
            journalBoxes[index].text = quests[index].questStageDescriptions[quests[index].questStage];
        }

        private void UpdateAllQuests()
        {
            for (int i = 0; i < quests.Length; i++)
            {
                journalBoxes[i].text = quests[i].questStageDescriptions[quests[i].questStage];
            }
        }
    }
}
