using UnityEngine;

namespace Zer0
{
    public class NPCChat : MonoBehaviour
    {
        [SerializeField] private string[] chatLines;
        [SerializeField] private string postInteractionLine;
        private int _currentChat;
        private Dialogue _dialogue;
        private bool _open;
        private bool _interactedWith;
        private bool _inRange;

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.E) && _inRange)
            {
                if (_open)
                    Advance();
                else
                {
                    _dialogue = FindObjectOfType<Dialogue>();
                    _dialogue.ActivateDialogue(!_interactedWith ? chatLines[0] : postInteractionLine);
                    _open = true;
                }
            }
        }

        private void Advance()
        {
            _currentChat++;
            if (_currentChat > chatLines.Length - 1 || _interactedWith)
            {
                _dialogue.EndDialogue();
                _currentChat = 0;
                _open = false;
                _interactedWith = true;
            }
            else
            {
                _dialogue.AdvanceDialogue(chatLines[_currentChat]);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
                _inRange = true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player))
                _inRange = false;
        }
    }
}
