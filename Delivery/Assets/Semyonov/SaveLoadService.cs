using ArcadeBridge.ArcadeIdleEngine.Items;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace ArcadeBridge
{
    public class SaveLoadService : MonoBehaviour
    {
        public static SaveLoadService instance { get; private set; }

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

            //PlayerProgress
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

            CarData carData = CheckCarDataOrInstantiate(carIndex);

            carData.carDetails.Add(new CarDetail(item.name));

            DelayedSaveProgress();
        }

        public CarData CheckCarDataOrInstantiate(int carIndex)
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
