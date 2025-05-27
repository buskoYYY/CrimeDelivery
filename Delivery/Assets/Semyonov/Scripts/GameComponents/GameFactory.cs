using UnityEngine;

namespace ArcadeBridge
{
    public class GameFactory: MonoBehaviour
    {
        [SerializeField] private CarConstruction constructedCar;
        private void Start()
        {
            int stage = SaveLoadService.instance.PlayerProgress.cunstructedCars.Count - 1;


        }
    }
}
