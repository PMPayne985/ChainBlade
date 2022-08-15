using TMPro;
using UnityEngine;

namespace Zer0
{
    public class ScoreUI : MonoBehaviour
    {
        private TMP_Text _scoreText;

        private void Awake()
        {
            _scoreText = GetComponent<TMP_Text>();
        }

        public void SetScore(int score)
        {
            _scoreText.text = $"Score: {score}";
        }
    }
}
