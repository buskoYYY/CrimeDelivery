using Doozy.Engine.UI;
using UnityEngine;
using TMPro;

public class GameoverUI : MonoBehaviour
{
    [SerializeField] private UIView gameoverUIView;
    [SerializeField] private TMP_Text reward;
    [SerializeField] private TMP_Text rewardX;
    [SerializeField] private GameoverInfoPanelUI gameoverInfoPanelUI;
    [SerializeField] private Transform gameoverInfoLayout;
    [SerializeField] private int score; // наше текущее количество очков
    [SerializeField] private int points; // количество очков которое мы добавляем за просмотр рекламы

    [SerializeField] private UIButton takeMoney;
    [SerializeField] private UIButton takeXMoney;

    public void GameoverUIStart(GameoverData gameoverData)
    {
        takeMoney.OnClick.OnTrigger.Event.AddListener(TakeMoney);
        takeXMoney.OnClick.OnTrigger.Event.AddListener(TakeXMoney);

        gameoverUIView.Show();

        string rewardType = "Deliveried";
        string rewardCountDelivery = $"{gameoverData.raceData.compleatedDeliveries}/{gameoverData.raceData.maxDeliveries}";
        SetupGameoverInfoPanel(rewardType, rewardCountDelivery, gameoverData.raceData.deliveryReward);
    }
    public void SetupGameoverInfoPanel(string rewardType, string rewardCount, int creditsEarned)
    {
        GameoverInfoPanelUI infoPanel = Instantiate(gameoverInfoPanelUI, gameoverInfoLayout);
        infoPanel.Setup(rewardType, rewardCount, creditsEarned);
    }

    public void TakeMoney()
    {
        Appodeal.Instance.ShowInterstitialAds();
    }

    public void TakeXMoney()
    {
        Appodeal.Instance.ShowRewardAds();
        UpdateScore();
    }

    private void UpdateScore()
    {
        score = Appodeal.Instance.AddScore(points);
        // здесь нужно выводить и сохранять количество очков
    }
}
