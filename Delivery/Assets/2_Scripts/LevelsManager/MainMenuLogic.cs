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

    public UIButton startRaceButton;


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
        currentSpawnPoint = spawnPointsList[Random.Range(0, spawnPointsList.Count)];
        SpawnPlayer(playerCarPrefab, currentSpawnPoint);
    }

    public void StartRace()
    {
        policeSpawner = FindFirstObjectByType<PoliceSpawner>();
        policeSpawner.player = playerCar.carTrasform;
        policeSpawner.spawnPointsOnPlayer = playerCar.GetComponent<PoliceSpawnPointsObject>();
        policeSpawner.Initialize();
        mainMenuUI.Hide();
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
