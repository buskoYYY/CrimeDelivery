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
            _saveLoadService = GetComponent<SaveLoadService>();

        }
        private void Start()
        {
            StartGame();
        }
        public void ClearGame()
        {
            OnDestroy();

            if (_gatherableSource)
                Destroy(_gatherableSource.gameObject);
            if (_gameFactory.CarForPartsSpawner)
            {
                _gameFactory.CarForPartsSpawner.DestroyObjectForInteraction();

                //if(_gameFactory.CarForPartsSpawner.ObjectForInteraction)
                 //   Destroy(_gameFactory.CarForPartsSpawner.ObjectForInteraction.gameObject);
                Destroy(_gameFactory.CarForPartsSpawner.gameObject);
            }
            if (_gameFactory.ConstructedCar)
            {
                Destroy(_gameFactory.ConstructedCar.gameObject);
            }
            if (_gameFactory.WorkBenchSpawner)
            {
                _gameFactory.WorkBenchSpawner.DestroyObjectForInteraction();
                //if (_gameFactory.WorkBenchSpawner.ObjectForInteraction)
                //    Destroy(_gameFactory.WorkBenchSpawner.ObjectForInteraction.gameObject);

                Destroy(_gameFactory.WorkBenchSpawner.gameObject);
            }
            if (_gameFactory.PumpSpawner)
            {
                _gameFactory.PumpSpawner.DestroyObjectForInteraction();
                //if (_gameFactory.PumpSpawner.ObjectForInteraction)
                //    Destroy(_gameFactory.PumpSpawner.ObjectForInteraction.gameObject);
                Destroy(_gameFactory.PumpSpawner.gameObject);
            }
        }
        public void StartGame()
        {
            if (_saveLoadService.database != null
                && _saveLoadService.StageForNewCar >= _saveLoadService.database.carsConfigs.Count)
                return;

            if (_saveLoadService.StageForNewCar >= 3) 
                return;

            _gameFactory.CreateConstructingCar();

            _gameFactory.CreateCarForPartsSpawner();

            _gameFactory.CarForPartsSpawner.CarSpawned += AfterFistCarSpawned;

            if (_saveLoadService.PlayerProgress.isWorkBenchSpawnerCreated)
            {
                _gameFactory.CreateWorkBenchSpawner();
            }

            if(!_saveLoadService.PlayerProgress.isWheelsPumped
                && _saveLoadService.PlayerProgress.isPumpCreated)
            {
                _gameFactory.CreatePumpSpawner();
            }
            else
            {
                _gameFactory.ConstructedCar.WheelsPlaced += SpawnPumpSpawner;
            }

        }

        private void SpawnPumpSpawner()
        {
            _gameFactory.ConstructedCar.WheelsPlaced -= SpawnPumpSpawner;

            _gameFactory.CreatePumpSpawner();
        }

        private void AfterFistCarSpawned(GatherableSource obj)
        {
            _gameFactory.CarForPartsSpawner.CarSpawned -= AfterFistCarSpawned;//ObjectForInteraction.GetComponent<GatherableSource>().OnSetActiveFalse += CreateWorkBanchSpawner;

            _gatherableSource = obj;

            _gatherableSource.OnSetActiveFalse += OnSatActiveFalseCarForParts;

            if (!_saveLoadService.PlayerProgress.isWorkBenchSpawnerCreated)
            {
                _gatherableSource.OnSetActiveFalse += CreateWorkBenchSpawner;
            }
        }

        private void OnSatActiveFalseCarForParts(GatherableSource obj)
        {
            //_gatherableSource.OnSetActiveFalse -= OnSatActiveFalseCarForParts;

            _saveLoadService.PlayerProgress.isCarForPartsCreated = false;

        }

        private void CreateWorkBenchSpawner(GatherableSource obj)
        {
            _gameFactory.CreateWorkBenchSpawner();
            _saveLoadService.PlayerProgress.isWorkBenchSpawnerCreated = true;
        }
        private void OnDestroy()
        {
            if (_gatherableSource)
            {
                _gatherableSource.OnSetActiveFalse -= OnSatActiveFalseCarForParts;

                _gatherableSource.OnSetActiveFalse -= CreateWorkBenchSpawner;
            }

            if (_gameFactory.CarForPartsSpawner)
                _gameFactory.CarForPartsSpawner.CarSpawned -= AfterFistCarSpawned;
            
            if(_gameFactory.ConstructedCar)
                 _gameFactory.ConstructedCar.WheelsPlaced -= SpawnPumpSpawner;

        }
    }
}
