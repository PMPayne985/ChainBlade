using System;
using UnityEngine;

namespace Zer0
{
    public class PopUpDialogue : MonoBehaviour
    {
        [SerializeField, TextArea(4, 100)] private string popUpText;
        [SerializeField] private bool deactivateAfterUse;
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
