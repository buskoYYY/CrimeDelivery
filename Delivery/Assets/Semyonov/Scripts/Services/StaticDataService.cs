using ArcadeBridge.ArcadeIdleEngine.Items;
using ArcadeBridge.ArcadeIdleEngine.Processors.Transformers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcadeBridge
{
    public class StaticDataService : MonoBehaviour
    {
        public static StaticDataService instance { get; private set; }

        private AssetProvider _assetProvider;
        private const string ItemsPath = "StaticData/ConstructingData/Items";

        private const string TransformerDefinitionsForCarsPath = "StaticData/ConstructingData/TransformDefinitions";

        private const string DataForCarsPath = "StaticData/ConstructingData/DataForCars";

        private const string PositionsAndRotationsForCarPath = "StaticData/ConstructingData";

        private const string ConstructedCarsPath = "ConstructedCars";

        private const string CarForPartsSpawnerPath = "Spawners/CarForPartsUnlocker";

        private const string WorkBenchSpawnerPath = "Spawners/WorkBenchUnlocker";

        private const string PumpSpawnerPath = "Spawners/PumpUnlocker";

        private const string OffsetsForUICarButtonsPath = "StaticData/OffsetsForUICarButtons";

        public List<Vector3> offsetsForUICarButtons = new List<Vector3>();

        private Dictionary<string, Item> _namesItems = new Dictionary<string, Item>();

        //private Dictionary<string, Vector3> _itemsPositions = new Dictionary<string, Vector3>();
        //private Dictionary<string, Vector3> _itemsRotations = new Dictionary<string, Vector3>();
        private List<CarConstruction> _cunstructedCars = new List<CarConstruction>();

        private List<Dictionary<string, Vector3>> _itemsPositionsForIndex = new List<Dictionary<string, Vector3>>();
        private List<Dictionary<string, Vector3>> _itemsRotationsForIndex = new List<Dictionary<string, Vector3>>();

        private List<TransformerDefinition> _transformDefinitions = new List<TransformerDefinition>();



        private void Awake()
        {
            _assetProvider = GetComponent<AssetProvider>();

            if (instance != null)
            {
                Debug.LogWarning("StaticDataService already has");
                Destroy(gameObject);
                return;
            }
            instance = this;

            offsetsForUICarButtons = _assetProvider.Load<OffsetsForUICarButtons>(OffsetsForUICarButtonsPath).offsets;

            _namesItems = _assetProvider.Load<ItemsStaticData>(ItemsPath).items.ToDictionary(x => x.name, x => x);

            PositionsRotationsToDictionary();

            AddDataForCars();

            AddTransformerDefinitions();

            AddConstructingCar();

            //transformDefinitionsList.OrderBy(x => x.car).ToList();

            /*
            for (int i = 0; i < positionsAndRotationsForFirstCar.items.Count; i++)
            {
                _itemsPositions.Add(positionsAndRotationsForFirstCar.items[i].name, positionsAndRotationsForFirstCar.localPositions[i]);
                _itemsRotations.Add(positionsAndRotationsForFirstCar.items[i].name, positionsAndRotationsForFirstCar.localRotations[i]);
            }*/

            /*foreach (string i in _namesItems.Keys)
            {
                Debug.Log(i);
            }*/
        }

        public CarSpawner GetCarForPartsSpawner()
        {
            return _assetProvider.Load<CarSpawner>(CarForPartsSpawnerPath);
        } 
        public WorkBenchSpawner GetWorkBenchSpawner()
        {
            return _assetProvider.Load<WorkBenchSpawner>(WorkBenchSpawnerPath);
        } 
        public PumpSpawner GetPumpSpawner()
        {
            return _assetProvider.Load<PumpSpawner>(PumpSpawnerPath);
        } 
        private void AddConstructingCar()
        {
            CarConstruction[] carConstructions = _assetProvider.LoadAll<CarConstruction>(ConstructedCarsPath);

            _cunstructedCars = new List<CarConstruction>(carConstructions).OrderBy(x => x.CarIndex).ToList();
        }

        private void AddTransformerDefinitions()
        {
            TransformerDefinition[] transformerDefinition = _assetProvider.LoadAll<TransformerDefinition>(TransformerDefinitionsForCarsPath);

            _transformDefinitions = new List<TransformerDefinition>(transformerDefinition).OrderBy(x => x.carIndex).ToList();
        }

        private void AddDataForCars()
        {
            DataForCars dataForCars = _assetProvider.Load<DataForCars>(DataForCarsPath);

            dataForCars.dataForCars.OrderBy(x => x.index).ToList();

            
        }

        private void PositionsRotationsToDictionary()
        {
            PositionsAndRotationsDetailForCar[] positionsAndRotationsForCar = _assetProvider.LoadAll<PositionsAndRotationsDetailForCar>(PositionsAndRotationsForCarPath);
            List<PositionsAndRotationsDetailForCar> positionsAndRotationsForCarList = new List<PositionsAndRotationsDetailForCar>(positionsAndRotationsForCar);

            positionsAndRotationsForCarList = positionsAndRotationsForCarList.OrderBy(x => x.index).ToList();


            for (int i = 0; i < positionsAndRotationsForCarList.Count; i++)
            {
                Dictionary<string, Vector3> itemsRotations = new Dictionary<string, Vector3>();
                Dictionary<string, Vector3> itemsPositions = new Dictionary<string, Vector3>();

                for (int j = 0; j < positionsAndRotationsForCarList[i].items.Count; j++)
                {
                    itemsPositions.Add(positionsAndRotationsForCarList[i].items[j].name, positionsAndRotationsForCarList[i].localPositions[j]);
                    itemsRotations.Add(positionsAndRotationsForCarList[i].items[j].name, positionsAndRotationsForCarList[i].localRotations[j]);

                    //Debug.Log(positionsAndRotationsForCarList[i].items[j].name + " " + i);
                }

                _itemsPositionsForIndex.Add(itemsPositions);
                _itemsRotationsForIndex.Add(itemsRotations);
            }
        }

        public Item GetItem(string name) =>
            _namesItems.TryGetValue(name, out Item item)
            ? item
            : null;
        public Vector3 GetLocalDetailPositionForCar(int index, Item item)
        {
            ClearCloneFromName.Clear(item);

            Dictionary<string, Vector3> itemsPositions = _itemsPositionsForIndex[index];

            return itemsPositions.TryGetValue(item.name, out Vector3 position)
                ? position
                : Vector3.zero;
        }

        public Vector3 GetLocalDetailRotationForFirstCar(int index, Item item)
        {
            ClearCloneFromName.Clear(item);

            Dictionary<string, Vector3> itemsRotations = _itemsRotationsForIndex[index];

            return itemsRotations.TryGetValue(item.name, out Vector3 rotation)
                ? rotation
                : Vector3.zero;
        }
        public TransformerDefinition GetTransformDefinitionForCar(int index)
        {
            return index < _transformDefinitions.Count
                ? _transformDefinitions[index]
                : null;
        }
        public CarConstruction GetConstructingCar(int index)
        {
            return index < _cunstructedCars.Count
                ? _cunstructedCars[index]
                : null;
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
