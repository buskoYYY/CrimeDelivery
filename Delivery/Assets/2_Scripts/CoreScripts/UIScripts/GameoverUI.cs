using Doozy.Engine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameoverUI : MonoBehaviour
{
    [SerializeField] private UIView gameoverUIView;
    [SerializeField] private TMP_Text reward;
    [SerializeField] private TMP_Text rewardX;
    [SerializeField] private GameoverInfoPanelUI gameoverInfoPanelUI;
    [SerializeField] private Transform gameoverInfoLayout;

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
        SceneManager.LoadScene(0);
    }

    public void TakeXMoney()
    {
        SceneManager.LoadScene(0);
    }
}
