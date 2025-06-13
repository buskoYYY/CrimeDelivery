using TMPro;
using UnityEngine;

namespace ArcadeBridge
{
    public class Ads : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;

        private int _score;

       
        public void PlayRewardAd()
        {
            Appodeal.Instance.ShowRewardAds();
            UpdateScore();
        }

        private void UpdateScore()
        {
            _score = Appodeal.Instance.AddScore(1);
            _scoreText.text = _score.ToString();
        }
    }
}
