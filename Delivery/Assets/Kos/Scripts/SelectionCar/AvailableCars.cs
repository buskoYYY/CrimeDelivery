using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    public class AvailableCars : MonoBehaviour
    {
        [SerializeField] private CarsDatabase carDatabase;

        public List<GameObject> defaultCars = new List<GameObject>();

        private void Awake()
        {
            PopulateDefaultCars();
        }

        private void PopulateDefaultCars()
        {
            if (carDatabase == null || carDatabase.carsConfigs == null)
            {
                Debug.LogWarning("CarsDatabase is missing or not assigned.");
                return;
            }

            defaultCars.Clear();

            foreach (var carConfig in carDatabase.carsConfigs)
            {
                if (carConfig == null || carConfig.carSettings == null || carConfig.gameObject == null)
                    continue;

                if (carConfig.carSettings.isDefault)
                {
                    defaultCars.Add(carConfig.gameObject);
                }
            }
        }
    }
}


