using System.Collections;
using UnityEngine;

public class SpawnerOnPlayer : MonoBehaviour
{
    public CarComponentsController prefabToSpawn;
    public Vector3 spawnSize = new Vector3(1, 1, 1); // Размер области проверки
    public Transform[] spawnPositions;
    public Transform[] startPositions;
    public LayerMask collisionMask;

    public int amountOfPolice = 2;
    /*
    private void Start()
    {
        StartCoroutine(SpawnCoorutine());
    }

    private IEnumerator SpawnCoorutine()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(3);
            int spawnCount = Mathf.Min(spawnPositions.Length, amountOfPolice);
            for (int i = 0; i < spawnCount; i++)
            {
                TrySpawn(startPositions[i],spawnPositions[i]);
            }
        }

    }

    public void TrySpawn(Transform startPosition, Transform spawnPosition)
    {
        // 1. Проверка наличия поверхности под точкой спавна
        if (!Physics.Raycast(spawnPosition.position, Vector3.down, out RaycastHit groundHit, 10f, collisionMask))
        {
            Debug.Log("Не удалось заспавнить: нет поверхности под точкой.");
            return;
        }

        Vector3 finalSpawnPosition = groundHit.point;

        // 2. Проверка препятствий по пути BoxCast
        Vector3 direction = (spawnPosition.position - startPosition.position).normalized;
        float distance = Vector3.Distance(startPosition.position, finalSpawnPosition);
        bool hit = Physics.BoxCast(startPosition.position, spawnSize * 0.5f, direction, out RaycastHit hitInfo, spawnPosition.rotation, distance, collisionMask);

        if (!hit)
        {
            Quaternion rotation = Quaternion.Euler(0, spawnPosition.eulerAngles.y, 0);
            Instantiate(prefabToSpawn, finalSpawnPosition, rotation);
        }
        else
        {
            Debug.Log("Не удалось заспавнить: путь к точке занят.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnSize);
    }
    */
}
