using System;
using System.Collections;
using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Data.Database;
using ArcadeBridge.ArcadeIdleEngine.Items;
using ArcadeBridge.ArcadeIdleEngine.Storage;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Gathering
{
	public class Gatherer : MonoBehaviour
	{
		[SerializeField] GatheringToolDefinitionLookup gatheringToolDefinitionLookup;
		[SerializeField] Inventory _inventory;
		[SerializeField] Transform _playerHand;

		List<GatherableSource> _gatherableSources = new List<GatherableSource>();
		GatheringTool _activeGatheringTool;
		WaitForSeconds _delayedCollectWait = new WaitForSeconds(0.7f);

		public event Action<GatheringTool> Starting;
		public event Action Stopping;

		private int _gatheredTriggerIn;
        private void Start()
        {
			if (SaveLoadService.instance == null) return;

			List<Item> items = new List<Item>();

            foreach(ItemData itemData in SaveLoadService.instance.PlayerProgress.itemDatasInInventory)
            {
				Item item = StaticDataService.instance.GetItem(itemData.name);

				ClearCloneFromName.Clear(item);

				items.Add(Instantiate<Item>(item));
            }

			StartCoroutine(DelayedAddItem(items, false));
        }
        void Update()
		{
			if (!_inventory.Interactable && _gatheredTriggerIn == 0)
			{
				Stopping?.Invoke();
				if (_activeGatheringTool)
				{
                    Destroy(_activeGatheringTool.gameObject);
                    //_activeGatheringTool.enabled = false;
				}
				return;
			}

			int gatherableSourcesCount = _gatherableSources.Count;
			if (!_activeGatheringTool && gatherableSourcesCount > 0)
			{
				TryInstantiateTool(_gatherableSources[0]);
			}

			if (_activeGatheringTool)
			{
				_activeGatheringTool.enabled = true;
				Starting?.Invoke(_activeGatheringTool);
			}
			else
			{
				return;
			}

			bool hasNonDepletedSource = false; 
			foreach (GatherableSource source in _activeGatheringTool.GatherableSources)
			{
				if (!source.Depleted)
				{
					hasNonDepletedSource = true;
					break;
				}
			}
			if (!hasNonDepletedSource)
			{
				Stopping?.Invoke();
				if (_activeGatheringTool)
				{
					_activeGatheringTool.enabled = false;
				}
			}

			if (gatherableSourcesCount == 0)
			{
				return;
			}
			for (int i = gatherableSourcesCount - 1; i >= 0; i--)
			{
				GatherableSource gatherableSource = _gatherableSources[i];
				_activeGatheringTool.AddGatherable(gatherableSource);
				gatherableSource.GatheredItemInstantiated += GatherableSource_GatheredItemInstantiated;
				_gatherableSources.RemoveAt(i);
			}
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent(out GatherableSource gatherableSource))
			{
				_gatherableSources.Add(gatherableSource);
				_gatheredTriggerIn++;

				gatherableSource.OnSetActiveFalse += OnGatherableSourceExit;
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.TryGetComponent(out GatherableSource gatherableItemSource))
			{
				OnGatherableSourceExit(gatherableItemSource);
			}
		}
		private void OnGatherableSourceExit(GatherableSource gatherableItemSource)
		{
			gatherableItemSource.OnSetActiveFalse -= OnGatherableSourceExit;

			_gatheredTriggerIn--;
			_gatherableSources.Remove(gatherableItemSource);

			if (!_activeGatheringTool)
			{
				return;
			}

			_activeGatheringTool.RemoveGatherable(gatherableItemSource);
			gatherableItemSource.GatheredItemInstantiated -= GatherableSource_GatheredItemInstantiated;
			if (!_activeGatheringTool.HasInteractableGatherable)
			{
				Destroy(_activeGatheringTool.gameObject);
				_activeGatheringTool = null;
				Stopping?.Invoke();
			}
		}
		bool TryInstantiateTool(GatherableSource gatherableSource)
		{
			int highestTierIndex = -99999;
			int prefabIndex = -1;
			List<GatheringToolDefinition> availableObjects = gatheringToolDefinitionLookup.AvailableObjects;
			for (int i = 0; i < availableObjects.Count; i++)
			{
				GatheringToolDefinition tool = availableObjects[i];
				if (tool.CanGather(gatherableSource.GatherableDefinition) && tool.Tier > highestTierIndex)
				{
					highestTierIndex = tool.Tier;
					prefabIndex = i;
				}
			}

			if (prefabIndex != -1)
			{
				_activeGatheringTool = Instantiate(availableObjects[prefabIndex].GatheringToolPrefab, _playerHand);
				return true;
			}
			else
			{
				return false;
			}
		}

		void GatherableSource_GatheredItemInstantiated(List<Item> items)
		{
			StartCoroutine(DelayedAddItem(items));
		}

		IEnumerator DelayedAddItem(List<Item> items, bool withSave = true)
		{
			yield return _delayedCollectWait;

			foreach (Item item in items)
			{
				_inventory.Add(item);

                if (withSave)
				{
					SaveLoadService.instance.AddItemToData(item);
					SaveLoadService.instance.DelayedSaveProgress();
				}
			}

			items.Clear();
		}
	}
}
