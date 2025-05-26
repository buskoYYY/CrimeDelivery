using ArcadeBridge.ArcadeIdleEngine.Items;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcadeBridge
{
    public class StaticDataService : MonoBehaviour
    {
        public static StaticDataService instance { get; private set; }

        private AssetProvider _assetProvider;
        private const string ItemsPath = "StaticData/Items";

        private const string PositionsAndRotationsForFirstCarPath = "StaticData/PositionsAndRotationsForTestConstructedCar";

        private Dictionary<string, Item> _namesItems = new Dictionary<string, Item>();

        private Dictionary<string, Vector3> _itemsPositions = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> _itemsRotations = new Dictionary<string, Vector3>();

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


            PositionsAndRotationsDetailForCar positionsAndRotationsForFirstCar = _assetProvider.Load<PositionsAndRotationsDetailForCar>(PositionsAndRotationsForFirstCarPath);

            for (int i = 0; i < positionsAndRotationsForFirstCar.items.Count; i++)
            {
                _itemsPositions.Add(positionsAndRotationsForFirstCar.items[i].name, positionsAndRotationsForFirstCar.localPositions[i]);
                _itemsRotations.Add(positionsAndRotationsForFirstCar.items[i].name, positionsAndRotationsForFirstCar.localRotations[i]);
            }

            /*foreach (string i in _namesItems.Keys)
            {
                Debug.Log(i);
            }*/
        }

        public Item GetItem(string name) =>
            _namesItems.TryGetValue(name, out Item item)
            ? item
            : null;
        public Vector3 GetLocalDetailPositionForFirstCar(Item item)
        {
            ClearCloneFromName.Clear(item);

            return _itemsPositions.TryGetValue(item.name, out Vector3 position)
                ? position
                : Vector3.zero;
        }

        public Vector3 GetLocalDetailRotationForFirstCar(Item item)
        {
            ClearCloneFromName.Clear(item);

            return _itemsRotations.TryGetValue(item.name, out Vector3 rotation)
                ? rotation
                : Vector3.zero;
        }
    }
    public static class ClearCloneFromName
    {
        public static void Clear(Item item)
        {
            if (item.name.Contains("(Clone)"))
            {
                item.name = item.name.Replace("(Clone)", "");
            }
        }
    }
}
