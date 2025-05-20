using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PoliceSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public float maxSpawnDistance = 30;

    public Transform player;

    public List<CarComponentsController> policeList = new List<CarComponentsController>();
    public CarComponentsController[] policePrefabs;
    public int policeToSpawnCount = 5;
    public int maxPoliceCount = 20;

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
        List<Transform> nearbyPoints = new List<Transform>();

        foreach (Transform point in spawnPoints)
        {
            float distance = Vector3.Distance(player.position, point.position);
            if (distance <= maxSpawnDistance)
            {
                nearbyPoints.Add(point);
            }
        }


        Shuffle(nearbyPoints);
        int count = Mathf.Min(policeToSpawnCount, nearbyPoints.Count);
        for (int i = 0; i < count; i++)
        {
            if (policeList.Count >= maxPoliceCount)
                break;

            CarComponentsController policeInstanse = Instantiate(policePrefabs[0], nearbyPoints[i].position, nearbyPoints[i].rotation);

            Driver driverPolice;
            foreach (CarComponent driver in policeInstanse.carComponents)
            {
                driverPolice = driver as Driver;
                if (driverPolice != null)
                {
                    driverPolice.ChangeTarget(player);
                    driverPolice.Throttle(1);
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
}
