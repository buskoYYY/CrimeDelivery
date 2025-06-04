using UnityEngine;
using UnityEngine.UI;

public class CarUIInfo : MonoBehaviour
{
    public CarComponentsController player;
    public Image carSlider;

    public void Initialize(CarComponentsController player)
    {
        this.player = player;
        this.player.carDamageHandler.OnUpdateHealthEvent += UpdateHelath;
    }
    public void UpdateHelath(float health, float maxHealth)
    {
        carSlider.fillAmount = health / maxHealth;
    }
}
