using ArcadeBridge.ArcadeIdleEngine.Items;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    [CreateAssetMenu(menuName = "StaticData/PositionsAndRotationsForFirstCar", fileName = "PositionsAndRotationsForFirstCar")]
    public class PositionsAndRotationsDetailForCar: ScriptableObject
    {
        public int index;
        public List<Item> items;
        public List<Vector3> localPositions;
        public List<Vector3> localRotations;
    }
}
