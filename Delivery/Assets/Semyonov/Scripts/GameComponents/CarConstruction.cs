using ArcadeBridge.ArcadeIdleEngine.Items;
using ArcadeBridge.ArcadeIdleEngine.Processors.Sellers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    public class CarConstruction : MonoBehaviour
    {
        [SerializeField] private int _carIndex;
        [SerializeField] private List<SellerFloatingText> _objectGetters = new List<SellerFloatingText>();

        private List<Item> _details = new List<Item>();

        private int _needCountForComplete;
        private int _constructedDetailsCount;
        private void Start()
        {
            foreach(SellerFloatingText objectGetter in _objectGetters)
            {
                _needCountForComplete = objectGetter.Definition.SellableItemDefinitions.Length;

                objectGetter.RemovingObjectFromInventoryWithSave += ConstructNewDetail;

                objectGetter.InitState(_carIndex);
            }
        }

        private void ConstructNewDetail(Item obj, bool withSave = true)
        {
            _constructedDetailsCount++;

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
