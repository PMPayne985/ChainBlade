using System;
using UnityEngine;

namespace Zer0
{
    public class PopUpDialogue : MonoBehaviour
    {
        [SerializeField, TextArea(4, 100)] private string popUpText;
        [SerializeField] private bool deactivateAfterUse;
        [SerializeField] private bool startQuest;
        [SerializeField] private bool endQuest;
        [SerializeField] private bool updateQuest;
        [SerializeField] private int startQuestIndex;
        [SerializeField] private int endQuestIndex;
        [SerializeField] private int updateQuestIndex;
        [SerializeField] private int updateQuestStage;
        private Dialogue _dialogue;
        private bool _open;

        private void Update()
        {
            if (_open)
            {
                if (Input.GetKeyUp(KeyCode.E))
                {
                    _dialogue.EndDialogue();
                    _open = false;
                    if (deactivateAfterUse) gameObject.SetActive(false);

                    if (startQuest)
                        FindObjectOfType<QuestLog>().StartQuest(startQuestIndex);
                    if (endQuest)
                        FindObjectOfType<QuestLog>().EndQuest(endQuestIndex);
                    if (updateQuest)
                        FindObjectOfType<QuestLog>().UpdateQuestStage(updateQuestIndex, updateQuestStage);
                    
                    startQuest = false;
                    endQuest = false;
                    updateQuest = false;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                _dialogue = FindObjectOfType<Dialogue>();
                _dialogue.ActivateDialogue(popUpText);
                _open = true;
            }
        }
    }
}
