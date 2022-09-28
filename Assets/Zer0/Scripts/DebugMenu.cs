using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Zer0
{
    public class DebugMenu : MonoBehaviour
    {
        public static DebugMenu Instance;

        [SerializeField, Tooltip("Check this to have debug options during play.")]
        private bool debugOn;
        [SerializeField, Tooltip("The panel that holds the debug info")]
        private GameObject debugPanel;
        [SerializeField, Tooltip("The input field for typing in debug commands.")]
        private TMP_InputField debugInputField;
        [SerializeField, Tooltip("The max number of debug messages that will display before removing old messages.")]
        private int maxMessages;
        [SerializeField, Tooltip("The message prefab.")]
        private GameObject messagePrefab;
        [SerializeField, Tooltip("the reference point for spawning new messages.")]
        private Transform referencePoint;
        [SerializeField, Tooltip("The distance each message is moved up when a new message is added.")]
        private float messageYOffset = 100;

        private List<GameObject> messages;

        private bool _menuOn;
        public bool MenuOn => _menuOn;

        public static event Action<int> OnRefillHealthCommand;
        public static event Action<int> OnAddLinksCommand;
        public static event Action<float> OnRefillSpellPointsCommand;
        public static event Action <int, Transform> OnDamageSelfCommand;

        private void Awake()
        {
            if (Instance)
                Destroy(gameObject);
            else
                Instance = this;
        }

        private void Start()
        {
            messages = new List<GameObject>();
        }

        private void Update()
        {
            if (PlayerInput.Debug())
                ToggleDebugWindow();
            
            if (_menuOn)
                SendMessage();
        }

        private void CheckAllCommands(string command)
        {
            switch (command)
            {
                case "refillhealth":
                    RefillHealthCommand(1000);
                    break;
                case "refillspellpoints":
                    RefillSpellPointsCommand(1000);
                    break;
                case "addlinks":
                    AddOneHundredLinksCommands();
                    break;
                case "damageself":
                    DamageSelfCommand(10);
                    break;
                default:
                    DebugCommand("Command not recognized.");
                    break;
            }
        }
        
        private void RefillHealthCommand(int health)
        {
            OnRefillHealthCommand?.Invoke(health);
            DebugCommand($"{health} health restored.");
        }

        private void DamageSelfCommand(int damage)
        {
            OnDamageSelfCommand?.Invoke(damage, transform);
            DebugCommand($"{damage} to self");
        }
        
        private void RefillSpellPointsCommand(int points)
        {
            OnRefillSpellPointsCommand?.Invoke(points);
            DebugCommand($"{points} spell points restored.");
        }
        
        private void AddOneHundredLinksCommands()
        {
            OnAddLinksCommand?.Invoke(100);
            DebugCommand($"100 Links added.");
        }

        public void DebugCommand(string command)
        {
            var position = referencePoint.position;
            if (messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    message.transform.position += new Vector3(0, messageYOffset, 0);
                }
            }

            if (messages.Count >= maxMessages) messages.Remove(messages[0]);
            
            var newCommand = Instantiate(messagePrefab, position, transform.rotation, debugPanel.transform);
            newCommand.GetComponent<Message>().DisplayMessage(command);
            
            messages.Add(newCommand);
        }

        private void ToggleDebugWindow()
        {
            if (PauseMenu.Paused) return;
            if (ChainUpgrade.EnhancementOpen) return;
            
            if (debugOn)
                _menuOn = !_menuOn;

            debugPanel.SetActive(_menuOn);

            if (_menuOn)
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void SendMessage()
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                if (debugInputField.text == string.Empty)
                {
                    debugInputField.ActivateInputField();
                }
                else
                {
                    DebugCommand(debugInputField.text);
                    CheckAllCommands(debugInputField.text);
                    debugInputField.text = string.Empty;
                }
            }
        }
    }
}
