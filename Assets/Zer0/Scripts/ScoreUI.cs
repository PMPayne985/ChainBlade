using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Zer0
{
    public class ScoreUI : MonoBehaviour
    {
        private TMP_Text _scoreText;
        public List<GameObject> _links = new List<GameObject>();

        private void Awake()
        {
            _scoreText = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            Collection.Instance.OnCollectedLink += StoreCollectedLink;

            var startingLinks = FindObjectsOfType<Collectible>();

            for (var i = 0; i < startingLinks.Length; i++)
            {
                _links.Add(startingLinks[i].gameObject);
                _links[i].SetActive(false);
            }
            
            ResetCollectedLink();
        }

        public void SetScore(int score)
        {
            _scoreText.text = $"Score: {score}";
            if (score % 5 == 0)
                ResetCollectedLink();
        }

        public void StoreCollectedLink(GameObject link)
        {
            var inList = false;
            foreach (var l in _links)
            {
                if (l == link)
                    inList = true;
            }
            
            if (!inList)
                _links.Add(link);
        }

        private void ResetCollectedLink()
        {
            if (_links.Count > 0)
                foreach (var link in _links)
                {
                    if (!link.activeInHierarchy)
                    {
                        link.SetActive(true);
                        _links.Remove(link);
                        break;
                    }
                }
        }
    }
}
