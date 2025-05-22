using UnityEngine;

namespace ArcadeBridge
{
    public class CarSpawner : MonoBehaviour
    {
        [SerializeField] private Car _car;
        public void CreateCar()
        {
            Instantiate(_car);
        }
    }
}
