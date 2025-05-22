using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PoliceSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public float maxSpawnDistance = 70;
    public float minSpawnDistance = 10;

    public Transform player;

    public List<CarComponentsController> policeList = new List<CarComponentsController>();
    public CarComponentsController[] policePrefabs;
    public int policeToSpawnCount = 5;
    public int maxPoliceCount = 20;

    public Camera playerCamera;

    //Ограничение по спавну

    private void Start()
    {
        StartCoroutine(SpawnPoliceCoorutine());
    }

    private IEnumerator SpawnPoliceCoorutine()
    {
        while (gameObject.activeSelf == true)
        {
            SpawnPolice();
            yield return new WaitForSeconds(3);
        }

    }

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
                    driverPolice.ChangeTarget(player);
                    driverPolice.Throttle(1);
                }

                AIDriftController ai = driver as AIDriftController;

                if (ai != null)
                {
                    ai.autoDestroy = true;
                }

                
            }
            policeInstanse.carDamageHandler.OnEndLivesEvent += OnEndOfLivesCar;
            policeList.Add(policeInstanse);
        }



    }

    private void OnDisable()
    {
        foreach (CarComponentsController police in policeList)
            police.carDamageHandler.OnEndLivesEvent -= OnEndOfLivesCar;
    }

    public void OnEndOfLivesCar(CarComponentsController car)
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
}
