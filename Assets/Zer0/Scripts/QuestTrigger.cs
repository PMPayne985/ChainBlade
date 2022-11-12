using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zer0
{
    public class QuestTrigger : MonoBehaviour
    {
        [SerializeField] private bool startQuest;
        [SerializeField] private bool endQuest;
        [SerializeField] private bool updateQuest;
        [SerializeField] private int startQuestIndex;
        [SerializeField] private int endQuestIndex;
        [SerializeField] private int updateQuestIndex;
        [SerializeField] private int updateQuestStage;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Player player)) return;
            
            
            if (startQuest)
                FindObjectOfType<QuestLog>().StartQuest(startQuestIndex);
            if (endQuest)
                FindObjectOfType<QuestLog>().EndQuest(endQuestIndex);
            if (updateQuest)
                FindObjectOfType<QuestLog>().UpdateQuestStage(updateQuestIndex, updateQuestStage);
        }
    }
}
