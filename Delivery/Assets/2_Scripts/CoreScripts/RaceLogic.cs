using UnityEngine;

[System.Serializable]
public class RaceData
{
    public int deliveryReward;
    public int maxDeliveries;
    public int compleatedDeliveries;
    public CompleteType completeType;

    public enum CompleteType
    {
        FINISHED, DESTROYED
    }
}

public class RaceLogic : MonoBehaviour
{
    public GameData gameData;
    public int difficultyIndex;

    public RaceData raceData;

    public DeliveryController deliveryController;

    public delegate void OnRaceCompleted(RaceData raceData);
    public event OnRaceCompleted OnRaceCompletedEvent;

    public delegate void OnRaceStarted(RaceData raceData);
    public event OnRaceStarted OnRaceStartedEvent;

    public GameoverController gameoverController;

    public PoliceSpawner policeSpawner;
    private CarComponentsController playerCar;

    [SerializeField] private bool initAtStart;

    private void Start()
    {
        if (initAtStart)
            Initialize(gameData);
    }

    public void Initialize(GameData gameData)
    {
        this.gameData = gameData;
    }

    public void StartRace(CarComponentsController playerCar)
    {
        OnRaceStartedEvent?.Invoke(raceData);

        this.playerCar = playerCar;
        deliveryController.Initialize(playerCar);
        deliveryController.OnDeliveredEvent += AddReward;
        raceData.maxDeliveries = deliveryController.deliveryTargets.Count / 2;
        
        deliveryController.OnDeliveredAllEvent += EndLevel;
        playerCar.carDamageHandler.OnEndLivesEvent += EndLevel;

        policeSpawner.player = playerCar.carTrasform;
        policeSpawner.spawnPointsOnPlayer = playerCar.GetComponent<PoliceSpawnPointsObject>();
        
        int difficultyCheck = difficultyIndex;
        if (difficultyCheck >= gameData.difficultyDatabase.difficultyConfigs.Length || difficultyCheck < 0)
        {
            Debug.LogError($"��� ��������� {difficultyCheck}");
            difficultyCheck = gameData.difficultyDatabase.difficultyConfigs.Length - 1;
        }
        policeSpawner.Initialize(this, gameData.difficultyDatabase.difficultyConfigs[difficultyCheck]);

        
    }

    private void OnDisable()
    {
        deliveryController.OnDeliveredEvent -= AddReward;
        deliveryController.OnDeliveredAllEvent -= EndLevel;
        playerCar.carDamageHandler.OnEndLivesEvent -= EndLevel;
    }

    public void AddReward(int value)
    {
        if (value > 0)
        {
            raceData.compleatedDeliveries++;
            raceData.deliveryReward += value;
        }

    }

    public void EndLevel(CarComponentsController car, RaceData.CompleteType completeType)
    {
        car.carDamageHandler.damageble = false;

        foreach (CarComponent carComponent in car.carComponents)
        {
            Driver driver = carComponent as Driver;
            if (driver != null)
            {
                driver.FinishRace();
            }
        }
        raceData.completeType = completeType;

        OnRaceCompletedEvent?.Invoke(raceData);
        gameoverController.Gameover(raceData);
    }
}