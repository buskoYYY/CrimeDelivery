using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class DifficultyConfig
{
    public CarAIConfig carAIDifficulty;
    public float maxPoliceHealth;
    
    public float damage;
    
    public int maxPoliceCount = 10;
    public int policeToSpawnCount = 5;
    public int spawnDelay = 3;

    public void CopySettingTo(DifficultyConfig config)
    {
        config.carAIDifficulty = carAIDifficulty;
        config.maxPoliceHealth = maxPoliceHealth;
        config.damage = damage;
        config.maxPoliceCount = maxPoliceCount;
        config.policeToSpawnCount = policeToSpawnCount;
        config.spawnDelay = spawnDelay;
    }

}

[System.Serializable]
public class CarAIConfig
{
    public float accelChangerMin = 5;
    public float accelChangerMax = 10;
    public float rotVelChanger = 0.5f;
    public float targetOffcetMin = 5;
    public float targetOffcetMax = 15;
    public float targetOffcetFarFromPlayer = 1;
}
     

public class PoliceSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public DifficultyConfig difficultyConfigLocal;
    public float maxSpawnDistance = 70;
    public float minSpawnDistance = 10;
    //public float policeHealth = 50;

    public Transform player;
    public PoliceSpawnPointsObject spawnPointsOnPlayer;
         

    public List<CarComponentsController> policeList = new List<CarComponentsController>();
    public CarComponentsController[] policePrefabs;
    //public int policeToSpawnCount = 5;
    public float spawnSpeed = 20;
    //public int maxPoliceCount = 20;
    //public int spawnDelay = 3;
    public float destroyDistance = 70;

    //public float maxPoliceHealth = 100;

    //public Camera playerCamera;



    public Vector3 spawnSize = new Vector3(1, 1, 1); // Размер области проверки
    public LayerMask collisionMask;
    //Ограничение по спавну

    [SerializeField] private bool initAtStart;

    private RaceLogic raceLogic;

    private void Start()
    {
        if (initAtStart)
            Initialize(raceLogic, difficultyConfigLocal);
    }

    public void Initialize(RaceLogic raceLogic, DifficultyConfig difficultyConfig)
    {
        difficultyConfig.CopySettingTo(difficultyConfigLocal);

        //StartCoroutine(SpawnPoliceCoorutine());
        this.raceLogic = raceLogic;
        this.raceLogic.OnRaceCompletedEvent += OnEndOFRace;
        spawnActive = true;
        StartCoroutine(SpawnCoorutine());
    }

    //СПАВН ПО СПАВН ПОИНТАМ НА УРОВНЕ
    /*
    private IEnumerator SpawnPoliceCoorutine()
    {
        while (gameObject.activeSelf == true)
        {
            yield return new WaitForSeconds(spawnDelay);
            SpawnPolice();
        }

    }
    */

    /*
public void SpawnPolice()
{
    List<(Transform point, float distanceToTarget)> nearbyPoints = new ();

    foreach (Transform point in spawnPoints)
    {
        if (nearbyPoints.Count > 20)
            break;

        float distance = Vector3.Distance(player.position, point.position);
        if (distance >= minSpawnDistance && distance <= maxSpawnDistance && !IsVisibleFromCamera(point.position, playerCamera))
        {
            nearbyPoints.Add((point, distance));
        }
    }

    nearbyPoints.Sort((a, b) => a.distanceToTarget.CompareTo(b.distanceToTarget));

    int count = Mathf.Min(policeToSpawnCount, nearbyPoints.Count);
    for (int i = 0; i < count; i++)
    {
        if (policeList.Count >= maxPoliceCount)
            break;

        CarComponentsController policeInstanse = Instantiate(policePrefabs[0], new Vector3(nearbyPoints[i].point.position.x, nearbyPoints[i].point.position.y + 2, nearbyPoints[i].point.position.z) , nearbyPoints[i].point.rotation);

        Vector3 bottom = GetLowestPoint(policeInstanse.carGameobject);

        // Считаем смещение от центра до низа
        float offsetY = bottom.y - policeInstanse.carTrasform.position.y;

        Vector3 directionToTarget = player.position - nearbyPoints[i].point.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        policeInstanse.carTrasform.rotation = lookRotation;

        Driver driverPolice;
        foreach (CarComponent driver in policeInstanse.carComponents)
        {
            driverPolice = driver as Driver;
            if (driverPolice != null)
            {

                driverPolice.Throttle(1);
                driverPolice.ChangeTarget(player);
            }

            AIDriftController ai = driver as AIDriftController;

            if (ai != null)
            {
                ai.autoDestroy = true;
            }


        }
        policeInstanse.StartRace();
        policeInstanse.carDamageHandler.OnEndLivesEvent += OnEndOfLivesCar;
        policeInstanse.carDamageHandler.ChangeMaxHealth(policeHealth); 
        policeList.Add(policeInstanse);
    }
}
*/

    private bool spawnActive;
    private IEnumerator SpawnCoorutine()
    {
        while (spawnActive)
        {
            yield return new WaitForSeconds(difficultyConfigLocal.spawnDelay);

            if (policeList.Count < difficultyConfigLocal.maxPoliceCount)
            {
                int spawnCount = Mathf.Min(spawnPointsOnPlayer.spawnRays.Count, difficultyConfigLocal.policeToSpawnCount);
                Shuffle(spawnPointsOnPlayer.spawnRays);
                for (int i = 0; i < spawnCount; i++)
                {
                    TrySpawn(spawnPointsOnPlayer.spawnRays[i]);
                }
            }

            if (!spawnActive)
                break;
        }

        if (raceData.completeType == RaceData.CompleteType.FINISHED)
        {
            for (int i = 0; i < policeList.Count; i++)
            {
                policeList[i].carDamageHandler.ApplyDamage(999999);
            }
        }
    }

    public void TrySpawn(PoliceSpawnPointsRay policeRay)
    {
        // 1. Проверка наличия поверхности под точкой спавна
        if (!Physics.Raycast(policeRay.spawnPosition.position, Vector3.down, out RaycastHit groundHit, 10f, collisionMask))
        {
            Debug.Log("Не удалось заспавнить: нет поверхности под точкой.");
            return;
        }

        Vector3 finalSpawnPosition = groundHit.point;

        // 2. Проверка препятствий по пути BoxCast
        Vector3 direction = (policeRay.spawnPosition.position - policeRay.startPosition.position).normalized;
        float distance = Vector3.Distance(policeRay.startPosition.position, finalSpawnPosition);
        bool hit = Physics.BoxCast(policeRay.startPosition.position, spawnSize * 0.5f, direction, out RaycastHit hitInfo, policeRay.spawnPosition.rotation, distance, collisionMask);

        if (!hit)
        {
            Quaternion rotation = Quaternion.Euler(0, policeRay.spawnPosition.eulerAngles.y, 0);
            CarComponentsController policeInstanse = Instantiate(policePrefabs[0], new Vector3(finalSpawnPosition.x, finalSpawnPosition.y + 2, finalSpawnPosition.z), rotation);
            SetupPolice(policeInstanse);
        }
        else
        {
            Debug.Log("Не удалось заспавнить: путь к точке занят.");
        }
    }
    private void SetupPolice(CarComponentsController policeInstanse)
    {
        Driver driverPolice;
        foreach (CarComponent driver in policeInstanse.carComponents)
        {
            driverPolice = driver as Driver;
            if (driverPolice != null)
            {
                driverPolice.ChangeTarget(player);
                driverPolice.Throttle(1);
            }


            policeInstanse.carDamageHandler.Initialize(false, 1, difficultyConfigLocal.maxPoliceHealth);

            AIDriftController ai = driver as AIDriftController;
            if (ai != null)
            {
                ai.distanceToDestroy = destroyDistance;
                ai.autoDestroy = true;
                ai.SetupCarAIConfig(difficultyConfigLocal.carAIDifficulty);
            }

            CarPusher carPusher = driver as CarPusher;
            if (carPusher != null)
            {
                
            }

            policeInstanse.StartRace();

        }

        StartCoroutine(AddForceToPolice(policeInstanse));
        policeInstanse.carDamageHandler.OnEndLivesEvent += OnEndOfLivesCar;
        policeList.Add(policeInstanse);
    }

    public float forceTime = 1;
    private IEnumerator AddForceToPolice(CarComponentsController policeInstanse)
    {
        float time = 0;
        while (time < forceTime)
        {
            yield return new WaitForSeconds(0.01f);
            time += Time.deltaTime;
            policeInstanse.carRigidbody.AddForce(policeInstanse.carTrasform.forward * spawnSpeed, ForceMode.Acceleration);
        }
    }

    private void OnDisable()
    {
        this.raceLogic.OnRaceCompletedEvent -= OnEndOFRace;
        foreach (CarComponentsController police in policeList)
            police.carDamageHandler.OnEndLivesEvent -= OnEndOfLivesCar;
    }

    public void OnEndOfLivesCar(CarComponentsController car, RaceData.CompleteType completeType)
    {
        policeList.Remove(car);
        car.carDamageHandler.OnDestroyCarEvent -= OnEndOfLivesCar;
    }

    public void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int k = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[k];
            list[k] = temp;
        }
    }

    // Метод, чтобы найти нижнюю точку по коллайдерам
    public Vector3 GetLowestPoint(GameObject obj)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();

        if (colliders.Length == 0)
            return obj.transform.position;

        float minY = float.MaxValue;
        Vector3 lowestPoint = Vector3.zero;

        foreach (var col in colliders)
        {
            Vector3 bottom = col.bounds.center - new Vector3(0, col.bounds.extents.y, 0);
            if (bottom.y < minY)
            {
                minY = bottom.y;
                lowestPoint = bottom;
            }
        }

        return lowestPoint;
    }

    public bool IsVisibleFromCamera(Vector3 position, Camera cam)
    {
        Vector3 viewportPoint = cam.WorldToViewportPoint(position);

        // Объект перед камерой и внутри поля зрения
        return viewportPoint.z > 0 &&
               viewportPoint.x > 0 && viewportPoint.x < 1 &&
               viewportPoint.y > 0 && viewportPoint.y < 1;
    }

    [SerializeField] private RaceData raceData;
    public void OnEndOFRace(RaceData raceData)
    {
        difficultyConfigLocal.maxPoliceCount = 0;
        this.raceData = raceData;
        spawnActive = false;
    }
}
