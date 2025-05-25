using ArcadeBridge.ArcadeIdleEngine.Items;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    [CreateAssetMenu(menuName = "StaticData/Items", fileName = "Items")]
    public class ItemsStaticData : ScriptableObject
    {
        public List<Item> items;
    }
}
