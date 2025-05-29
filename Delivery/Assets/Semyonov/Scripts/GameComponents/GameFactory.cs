using UnityEngine;

namespace ArcadeBridge
{
    public class GameFactory: MonoBehaviour
    {
        public CarConstruction ConstructedCar { get; private set; }
        public CarSpawner CarForParts { get; private set; }
        public WorkBenchSpawner WorkBenchSpawner { get; private set; }
        public PumpSpawner PumpSpawner { get; private set; }


        public CarConstruction CreateConstructingCar()
        {
            int stage;

            CarData data = SaveLoadService.instance.PlayerProgress.cunstructedCars.Find(x => x.isCompleted);

            if (data == null)
                stage = 0;
            else
                stage = data.index + 1;

            CarConstruction car = StaticDataService.instance.GetConstructingCar(stage);

            ConstructedCar = Instantiate(car);//, _positionConstructedCar, Quaternion.Euler(_rotationConstructedCar));

            return ConstructedCar;
        }

        public CarSpawner CreateCarForPartsUnlocker()
        {
            CarSpawner carSpawner = StaticDataService.instance.GetCarForPartsSpawner();

            CarForParts = Instantiate(carSpawner);
                
            return CarForParts;
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
