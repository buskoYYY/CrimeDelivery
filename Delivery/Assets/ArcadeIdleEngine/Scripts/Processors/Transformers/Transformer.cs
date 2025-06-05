using System;
using System.Collections;
using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Helpers;
using ArcadeBridge.ArcadeIdleEngine.Interactables;
using ArcadeBridge.ArcadeIdleEngine.Items;
using ArcadeBridge.ArcadeIdleEngine.Storage;
using ArcadeIdleEngine.ExternalAssets.NaughtyAttributes_2._1._4.Core.MetaAttributes;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Processors.Transformers
{
    [SelectionBase]
    public class Transformer : MonoBehaviour
    {
        public Inventory InputInventory => _inputInventory;
        public Inventory OutputInventory => _outputInventory;

        [SerializeField] TransformerDefinition _definition;
        
        [Header("Inventories")]
        [SerializeField] Inventory _inputInventory;
        [SerializeField] Inventory _outputInventory;

        [Header("Points")]
        [SerializeField, Tooltip("Point where the transformation happens. You most likely want to place it somewhere hidden from user.")] 
        Transform _transformationPoint;
        [SerializeField, ShowIf(nameof(_usePreTransformationQueue))] 
        Transform _transformationQueue;

        [Header("Timers")]
        [SerializeField] Timer _transformationTimer;
        [SerializeField] Timer _collectingForTransformationTimer;

        [Header("Configurations")]
        [SerializeField] bool _usePreTransformationQueue;
        [SerializeField, ShowIf(nameof(_usePreTransformationQueue))] 
        RowColumnHeight _transformationQueueLayout;
        
        Dictionary<ItemDefinition, int> _neededResources = new Dictionary<ItemDefinition, int>();
        List<Item> _itemsOnTransformationQueue;
        bool _transforming;
        
        public event Action<ItemDefinition, int> ItemAdded;
        public event Action ItemProduced;

        public TransformerRuleset Ruleset => _definition.Ruleset;

        //public InventoryCollectorTriggerArea InputArea => _inputArea;

        //private InventoryCollectorTriggerArea _inputArea;
        private int _alreadySpawnedOutputValue;

        private CarData _carData = null;

        void Awake()
        {
            foreach (ItemDefinitionCountPair rulesetTypeCountPair in _definition.Ruleset.Inputs)
            {
                _neededResources.Add(rulesetTypeCountPair.ItemDefinition, rulesetTypeCountPair.Count);
            }

            if (_usePreTransformationQueue)
            {
                _itemsOnTransformationQueue = new List<Item>(6);
            }
            //_inputArea = _inputInventory.GetComponent<InventoryCollectorTriggerArea>();
        }

        public void SetDefinition(TransformerDefinition definition)
        {
            _definition = definition;
        }
        private void Start()
        {
            if (SaveLoadService.instance != null)
            {
                _carData = SaveLoadService.instance.CheckCarDataOrInstantiate(_definition.carIndex);

                _alreadySpawnedOutputValue = _carData.workBenchAlreadySpawnedCount;

                //_inputArea.SetAlreadySpawnedCount(_alreadySpawnedOutputValue);
            }
        }
        void Update()
        {
            if (_transforming)
            {
                _transformationTimer.Tick();
                if (_transformationTimer.IsCompleted)
                {
                    TryProduceOutput();
                    _transformationTimer.SetZero();
                    _transforming = false;
                }
            }

            if (_inputInventory.IsEmpty)
            {
                return;
            }

            if (_alreadySpawnedOutputValue >= _definition.Ruleset.Outputs.Length)
                return;

            /*if(SequenceOfActivities.Instance != null 
                && SequenceOfActivities.Instance.GameFactory.ConstructedCar.ConstructedDetailsCount < _alreadySpawnedOutputValue)
            {
                return;
            }*/

            // If we still need to pick something and we can pick something, start the timer.
            foreach (ItemDefinitionCountPair itemDefinitionCountPair in _definition.Ruleset.Inputs)
            {                
                if (_neededResources[itemDefinitionCountPair.ItemDefinition] <= 0)
                {
                    continue;
                }

                if (_inputInventory.Contains(itemDefinitionCountPair.ItemDefinition, out Item item))
                {
                    if (_collectingForTransformationTimer.IsCompleted)
                    {
                        if(SaveLoadService.instance != null)
                            SaveLoadService.instance.RemoveFromData(item);

                        _inputInventory.Remove(item);
                        AddToTransformationQueue(item);
                        _collectingForTransformationTimer.SetZero();
                    }
                    else
                    {
                        _collectingForTransformationTimer.Tick();
                    }
                }
            }
        }

        void AddToTransformationQueue(Item item)
        {
            TweenHelper.KillAllTweens(item.transform);
            _neededResources[item.Definition] -= 1;

            if (_usePreTransformationQueue)
            {
                _itemsOnTransformationQueue.Add(item);
                Vector3 point = ArcadeIdleHelper.GetPoint(_itemsOnTransformationQueue.Count, _transformationQueueLayout);
                Transform trans = item.transform;
                trans.SetParent(_transformationQueue);
                TweenHelper.LocalJumpAndRotate(trans, point, Vector3.zero, _definition.JumpHeight, _definition.JumpDuration);
            }
            else
            {
                ItemUtil.JumpToDisappearIntoPool(item, _transformationPoint.position, _definition.JumpHeight, 1, _definition.JumpDuration);
            }
            
            ItemAdded?.Invoke(item.Definition, _neededResources[item.Definition]);

            // Try to start a transforming process
            foreach (KeyValuePair<ItemDefinition,int> neededResource in _neededResources)
            {
                if (neededResource.Value > 0)
                {
                    return;
                }
            }
            if (_neededResources[item.Definition] <= 0)
            {
                _transforming = true;

                if (_usePreTransformationQueue)
                {
                    foreach (Item val in _itemsOnTransformationQueue)
                    {
                        ItemUtil.JumpToDisappearIntoPool(val, _transformationPoint.position, _definition.JumpHeight, 1, _definition.JumpDuration);
                    }    
                    _itemsOnTransformationQueue.Clear();
                }
            }
        }

        void TryProduceOutput()
        {
            foreach (var neededResource in _neededResources)
            {
                if (neededResource.Value > 0)
                {
                    return;
                }
            }

            int j = 0;
            foreach (ItemDefinitionCountPair output in _definition.Ruleset.Outputs)
            {
                if (j++ != _alreadySpawnedOutputValue)
                {
                    continue;
                }

                for (int i = 0; i < output.Count; i++)
                {
                    Item p = output.ItemDefinition.Pool.Get();
                    p.transform.position = transform.position;

                    if(_alreadySpawnedOutputValue >= 4)
                        p.transform.localScale = p.transform.localScale * 0.33f;

                    _outputInventory.AddVisible(p);

                    if (SaveLoadService.instance != null)
                    {
                        SaveLoadService.instance.AddItemToData(p);
                        SaveLoadService.instance.DelayedSaveProgress();
                    }
                }    
            }

            if (_carData != null)
            {
                _carData.workBenchAlreadySpawnedCount = ++_alreadySpawnedOutputValue;
                //_inputArea.SetAlreadySpawnedOutputValue(_alreadySpawnedOutputValue);
            }

            //if (_countOutputValue >= _definition.Ruleset.Outputs.Length) _countOutputValue = 0;

            foreach (ItemDefinitionCountPair rulesetTypeCountPair in _definition.Ruleset.Inputs)
            {
                _neededResources[rulesetTypeCountPair.ItemDefinition] = rulesetTypeCountPair.Count;
            }
            ItemProduced?.Invoke();
        }
    }
}