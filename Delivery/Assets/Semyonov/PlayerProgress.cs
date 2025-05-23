using System;
using System.Collections.Generic;

namespace ArcadeBridge
{
    [Serializable]
    public class PlayerProgress
    {
        public List<ItemData> itemDatasInInventory = new List<ItemData>();

        public bool isCarForPartsCreated;

        public bool isCarForPartsBrokenAbsolutly;

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
}