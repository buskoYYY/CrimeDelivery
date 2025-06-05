using ArcadeBridge.ArcadeIdleEngine.Gathering;
using ArcadeBridge.ArcadeIdleEngine.Items;
using ArcadeBridge.ArcadeIdleEngine.Processors.Transformers;
using ArcadeBridge.ArcadeIdleEngine.Storage;
using System;
using UnityEngine;

namespace ArcadeBridge
{
    public class IndicatorPlacement: MonoBehaviour
    {
        [SerializeField] private Inventory _playerInventory;

        private const string Detail = "Detail";

        private GameFactory _gameFactory;
        private SaveLoadService _saveLoad;
        private CarData _carData;

        private Transformer _workbench;

        private void Awake()
        {
            _gameFactory = GetComponent<GameFactory>();
            _gameFactory.CreateIndicator(Vector3.zero);
            _saveLoad = SaveLoadService.instance;
        }

        private void Start()
        {
        }
        public void ClearSubscribes()
        {
            OnDestroy();
        }
        public void Init()
        {
            _carData = _saveLoad.CheckCarDataOrInstantiate(_saveLoad.StageForNewCar);

            _playerInventory.ItemAdded += UpdatePlacement1;
            _playerInventory.ItemRemoved += UpdatePlacement1;

            _gameFactory.CarForPartsSpawner.ObjectSpawned += OnCarForPartsSpawn;

            _gameFactory.WorkBenchSpawner.ObjectSpawned += UpdatePlacement2;

            _workbench = _gameFactory.WorkBenchSpawner.ObjectForInteraction.GetComponent<Transformer>();

            _workbench.ItemProduced += UpdatePlacement;

            _gameFactory.PumpSpawner.ObjectSpawned += UpdatePlacement2;
            (_gameFactory.PumpSpawner.ObjectForInteraction as Pump).OnPumpTaken += UpdatePlacement;
            _gameFactory.PumpSpawner.OnWheelsPumped += UpdatePlacement;

            _gameFactory.ConstructedCar.DetailConstructed += UpdatePlacement;
            _gameFactory.ConstructedCar.WheelsPlaced += UpdatePlacement;
        }


        public void UpdatePlacement()
        {
            if (_saveLoad.PlayerProgress.isWheelsPumped)
            {
                CheckItemInOutputTransformer();
            }
            else
            {
                if(_gameFactory.ConstructedCar.ConstructedDetailsCount == 4)
                {
                    if (_gameFactory.PumpSpawner.ObjectForInteraction && _gameFactory.PumpSpawner.ObjectForInteraction.gameObject.activeSelf)
                    {
                        if((_gameFactory.PumpSpawner.ObjectForInteraction as Pump).PumpTaken)
                        {
                            _gameFactory.Indicator.transform.position = _gameFactory.ConstructedCar.transform.position;
                        }
                        else
                        {
                            _gameFactory.Indicator.transform.position = _gameFactory.PumpSpawner.ObjectForInteraction.transform.position;
                        }
                    }
                    else
                    {
                        _gameFactory.Indicator.transform.position = _gameFactory.PumpSpawner.transform.position;
                    }
                }
                else
                {
                    CheckItemInOutputTransformer();
                }
            }
        }

        private bool CheckConstructionDetail()
        {
            Item item = _playerInventory.InventoryVisible.Items.Find(x => !x.name.Equals(Detail));

            if (item != null)
            {
                _gameFactory.Indicator.transform.position = _gameFactory.ConstructedCar.transform.position;
            }

            return item != null;
        }

        private void CheckItemInOutputTransformer()
        {
            bool checkConstructionDetail = CheckConstructionDetail();

            if (_workbench 
                && _workbench.gameObject.activeSelf
                && _workbench.OutputInventory.InventoryVisible.Count > 0
                && !checkConstructionDetail)
            {
                _gameFactory.Indicator.transform.position = _workbench.OutputInventory.transform.position;
            }
            else if(!checkConstructionDetail)
            {
                CheckSalableDetail();
            }
        }

        private void CheckSalableDetail()
        {
            Item itemSalable = _playerInventory.InventoryVisible.Items.Find(x => x.name.Equals(Detail));

            if (itemSalable != null)
            {
                CheckWorkbench();
            }
            else
            {
                CheckCarForParts();
            }
        }

        private void CheckWorkbench()
        {
            if (_gameFactory.WorkBenchSpawner.ObjectForInteraction
                && _gameFactory.WorkBenchSpawner.ObjectForInteraction.gameObject.activeSelf)
            {
                _gameFactory.Indicator.transform.position = _workbench.InputInventory.transform.position;
            }
            else
            {
                CheckWorkbenchSpawner();
            }
        }

        private void CheckWorkbenchSpawner()
        {
            if (_gameFactory.WorkBenchSpawner
               && _gameFactory.WorkBenchSpawner.gameObject.activeSelf)
            {
                _gameFactory.Indicator.transform.position = _gameFactory.WorkBenchSpawner.transform.position;
            }
            else
            {
                CheckCarForParts();
            }
        }

        private void CheckCarForParts()
        {
            if (_gameFactory.CarForPartsSpawner.ObjectForInteraction && _gameFactory.CarForPartsSpawner.ObjectForInteraction.gameObject.activeSelf)
            {
                _gameFactory.Indicator.transform.position = _gameFactory.CarForPartsSpawner.ObjectForInteraction.transform.position;
            }
            else
            {
                _gameFactory.Indicator.transform.position = _gameFactory.CarForPartsSpawner.transform.position;
            }
        }

        private void OnDestroy()
        {
            if (_workbench)
            {
                _workbench.ItemProduced -= UpdatePlacement;
            }

            if (_playerInventory)
            {
                _playerInventory.ItemAdded -= UpdatePlacement1;
                _playerInventory.ItemRemoved -= UpdatePlacement1;
            }

            if (_gameFactory.CarForPartsSpawner)
            {
                _gameFactory.CarForPartsSpawner.ObjectSpawned -= OnCarForPartsSpawn;

                if (_gameFactory.CarForPartsSpawner.ObjectForInteraction)
                {
                    _gameFactory.CarForPartsSpawner.ObjectForInteraction.GetComponent<GatherableSource>().OnSetActiveFalse -= UpdatePlacement3;
                }
            }

            if(_gameFactory.WorkBenchSpawner)
                _gameFactory.WorkBenchSpawner.ObjectSpawned -= UpdatePlacement2;

            if (_gameFactory.PumpSpawner)
            {
                _gameFactory.PumpSpawner.ObjectSpawned -= UpdatePlacement2;
                _gameFactory.PumpSpawner.OnWheelsPumped -= UpdatePlacement;

                if(_gameFactory.PumpSpawner.ObjectForInteraction)
                    (_gameFactory.PumpSpawner.ObjectForInteraction as Pump).OnPumpTaken -= UpdatePlacement;
            }

            if (_gameFactory.ConstructedCar)
            {
                _gameFactory.ConstructedCar.DetailConstructed -= UpdatePlacement;
                _gameFactory.ConstructedCar.WheelsPlaced -= UpdatePlacement;
            }
        }
        private void OnCarForPartsSpawn(ObjectForInteraction obj)
        {
            obj.GetComponent<GatherableSource>().OnSetActiveFalse += UpdatePlacement3;

            UpdatePlacement();
        }
        private void UpdatePlacement2(ObjectForInteraction obj) => UpdatePlacement();
        private void UpdatePlacement1(Item arg1, int arg2) => UpdatePlacement();
        private void UpdatePlacement3(GatherableSource obj) => UpdatePlacement();
    }
}
