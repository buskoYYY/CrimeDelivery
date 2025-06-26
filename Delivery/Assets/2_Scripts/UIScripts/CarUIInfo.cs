using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Doozy.Engine.UI;

public class CarUIInfo : MonoBehaviour
{
    public CarComponentsController player;
    public Image carSlider;

    private RaceLogic raceLogic;

    [SerializeField] private UIView cathingView;
    public Image catchSlider;

    [SerializeField] private GameObject[] policeStars;

    [SerializeField] private TMP_Text deliviries;
    [SerializeField] private TMP_Text deliviriesReward;

    private void Awake()
    {
        raceLogic = FindFirstObjectByType<RaceLogic>();
        raceLogic.deliveryController.OnDeliveredEvent += UpdateStars;
        raceLogic.deliveryController.OnDeliveredEvent += UpdateDeliviriesText;

    }



    public void Initialize(CarComponentsController player)
    {
        this.player = player;
        this.player.carDamageHandler.OnUpdateHealthEvent += UpdateHelath;
        UpdateStars(0);
        UpdateDeliviriesText(0);
    }

    public void UpdateDeliviriesText(int reward)
    {
        deliviries.text = $"Delivered {raceLogic.deliveryController.activeTarget + 1}/{raceLogic.deliveryController.deliveryCount}";
        deliviriesReward.text = $"Reward {raceLogic.deliveryController.summReward}"; 
    }

    public void UpdateStars(int reward)
    {
        int policeStarsCount = Mathf.Min(raceLogic.deliveryController.activeTarget + 2, policeStars.Length);
        foreach (GameObject star in policeStars)
            star.SetActive(false);

        for (int i = 0; i < policeStarsCount; i++)
        {
            policeStars[i].SetActive(true);
        }
    }
    public void UpdateHelath(float health, float maxHealth)
    {
        carSlider.fillAmount = health / maxHealth;
    }

    private void OnDisable()
    {
        raceLogic.deliveryController.OnDeliveredEvent -= UpdateStars;
        player.carDamageHandler.OnUpdateHealthEvent -= UpdateHelath;
        raceLogic.deliveryController.OnDeliveredEvent -= UpdateDeliviriesText;
    }

    private void FixedUpdate()
    {
        if (raceLogic.raceStarted && !raceLogic.raceComleated)
            CatchLogic();
        else if (raceLogic.raceComleated && cathingView.IsVisible)
            cathingView.Hide();
    }

    public void CatchLogic()
    {
        if (raceLogic.catchStarted && !cathingView.IsVisible)
            cathingView.Show();
        else if (!raceLogic.catchStarted && cathingView.IsVisible)
            cathingView.Hide();

        if (raceLogic.catchStarted)
        {
            catchSlider.fillAmount = (raceLogic.currentCathTime - raceLogic.startCatchTimer) / (raceLogic.catchTimer - raceLogic.startCatchTimer);
        }

    }
}
