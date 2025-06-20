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

    private void Awake()
    {
        raceLogic = FindFirstObjectByType<RaceLogic>();
    }

    public void Initialize(CarComponentsController player)
    {
        this.player = player;
        this.player.carDamageHandler.OnUpdateHealthEvent += UpdateHelath;
        
    }
    public void UpdateHelath(float health, float maxHealth)
    {
        carSlider.fillAmount = health / maxHealth;
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
            catchSlider.fillAmount = raceLogic.currentCathTime / raceLogic.catchTimer;
        }

    }
}
