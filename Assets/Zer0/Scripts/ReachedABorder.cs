using System;
using UnityEngine;

namespace Zer0
{
    public class ReachedABorder : MonoBehaviour
    {
        [SerializeField] private string boundsText;
        private Dialogue _dialogue;
        private bool _open;

        private void Update()
        {
            if (_open)
            {
                if (Input.GetKeyUp(KeyCode.Return))
                {
                    _dialogue.EndDialogue();
                    _open = false;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                _dialogue = FindObjectOfType<Dialogue>();
                _dialogue.ActivateDialogue(boundsText);
                _open = true;
            }
        }
    }
}
