using System;
using TMPro;
using UnityEngine;

public class Ads : MonoBehaviour
{
    public static Ads Instance;

    [SerializeField] private TextMeshProUGUI _scoreText;

    private int _score;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("Ads objects > 1");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public bool PlayInterstitialAd()
    {
        return Appodeal.Instance.ShowInterstitialAds();
    }

    public void PlayRewardAd(Action onReward)
    {
        Appodeal.Instance.ShowRewardAds(onReward);
        UpdateScore();
    }

    private void UpdateScore()
    {
        _score = Appodeal.Instance.AddScore(1);

        if(_scoreText)
            _scoreText.text = _score.ToString();
    }
}
