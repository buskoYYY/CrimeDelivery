using ArcadeBridge.ArcadeIdleEngine.Items;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ArcadeBridge
{
    public class SaveLoadService : MonoBehaviour
    {
        public static SaveLoadService instance { get; private set; }
        public CarsDatabase database;

        //private const string ProgressKey = "PlayerData";
        private const string SettingsKey = "SettingsData";

        private string PlayerProgressPathFile = "Assets/Resources/playerProgress.txt";

        public PlayerProgress PlayerProgress { get; private set; }

        private Coroutine _saveCoroutine;

        private void Awake()
        {
            if(instance != null)
            {
                Debug.LogWarning("SaveLoadService already has");
                Destroy(gameObject);
                return;
            }
            instance = this;

            PlayerProgress = LoadProgress() ?? new PlayerProgress();

            if (database != null)
                ValidateData();
        }
        private void ValidateData()
        {
            List<CarData> forRemove = new List<CarData>();
            foreach(CarData carData in PlayerProgress.cunstructedCars)
            {
                foreach(CarConfig config in database.carsConfigs)
                {
                    if(int.Parse(config.id) == carData.index
                        && !config.carSettings.carNarrativeName.Equals(carData.carNarrativeName))
                    {
                        forRemove.Add(carData);
                    }
                }
            }

            foreach(CarData carData in forRemove)
            {
                PlayerProgress.cunstructedCars.Remove(carData);
            }
            DelayedSaveProgress();
        }
        public int GetStage()
        {
            CarData data = PlayerProgress.cunstructedCars.Find(x => x.isCompleted);
            int stage;

            if (data == null)
                stage = 0;
            else
                stage = data.index + 1;

            return stage;
        }
        public void AddItemToData(Item item)
        {
            ClearCloneFromName.Clear(item);

            PlayerProgress.itemDatasInInventory.Add(new ItemData(item.name));
            DelayedSaveProgress();

        }
        public void AddDetailToCarData(int carIndex, Item item)
        {
            ClearCloneFromName.Clear(item);

            //string carName = database.carsConfigs[carIndex].carSettings.carNarrativeName;

            CarData carData = CheckCarDataOrInstantiate(carIndex);

            carData.carDetails.Add(new CarDetail(item.name));

            DelayedSaveProgress();
        }

        public CarData CheckCarDataOrInstantiate(int carIndex, string carName = "")
        {
            CarData carData = null;

            foreach (CarData carData1 in PlayerProgress.cunstructedCars)
            {
                if (carIndex == carData1.index)
                {
                    carData = carData1;
                    break;
                }
            }

            if (carData == null)
            {
                carData = new CarData(carIndex);
                if (database != null)
                {
                    carData.carNarrativeName = database.carsConfigs[carIndex].carSettings.carNarrativeName;

                    carData.isDefault = database.carsConfigs[carIndex].carSettings.isDefault;
                }
                else
                    Debug.LogWarning("Cardatabase is null");

                carData.isCompleted = false;

                PlayerProgress.cunstructedCars.Add(carData);
            }

            return carData;
        }
        public CarData CheckCarDataOrInstantiate(int carIndex, bool isDefault, string carName)
        {
            CarData carData = null;

            foreach (CarData carData1 in PlayerProgress.cunstructedCars)
            {
                if (carIndex == carData1.index)
                {
                    carData = carData1;
                    break;
                }
            }

            if (carData == null)
            {
                carData = new CarData(carIndex);

                carData.isCompleted = false;

                carData.isDefault = isDefault;

                PlayerProgress.cunstructedCars.Add(carData);
            }

            return carData;
        }

        public void RemoveFromData(Item item)
        {
            ClearCloneFromName.Clear(item);

            ItemData itemDataForRemove = null;

            foreach (ItemData itemData in instance.PlayerProgress.itemDatasInInventory)
            {
                if (itemData.name.Equals(item.name))
                {
                    itemDataForRemove = itemData;
                    break;
                }
            }
            if (itemDataForRemove != null)
            {
                instance.PlayerProgress.itemDatasInInventory.Remove(itemDataForRemove);
                instance.DelayedSaveProgress();
            }
        }
        public PlayerProgress LoadProgress() =>
            JsonUtility.FromJson<PlayerProgress>(File.ReadAllText(PlayerProgressPathFile));
        //JsonUtility.FromJson<PlayerProgress>(PlayerPrefs.GetString(ProgressKey));
        public void DelayedSaveProgress()
        {
            if (_saveCoroutine != null)
                StopCoroutine(_saveCoroutine);

            _saveCoroutine = StartCoroutine(DelayedSaveProgressCoroutine());
        }
        private IEnumerator DelayedSaveProgressCoroutine()
        {
            yield return new WaitForSeconds(0.3f);
            SaveProgressOnDevice();
            SaveProgressOnCloud();
            _saveCoroutine = null;
        }
        private void SaveProgressOnCloud()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        _webMediatorService.SavePlayerData(_progressService.ProgressData);
#endif
        }
        private void SaveProgressOnDevice()
        {
            string dataString = JsonUtility.ToJson(PlayerProgress);

            byte[] bytes = Encoding.ASCII.GetBytes(dataString);

            File.WriteAllBytes(PlayerProgressPathFile, bytes);
        }

        //PlayerPrefs.SetString(ProgressKey, JsonUtility.ToJson(PlayerProgress));
    }
}
