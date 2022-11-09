using UnityEngine;
using UnityEngine.UI;

namespace Zer0
{
    public class Dialogue : MonoBehaviour
    {
        [SerializeField] private GameObject dialogueWindow;
        [SerializeField] private Text dialogueText;


        public void ActivateDialogue(string line)
        {
            dialogueWindow.SetActive(true);
            Time.timeScale = 0;
            dialogueText.text = line;
        }

        public void AdvanceDialogue(string nextLine)
        {
            dialogueText.text = nextLine;
        }

        public void EndDialogue()
        {
            dialogueText.text = "";
            Time.timeScale = 1;
            dialogueWindow.SetActive(false);
        }
    }
}
