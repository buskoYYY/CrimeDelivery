using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    public class AvailableCars : MonoBehaviour
    {
        public List<GameObject> completedCars = new List<GameObject>();
        private SaveLoadService saveLoadService;
        public List<int> carIndexes;

        private void Start()
        {
            saveLoadService = SaveLoadService.instance;
            if (saveLoadService == null)
            {
                Debug.LogError("FarmManager instance is null!");
                enabled = false;
                return;
            }
            Debug.Log("Start");
            PopulateCompletedCars();
        }

        private void PopulateCompletedCars()
        {
            Debug.Log("В методе");
            carIndexes = new List<int>();
            Debug.Log("1");
            var constructed = saveLoadService.PlayerProgress.cunstructedCars;
            Debug.Log("2");

            // Собираем индексы всех завершённых машин
            foreach (var car in constructed)
            {
                Debug.Log("Индексы");
                if (car.isCompleted)
                {
                    carIndexes.Add(car.index);
                    Debug.Log($"Invalid car index {car.index} in SaveLoadService.");
                }
                Debug.Log("3");
            }
            Debug.Log("4");
            var configs = saveLoadService.database.carsConfigs;
            Debug.Log("5");
            // Заполняем список префабами по индексам
            foreach (int index in carIndexes)
            {
                Debug.Log("Префабы");
                if (index >= 0 && index < configs.Count && configs[index] != null)
                {
                    completedCars.Add(configs[index].gameObject);
                    Debug.Log($"Invalid car index {index} in Prefab.");
                }
                else
                {
                    Debug.LogWarning($"Invalid car index {index} in SaveLoadService.");
                }
                Debug.Log("6");
            }
            Debug.Log("Выход");
        }
    }
}









