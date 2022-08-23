using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Zer0
{
    public class ScoreUI : MonoBehaviour
    {
        private TMP_Text _scoreText;
        private List<GameObject> _links;

        private void Awake()
        {
            _scoreText = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            LinkCollectible.OnCollectedLink += StoreCollectedLink;

            _links = new List<GameObject>();
            var startingLinks = FindObjectsOfType<Collectible>();

            for (var i = 0; i < startingLinks.Length; i++)
            {
                _links.Add(startingLinks[i].gameObject);
                _links[i].SetActive(false);
            }
            
            ResetCollectedLink();
            
            Enemy.ResetScore();
        }

        public void SetScore(int score)
        {
            _scoreText.text = $"Kills: {score}";
            var testScore = score % 5;
            if (testScore == 0)
                ResetCollectedLink();
        }

        private void StoreCollectedLink(Collectible link)
        {
            var inList = false;
            foreach (var l in _links)
            {
                if (l == link.gameObject)
                    inList = true;
            }
 
            if (!inList)
                _links.Add(link.gameObject);
        }

        private void ResetCollectedLink()
        {
            if (_links.Count <= 0) return;

            var random = UnityEngine.Random.Range(0, _links.Count);
            
            for (var i = 0; i < _links.Count; i++)
            {
                if (i != random & _links[i].activeInHierarchy) continue;
                
                _links[i].SetActive(true);
                _links.Remove(_links[i]);
                break;
            }
        }
    }
}
