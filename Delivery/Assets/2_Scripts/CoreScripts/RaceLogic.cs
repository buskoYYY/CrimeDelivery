using UnityEngine;

[System.Serializable]
public class RaceData
{
    public int deliveryReward;
    public int maxDeliveries;
    public int compleatedDeliveries;
}

public class RaceLogic : MonoBehaviour
{
    public RaceData raceData;

    public DeliveryController deliveryController;

    public delegate void OnRaceCompleted(RaceData raceData);
    public event OnRaceCompleted OnRaceCompletedEvent;

    public GameoverController gameoverController;

    public PoliceSpawner policeSpawner;
    //private CarComponentsController playerCar;

    public void Initialize(CarComponentsController playerCar)
    {
        deliveryController.Initialize(playerCar);
        deliveryController.OnDeliveredAllEvent += EndLevel;
        deliveryController.OnDeliveredEvent += AddReward;
        raceData.maxDeliveries = deliveryController.deliveryTargets.Count / 2;
        playerCar.carDamageHandler.OnEndLivesEvent += EndLevel;

        policeSpawner.player = playerCar.carTrasform;
        policeSpawner.spawnPointsOnPlayer = playerCar.GetComponent<PoliceSpawnPointsObject>();
        policeSpawner.Initialize();
    }

    public void AddReward(int value)
    {
        if (value > 0)
        {
            raceData.compleatedDeliveries++;
            raceData.deliveryReward += value;
        }

    }

    public void EndLevel(CarComponentsController car)
    {
        OnRaceCompletedEvent?.Invoke(raceData);
        gameoverController.Gameover(raceData);
    }
}