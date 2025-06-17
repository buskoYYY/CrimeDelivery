using System;
using System.Collections.Generic;

namespace ArcadeBridge
{
    [Serializable]
    public class PlayerProgress
    {
        public List<ItemData> itemDatasInInventory = new List<ItemData>();
        public List<CarData> cunstructedCars = new List<CarData>();

        public int selectedCarIdInDatabase = 0;

        public int deliveriesCount;

        public int money = 50000;

        public bool isWheelsPumped;

        public bool isPumpCreated;

        public int needCoinsForUnloakedPump;

        public bool isCarForPartsCreated;

        public int needCoinsForUnloakedCar;

        public bool isCarForPartsBrokenAbsolutly;

        public int needCoinsForWorkBench;

        public bool isWorkBenchCreated;

        public bool isWorkBenchSpawnerCreated;
    }
    [Serializable]
    public class ItemData
    {
        public string name;
        public ItemData(string name)
        {
            this.name = name;
        }
    }
    [Serializable]
    public class CarData
    {
        public int index;
        public bool isCompleted;
        public int workBenchAlreadySpawnedCount;
        public string carNarrativeName;
        public bool isDefault = false; //если true, то машина открыта сразу и её не нужно покупать

        public List<CarDetail> carDetails = new List<CarDetail>();
        public CarData(int index, string name ="")
        {
            this.index = index;
            carNarrativeName = name;
        }
        public CarData(int index, bool isDefault, string name)
        {
            this.index = index;
            carNarrativeName = name;
        }
    }

    [Serializable]
    public class CarDetail
    {
        public string name;
        public CarDetail(string name)
        {
            this.name = name;
        }
    }
}