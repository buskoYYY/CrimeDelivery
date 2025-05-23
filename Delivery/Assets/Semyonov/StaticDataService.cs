using ArcadeBridge.ArcadeIdleEngine.Items;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcadeBridge
{
    public class StaticDataService: MonoBehaviour
    {
        public static StaticDataService instance { get; private set; }

        private AssetProvider _assetProvider;
        private const string ItemsPath = "StaticData/Items";
        private Dictionary<string, Item> _namesItems = new Dictionary<string, Item>();

        private void Awake()
        {
            _assetProvider = GetComponent<AssetProvider>();

            if (instance != null)
            {
                Debug.LogWarning("SaveLoadService already has");
                Destroy(gameObject);
                return;
            }
            instance = this;

            _namesItems = _assetProvider.Load<ItemsStaticData>(ItemsPath).items.ToDictionary(x => x.name, x => x);

            /*foreach (string i in _namesItems.Keys)
            {
                Debug.Log(i);
            }*/
        }

        public Item GetItem(string name) =>
            _namesItems.TryGetValue(name, out Item item)
            ? item
            : null;
    }
}
