using TMPro;
using UnityEngine;

namespace Zer0
{
    public class Message : MonoBehaviour
    {
        [SerializeField, Tooltip("The message to be displayed.")]
        private string _message;

        [SerializeField, Tooltip("The text field for displaying this message.")]
        private TMP_Text textField;
        
        public void DisplayMessage(string message)
        {
            _message = message;

            textField.text = _message;
        }
    }
}
