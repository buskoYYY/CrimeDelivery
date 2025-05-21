using UnityEngine;
using System.Collections.Generic;

namespace ArcadeBridge
{
    public class LevelConfig : MonoBehaviour
    {
        [SerializeField] private List<GameObject> policeComponents;
        [SerializeField] private List<Transform> spawnPoints;

        void Start()
        {
            SpawnPolice();
        }

        private void SpawnPolice()
        {
            // Проверка на наличие элементов в списках
            if (policeComponents == null || policeComponents.Count == 0)
            {
                Debug.LogError("No police components assigned!", this);
                return;
            }

            if (spawnPoints == null || spawnPoints.Count == 0)
            {
                Debug.LogError("No spawn points assigned!", this);
                return;
            }

            // Перебераем список позиций
            foreach (Transform spawnPoint in spawnPoints)
            {
                // Случайный префаб
                int randomIndex = Random.Range(0, policeComponents.Count);
                GameObject randomPolicePrefab = policeComponents[randomIndex];
                
                // Создаем экземпляр
                Instantiate(randomPolicePrefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
    }
}
