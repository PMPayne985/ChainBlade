using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zer0
{
    public class SceneChanger : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        [SerializeField] private int linkedIndex;

        private void OnTriggerEnter(Collider other)
        {
            if (TryGetComponent(out Player player))
            {
                SceneManager.LoadScene(sceneName);
                SavedStats.Instance.linkedIndex = linkedIndex;
                var saveAll = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();

                foreach (var saveable in saveAll)
                {
                    saveable.SaveData();
                }
            }
        }
    }
}
