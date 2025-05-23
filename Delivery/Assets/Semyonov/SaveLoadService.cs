using ArcadeBridge.ArcadeIdleEngine.Items;
using System.Collections;
using UnityEngine;

namespace ArcadeBridge
{
    public class SaveLoadService : MonoBehaviour
    {
        public static SaveLoadService instance { get; private set; }

        private const string ProgressKey = "PlayerData";
        private const string SettingsKey = "SettingsData";

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
        }
        public void AddItemToData(Item item)
        {
            if (item.name.Contains("(Clone)"))
            {
                item.name = item.name.Replace("(Clone)", "");
            }
            PlayerProgress.itemDatasInInventory.Add(new ItemData(item.name));
            DelayedSaveProgress();

        }

        public void RemoveFromData(Item item)
        {
            if (item.name.Contains("(Clone)"))
            {
                item.name = item.name.Replace("(Clone)", "");
            }

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
             JsonUtility.FromJson<PlayerProgress>(PlayerPrefs.GetString(ProgressKey));
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
        private void SaveProgressOnDevice() =>
            PlayerPrefs.SetString(ProgressKey, JsonUtility.ToJson(PlayerProgress));
    }
}
