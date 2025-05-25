using System;
using System.Collections.Generic;

namespace ArcadeBridge
{
    [Serializable]
    public class PlayerProgress
    {
        public List<ItemData> itemDatasInInventory = new List<ItemData>();
        public List<CarData> cunstructedCars = new List<CarData>();

        public int money = 110;

        public bool isCarForPartsCreated;

        public int needCoinsForUnloakedCar;

        public bool isCarForPartsBrokenAbsolutly;

        public int needCoinsForWorkBench;

        public bool isWorkBenchCreated;
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
        public int workBenchAlreadySpawned;

        public List<CarDetail> carDetails = new List<CarDetail>();
        public CarData(int index)
        {
            this.index = index;
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