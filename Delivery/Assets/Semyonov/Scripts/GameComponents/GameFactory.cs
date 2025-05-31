using UnityEngine;

namespace ArcadeBridge
{
    public class GameFactory: MonoBehaviour
    {
        public CarConstruction ConstructedCar { get; private set; }
        public CarSpawner CarForPartsSpawner { get; private set; }
        public WorkBenchSpawner WorkBenchSpawner { get; private set; }
        public PumpSpawner PumpSpawner { get; private set; }


        public CarConstruction CreateConstructingCar()
        {
            int stage = SaveLoadService.instance.StageForNewCar;

            CarConstruction car = StaticDataService.instance.GetConstructingCar(stage);

            if (car == null)
                car = StaticDataService.instance.GetConstructingCar(stage - 1);

            ConstructedCar = Instantiate(car);//, _positionConstructedCar, Quaternion.Euler(_rotationConstructedCar));

            return ConstructedCar;
        }

        public CarSpawner CreateCarForPartsSpawner()
        {
            CarSpawner carSpawner = StaticDataService.instance.GetCarForPartsSpawner();

            CarForPartsSpawner = Instantiate(carSpawner);
                
            return CarForPartsSpawner;
        }
        public WorkBenchSpawner CreateWorkBenchSpawner()
        {
            WorkBenchSpawner spawner = StaticDataService.instance.GetWorkBenchSpawner();

            WorkBenchSpawner = Instantiate(spawner);

            return WorkBenchSpawner;
        }
        public PumpSpawner CreatePumpSpawner()
        {
            PumpSpawner spawner = StaticDataService.instance.GetPumpSpawner();

            PumpSpawner = Instantiate(spawner);
            
            return PumpSpawner;
        }
    }
}
