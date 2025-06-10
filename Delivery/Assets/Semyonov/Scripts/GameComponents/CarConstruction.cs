using ArcadeBridge.ArcadeIdleEngine.Interactables;
using ArcadeBridge.ArcadeIdleEngine.Items;
using ArcadeBridge.ArcadeIdleEngine.Processors.Sellers;
using ArcadeBridge.ArcadeIdleEngine.Processors.Transformers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    public class CarConstruction : MonoBehaviour
    {
        public event Action DetailConstructed;
        public event Action WheelsPlaced;
        public int CarIndex => _carIndex;
        public int NeedCountForComplete => _needCountForComplete;
        public int ConstructedDetailsCount => _constructedDetailsCount;

        //[SerializeField] private InventoryCollectorTriggerArea _inventoryCollectorTriggerArea;
        [SerializeField] private int _carIndex;
        [SerializeField] private List<SellerFloatingText> _objectGetters = new List<SellerFloatingText>();

        private List<Item> _details = new List<Item>();

        private int _needCountForComplete;
        private int _constructedDetailsCount;
        public void Init()
        {
            _objectGetters[_objectGetters.Count - 1].gameObject.SetActive(false);

            foreach (SellerFloatingText objectGetter in _objectGetters)
            {
                _needCountForComplete += objectGetter.Definition.SellableItemDefinitions.Length;

                objectGetter.RemovingObjectFromInventoryWithSave += ConstructNewDetail;

                objectGetter.InitState(_carIndex);
            }
        }
        
        private void ConstructNewDetail(Item obj, bool withSave = true)
        {
            //_inventoryCollectorTriggerArea.ItemPlaced();
            //SequenceOfActivities.Instance.GameFactory.WorkBenchSpawner.ObjectForInteraction.GetComponent<Transformer>().InputArea.ItemPlaced();

            DetailConstructed?.Invoke();

            _constructedDetailsCount++;

            if (_constructedDetailsCount == 4)
            {
                WheelsPlaced?.Invoke();

                if (!SaveLoadService.instance.PlayerProgress.isWheelsPumped)
                {
                    SequenceOfActivities.Instance.GameFactory.PumpSpawner.OnWheelsPumped += ContinueConstructionCar;
                }
                else
                    ContinueConstructionCar();
            }

            Vector3 localPosition = StaticDataService.instance.GetLocalDetailPositionForCar(_carIndex, obj);
            Vector3 localRotation = StaticDataService.instance.GetLocalDetailRotationForFirstCar(_carIndex, obj);

            Item detail = Instantiate<Item>(obj, transform);

            detail.transform.localPosition = localPosition;
            detail.transform.localEulerAngles = localRotation;

            _details.Add(detail);

            PumpingWheelTrigger wheelTrigger = detail.GetComponentInChildren<PumpingWheelTrigger>();

            if (wheelTrigger != null)
            {
                wheelTrigger.Activate();
            }

            if (SaveLoadService.instance != null && withSave)
            {
                SaveLoadService.instance.AddDetailToCarData(_carIndex, detail);

                if (_constructedDetailsCount == _needCountForComplete && SaveLoadService.instance.PlayerProgress.isWheelsPumped)
                {
                    foreach(CarData carData in SaveLoadService.instance.PlayerProgress.cunstructedCars)
                    {
                        if(carData.index == _carIndex)
                        {
                            carData.isCompleted = true;

                            //SaveLoadService.instance.PlayerProgress.isPumpCreated = false;
                            //SaveLoadService.instance.PlayerProgress.isWorkBenchCreated = false;
                            //SaveLoadService.instance.PlayerProgress.isWorkBenchSpawnerCreated = false;
                            //SaveLoadService.instance.PlayerProgress.isCarForPartsCreated = false;
                            SaveLoadService.instance.PlayerProgress.isCarForPartsBrokenAbsolutly = false;
                            SaveLoadService.instance.PlayerProgress.isWheelsPumped = false;
                            SaveLoadService.instance.PlayerProgress.needCoinsForUnloakedCar = -1;
                            SaveLoadService.instance.PlayerProgress.needCoinsForUnloakedPump = -1;
                            SaveLoadService.instance.PlayerProgress.needCoinsForWorkBench = -1;
                            SaveLoadService.instance.DelayedSaveProgress();

                            ConstructionCanvas.Instance.ShowFinishWindow();
                        }
                    }
                }
            }
        }

        private void ContinueConstructionCar()
        {
            _objectGetters[_objectGetters.Count - 1].gameObject.SetActive(true);

            SequenceOfActivities.Instance.GameFactory.PumpSpawner.OnWheelsPumped -= ContinueConstructionCar;
        }

        private void OnDestroy()
        {
            foreach (SellerFloatingText objectGetter in _objectGetters)
            {
                objectGetter.RemovingObjectFromInventoryWithSave -= ConstructNewDetail;
            }

            if(SequenceOfActivities.Instance != null && SequenceOfActivities.Instance.GameFactory.PumpSpawner)
                SequenceOfActivities.Instance.GameFactory.PumpSpawner.OnWheelsPumped -= ContinueConstructionCar;
        }
    }
}
