using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarUIInfo : MonoBehaviour
{
    public CarComponentsController player;
    public Image carSlider;

    public TMP_Text deliviried;
    public int maxDeliviries = 0;
    public int compleatedDeliviries = 0;

    private int reward;

    private RaceLogic raceLogic;

    public void Initialize(CarComponentsController player, RaceLogic raceLogic)
    {
        this.raceLogic = raceLogic;
        maxDeliviries = this.raceLogic.deliveryController.deliveryTargets.Count / 2;
        this.raceLogic.deliveryController.OnDeliveredEvent += OnDelivery;
        deliviried.text = $"{compleatedDeliviries}/{maxDeliviries}"; 
        this.player = player;
        this.player.carDamageHandler.OnUpdateHealthEvent += UpdateHelath;
    }

    public void OnDelivery(int reward)
    {
        compleatedDeliviries = (int)Mathf.Clamp((raceLogic.deliveryController.activeTarget + 1) / 2, 0, Mathf.Infinity);
        deliviried.text = $"{compleatedDeliviries}/{maxDeliviries}";
    }

    public void UpdateHelath(float health, float maxHealth)
    {
        carSlider.fillAmount = health / maxHealth;
    }
}
