using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    public SavigsData savingsData;
    public CarsDatabase carsDatabase;

    private void Awake()
    {
        SavesManager.LoadGame();
        InitCars();
    }

    private void InitCars()
    {
        List<string> carsToAdd = new List<string>();

        for (int i = 0; i < carsDatabase.carsConfigs.Count; i++)
        {
            CarData carsData = savingsData.carsData.Find(x => x.id == carsDatabase.carsConfigs[i].id);


            if (carsData == null)
            {
                carsToAdd.Add(carsDatabase.carsConfigs[i].id);
            }
        }

        for (int i = 0; i < carsToAdd.Count; i++)
        {
            CarData carsData = new CarData();
            carsData.id = carsToAdd[i];
            carsData.carSettingsData = carsDatabase.carsConfigs.Find(x => x.id == carsData.id).carSettings;
            savingsData.carsData.Add(carsData);
        }

        List<string> carsToRemove = new List<string>();

        for (int i = 0; i < savingsData.carsData.Count; i++)
        {
            CarConfig carsData = carsDatabase.carsConfigs.Find(x => x.id == savingsData.carsData[i].id);

            if (carsData == null)
            {
                carsToRemove.Add(savingsData.carsData[i].id);
            }
        }

        for (int i = 0; i < carsToRemove.Count; i++)
        {
            savingsData.carsData.Remove(savingsData.carsData.Find(x => x.id == carsToRemove[i]));
        }

        for (int i = 0; i < savingsData.carsData.Count; i++)
        {
            CarData newCarsData = new CarData();
            newCarsData = savingsData.carsData[i];
            newCarsData.carSettingsData = carsDatabase.carsConfigs.Find(x => x.id == newCarsData.id).carSettings;
            if (newCarsData.carSettingsData.isDefault == true)
                newCarsData.openingProcent = 100;
            savingsData.carsData[i] = newCarsData;
        }
    }
}