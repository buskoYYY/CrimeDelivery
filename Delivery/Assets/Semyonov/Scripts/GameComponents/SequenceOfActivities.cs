using ArcadeBridge.ArcadeIdleEngine.Gathering;
using System;
using System.Collections;
using UnityEngine;

namespace ArcadeBridge
{
    public class SequenceOfActivities: MonoBehaviour
    {
        public GameFactory GameFactory => _gameFactory;
        private GameFactory _gameFactory;
        private SaveLoadService _saveLoadService;
        private GatherableSource _gatherableSource;

        public static SequenceOfActivities Instance { get; private set; }
        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _gameFactory = GetComponent<GameFactory>();

        }

        private void Start()
        {
            _saveLoadService = SaveLoadService.instance;

            _gameFactory.CreateConstructingCar();

            _gameFactory.CreateCarForPartsUnlocker();

            if (_saveLoadService.PlayerProgress.isWorkBenchCreated)
            {
                _gameFactory.CreateWorkBenchSpawner();
                //int countDetail = _gameFactory.WorkBenchSpawner.ObjectForInteraction.GetComponent<>
            }
            else
            {
                _gameFactory.CarForParts.CarSpawned += AfterFistCarSpawned;//ObjectForInteraction.GetComponent<GatherableSource>().OnSetActiveFalse += CreateWorkBanchSpawner;
            }
            if(!_saveLoadService.PlayerProgress.isWheelsPumped
                && _saveLoadService.PlayerProgress.isPumpCreated)
            {
                _gameFactory.CreatePumpSpawner();
            }
            else
            {
                _gameFactory.ConstructedCar.WheelsPlaced += SpawnPump;
            }

        }

        private void SpawnPump()
        {
            _gameFactory.ConstructedCar.WheelsPlaced -= SpawnPump;

            _gameFactory.CreatePumpSpawner();
        }

        private void AfterFistCarSpawned(GatherableSource obj)
        {
            _gameFactory.CarForParts.CarSpawned -= AfterFistCarSpawned;//ObjectForInteraction.GetComponent<GatherableSource>().OnSetActiveFalse += CreateWorkBanchSpawner;

            _gatherableSource = obj;

            _gatherableSource.OnSetActiveFalse += CreateWorkBenchSpawner;
        }

        private void CreateWorkBenchSpawner(GatherableSource obj)
        {
            _gameFactory.CreateWorkBenchSpawner();
        }
        private void OnDestroy()
        {
            if(_gatherableSource)
                _gatherableSource.OnSetActiveFalse -= CreateWorkBenchSpawner;
        }
    }
}
