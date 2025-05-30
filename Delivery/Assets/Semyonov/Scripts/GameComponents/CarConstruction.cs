using ArcadeBridge.ArcadeIdleEngine.Items;
using ArcadeBridge.ArcadeIdleEngine.Processors.Sellers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    public class CarConstruction : MonoBehaviour
    {
        public event Action WheelsPlaced;
        public int CarIndex => _carIndex;
        public int NeedCountForComplete => _needCountForComplete;
        public int ConstructedDetailsCount => _constructedDetailsCount;

        [SerializeField] private int _carIndex;
        [SerializeField] private List<SellerFloatingText> _objectGetters = new List<SellerFloatingText>();

        private List<Item> _details = new List<Item>();

        private int _needCountForComplete;
        private int _constructedDetailsCount;
        private void Start()
        {
            foreach(SellerFloatingText objectGetter in _objectGetters)
            {
                _needCountForComplete += objectGetter.Definition.SellableItemDefinitions.Length;

                objectGetter.RemovingObjectFromInventoryWithSave += ConstructNewDetail;

                objectGetter.InitState(_carIndex);
            }
        }

        private void ConstructNewDetail(Item obj, bool withSave = true)
        {
            _constructedDetailsCount++;

            if(_constructedDetailsCount == 4)
            {
                WheelsPlaced?.Invoke();
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

                if (_constructedDetailsCount == _needCountForComplete)
                {
                    foreach(CarData carData in SaveLoadService.instance.PlayerProgress.cunstructedCars)
                    {
                        if(carData.index == _carIndex)
                        {
                            carData.isCompleted = true;

                            SaveLoadService.instance.PlayerProgress.isCarForPartsBrokenAbsolutly = false;
                            SaveLoadService.instance.PlayerProgress.isCarForPartsCreated = false;
                            SaveLoadService.instance.PlayerProgress.isPumpCreated = false;
                            SaveLoadService.instance.PlayerProgress.isWheelsPumped = false;
                            SaveLoadService.instance.PlayerProgress.isWorkBenchCreated = false;
                            SaveLoadService.instance.PlayerProgress.isWorkBenchSpawnerCreated = false;
                            SaveLoadService.instance.PlayerProgress.needCoinsForUnloakedCar = -1;
                            SaveLoadService.instance.PlayerProgress.needCoinsForUnloakedPump = -1;
                            SaveLoadService.instance.PlayerProgress.needCoinsForWorkBench = -1;
                            SaveLoadService.instance.DelayedSaveProgress();
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            foreach (SellerFloatingText objectGetter in _objectGetters)
            {
                objectGetter.RemovingObjectFromInventoryWithSave -= ConstructNewDetail;
            }
        }
    }
}
