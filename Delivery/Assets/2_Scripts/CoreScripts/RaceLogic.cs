using UnityEngine;

public class RaceData
{
    public int reward;
    public int maxDeliveries;
    public int compleatedDeliveries;
}

public class RaceLogic : MonoBehaviour
{
    public RaceData raceData;

    public DeliveryController deliveryController;

    public void Initialize()
    {
        deliveryController.OnDeliveredAllEvent += EndLevel;
    }

    public void AddReward(int value)
    {
        raceData.reward += value;
    }

    public void EndLevel()
    {
        
    }
}