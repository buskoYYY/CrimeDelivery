using Doozy.Engine.UI;
using SickscoreGames.HUDNavigationSystem;
using System.Collections.Generic;
using UnityEngine;
public class MainMenuLogic : MonoBehaviour
{
    public CarComponentsController playerCarPrefab;
    public CarComponentsController playerCar;
    public NewCameraController playerCamera;
    public PoliceSpawner policeSpawner;
    public List<Transform> spawnPointsList = new List<Transform>();
    public Transform currentSpawnPoint;

    public UIView mainMenuUI;
    public UIView raceUI;

    public UIButton startRaceButton;

    public CarUIInfo carUIInfo;
    public PlayerUIController playerUIController;
    private void Awake()
    {
        startRaceButton.OnClick.OnTrigger.Event.AddListener(StartRace);
        carUIInfo = GetComponent<CarUIInfo>();
        playerUIController = GetComponent<PlayerUIController>();
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        currentSpawnPoint = spawnPointsList[Random.Range(0, spawnPointsList.Count)];
        SpawnPlayer(playerCarPrefab, currentSpawnPoint);
        
        carUIInfo.Initialize(playerCar);
        playerUIController.Initialize(playerCar);
    }

    public void StartRace()
    {
        policeSpawner = FindFirstObjectByType<PoliceSpawner>();
        policeSpawner.player = playerCar.carTrasform;
        policeSpawner.spawnPointsOnPlayer = playerCar.GetComponent<PoliceSpawnPointsObject>();
        policeSpawner.Initialize();
        mainMenuUI.Hide();
        raceUI.Show();
        playerCar.StartRace();
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
