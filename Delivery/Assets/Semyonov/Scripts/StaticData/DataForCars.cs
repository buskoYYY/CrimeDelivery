using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    [CreateAssetMenu(menuName = "StaticData/DataForCars", fileName = "DataForCars")]
    public class DataForCars: ScriptableObject
    {
        [Header("Needs index = index in List")]
        public List<DataForCar> dataForCars = new List<DataForCar>();
    }
}
