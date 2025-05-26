using UnityEngine;
using UnityEngine.UI;

public class CarUIInfo : MonoBehaviour
{
    public CarComponentsController player;
    public Image carSlider;

    private void Start()
    {
        player.carDamageHandler.OnUpdateHealthEvent += UpdateHelath;
    }
    public void UpdateHelath(float health, float maxHealth)
    {
        carSlider.fillAmount = health / maxHealth;
    }
}
