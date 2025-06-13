using Doozy.Engine.UI;
using SickscoreGames.HUDNavigationSystem;
using System.Collections.Generic;
using UnityEngine;
public class MainMenuLogic : MonoBehaviour
{
    public GameData gameData;

    public CarComponentsController playerCarPrefab;
    private CarComponentsController playerCar;
    public NewCameraController playerCamera;

    public RaceLogic raceLogic;
    
    public List<Transform> spawnPointsList = new List<Transform>();
    private Transform currentSpawnPoint;

    public UIView mainMenuUI;
    public UIView raceUI;

    public UIButton startRaceButton;

    public CarUIInfo carUIInfo;
    public PlayerUIController playerUIController;
    private void Awake()
    {
        startRaceButton.OnClick.OnTrigger.Event.AddListener(StartRace);
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        gameData = FindFirstObjectByType<GameData>();
        currentSpawnPoint = spawnPointsList[Random.Range(0, spawnPointsList.Count)];
        SpawnPlayer(playerCarPrefab, currentSpawnPoint);

    }

    public void StartRace()
    {
        raceLogic.Initialize(gameData);
        raceLogic.StartRace(playerCar);

        
        playerUIController.Initialize(playerCar);
        playerCar.StartRace();

        mainMenuUI.Hide();
        raceUI.Show();
        carUIInfo.Initialize(playerCar);
    }

    public void SpawnPlayer(CarComponentsController playerCarPrefab, Transform spawnPoint)
    {
        Quaternion rotation = Quaternion.Euler(0, spawnPoint.eulerAngles.y, 0);
        playerCar = Instantiate(playerCarPrefab, new Vector3(spawnPoint.position.x, spawnPoint.position.y + 2, spawnPoint.position.z), rotation);
        playerCar.carGameobject.AddComponent<HNSPlayerController>();
        playerCamera.Initialize(playerCar);
    }

    public void ChangePlayerCar()
    {
        Destroy(playerCar.gameObject);
        SpawnPlayer(playerCarPrefab, currentSpawnPoint);
    }
}
