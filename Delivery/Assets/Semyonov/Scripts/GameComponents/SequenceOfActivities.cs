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

        private IndicatorPlacement _indicatorPlacement;
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
            _indicatorPlacement = GetComponent<IndicatorPlacement>();
        }
        private void Start()
        {
            StartGame();
        }
        public void ClearGame()
        {
            _indicatorPlacement.ClearSubscribes();

            OnDestroy();

            if (_gatherableSource)
                Destroy(_gatherableSource.gameObject);

            if (_gameFactory.CarForPartsSpawner)
            {
                _gameFactory.CarForPartsSpawner.DestroyObjectForInteraction();

                Destroy(_gameFactory.CarForPartsSpawner.gameObject);
            }
            if (_gameFactory.ConstructedCar)
            {
                Destroy(_gameFactory.ConstructedCar.gameObject);
            }
            if (_gameFactory.WorkBenchSpawner)
            {
                _gameFactory.WorkBenchSpawner.DestroyObjectForInteraction();

                Destroy(_gameFactory.WorkBenchSpawner.gameObject);
            }
            if (_gameFactory.PumpSpawner)
            {
                _gameFactory.PumpSpawner.DestroyObjectForInteraction();

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

            _gameFactory.CreatePumpSpawner();
            _gameFactory.PumpSpawner.GetComponent<CheckProgressPump>().Init();

            _gameFactory.CreateConstructingCar();

            if (!_saveLoadService.PlayerProgress.isPumpCreated)
            {
                _gameFactory.PumpSpawner.gameObject.SetActive(false);
                _gameFactory.ConstructedCar.WheelsPlaced += ShowPumpSpawner;
            }

            _gameFactory.ConstructedCar.Init();

            _gameFactory.CreateCarForPartsSpawner();

            _gameFactory.CarForPartsSpawner.ObjectSpawned += AfterCarSpawned;

            _gameFactory.CarForPartsSpawner.GetComponent<CheckProgressCarForParts>().Init();

            _gameFactory.CreateWorkBenchSpawner();
            _gameFactory.WorkBenchSpawner.GetComponent<CheckProgressForWorkBench>().Init();

            if (!_saveLoadService.PlayerProgress.isWorkBenchSpawnerCreated)
            {
                _gameFactory.WorkBenchSpawner.gameObject.SetActive(false);
            }

            _gameFactory.ConstructedCar.DetailConstructed += OnDetailConstructed;

            _indicatorPlacement.Init();

            _indicatorPlacement.UpdatePlacement();
        }

        private void OnDetailConstructed()
        {
            if(!_gameFactory.CarForPartsSpawner.ObjectForInteraction
                || !_gameFactory.CarForPartsSpawner.ObjectForInteraction.gameObject.activeSelf)
            {
                _gameFactory.CarForPartsSpawner.gameObject.SetActive(true);
            }
        }

        private void ShowPumpSpawner()
        {
            _gameFactory.ConstructedCar.WheelsPlaced -= ShowPumpSpawner;

            _gameFactory.PumpSpawner.gameObject.SetActive(true);
        }

        private void AfterCarSpawned(ObjectForInteraction gatherableSource)
        {
            if(_gatherableSource)
                _gatherableSource.OnSetActiveFalse -= OnSatActiveFalseCarForParts;

            GatherableSource obj = gatherableSource.GetComponent<GatherableSource>();

            _gatherableSource = obj;

            _gatherableSource.OnSetActiveFalse += OnSatActiveFalseCarForParts;

            if (!_saveLoadService.PlayerProgress.isWorkBenchSpawnerCreated)
            {
                _gatherableSource.OnSetActiveFalse += ActivateWorkBenchSpawner;
            }
        }

        private void OnSatActiveFalseCarForParts(GatherableSource obj)
        {
            obj.OnSetActiveFalse -= OnSatActiveFalseCarForParts;

            _saveLoadService.PlayerProgress.isCarForPartsCreated = false;

            _saveLoadService.DelayedSaveProgress();
        }

        private void ActivateWorkBenchSpawner(GatherableSource obj)
        {
            _gameFactory.WorkBenchSpawner.gameObject.SetActive(true);

            //_gameFactory.WorkBenchSpawner.GetComponent<CheckProgressForWorkBench>().Init();

            _saveLoadService.PlayerProgress.isWorkBenchSpawnerCreated = true;

            _saveLoadService.DelayedSaveProgress();
        }
        private void OnDestroy()
        {
            if (_gatherableSource)
            {
                _gatherableSource.OnSetActiveFalse -= OnSatActiveFalseCarForParts;

                _gatherableSource.OnSetActiveFalse -= ActivateWorkBenchSpawner;
            }

            if (_gameFactory.CarForPartsSpawner)
            {
                if(_gameFactory.CarForPartsSpawner.ObjectForInteraction)
                    _gameFactory.CarForPartsSpawner.ObjectSpawned -= AfterCarSpawned;
            }

            if (_gameFactory.ConstructedCar)
            {
                _gameFactory.ConstructedCar.DetailConstructed -= OnDetailConstructed;

                _gameFactory.ConstructedCar.WheelsPlaced -= ShowPumpSpawner;
            }

        }
    }
}
