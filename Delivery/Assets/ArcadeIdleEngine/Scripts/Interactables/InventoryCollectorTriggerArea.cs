using System.Collections;
using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Items;
using ArcadeBridge.ArcadeIdleEngine.Storage;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Interactables
{
	[SelectionBase]
	public class InventoryCollectorTriggerArea : MonoBehaviour
	{
		[SerializeField] ItemDefinitionCountPair[] _itemsToCollect;
		[SerializeField] Timer _collectingIntervalTimer;
		[SerializeField] Inventory _outputInventory;

		Dictionary<Inventory, Coroutine> _coroutineDictionary = new Dictionary<Inventory, Coroutine>();
		ItemDefinitionCountPair[] _currentItemsToCollect;

		private int _alreadySpawnedCount;
		//[SerializeField] private bool _ignoreLimitOnInputItem;
		void Awake()
		{
			_currentItemsToCollect = new ItemDefinitionCountPair[_itemsToCollect.Length];
			for (int i = 0; i < _currentItemsToCollect.Length; i++)
			{
				_currentItemsToCollect[i].ItemDefinition = _itemsToCollect[i].ItemDefinition;
				_currentItemsToCollect[i].Count = _itemsToCollect[i].Count;
			}
		}
		public void SetAlreadySpawnedCount(int count)
        {
			_alreadySpawnedCount = count;
		}
		void OnTriggerEnter(Collider other)
		{
			/*if (SequenceOfActivities.Instance != null
				&& SequenceOfActivities.Instance.GameFactory.ConstructedCar.ConstructedDetailsCount < _alreadySpawnedCount)
			{
				return;
			}*/
			if (other.TryGetComponent(out Inventory inventory))
			{
				_coroutineDictionary.Add(inventory, StartCoroutine(Co_Collect(inventory)));
			}
		}
		
		void OnTriggerExit(Collider other)
		{
			if (other.TryGetComponent(out Inventory inventory))
			{
				if(_coroutineDictionary.TryGetValue(inventory, out Coroutine coroutine))
                {
					StopCoroutine(coroutine);
					_coroutineDictionary.Remove(inventory);
					_collectingIntervalTimer.SetZero();
				}
				//StopCoroutine(_coroutineDictionary[inventory]);
			}
		}
		/*int _getItemCount;
		public void ItemPlaced()
        {
			_getItemCount = 0;
        }*/
		IEnumerator Co_Collect(Inventory inventory)
		{
			while (true)
			{
				if (_currentItemsToCollect.Length == 0 || !inventory.Interactable || _outputInventory.IsVisibleFull)
				{
					yield return null;
					continue;
				}

				if (_collectingIntervalTimer.IsCompleted)
				{
					/*if (!_ignoreLimitOnInputItem && _getItemCount > 0)
					{
						yield break;
					}
*/
					//_getItemCount++;
					for (int i = 0; i < _currentItemsToCollect.Length; i++)
					{
						if ((_itemsToCollect[i].Count > 0 && _currentItemsToCollect[i].Count <= 0) || !inventory.Contains(_currentItemsToCollect[i].ItemDefinition, out Item item))
						{
							continue;
						}

						_currentItemsToCollect[i].Count--;
						inventory.Remove(item);
						_outputInventory.Add(item);
						_collectingIntervalTimer.SetZero();

						bool shouldRefresh = true;
						foreach (ItemDefinitionCountPair currentItem in _currentItemsToCollect)
						{
							if (currentItem.Count > 0)
							{
								shouldRefresh = false;
							}
						}

						if (shouldRefresh)
						{
							for (int index = 0; index < _currentItemsToCollect.Length; index++)
							{
								_currentItemsToCollect[index].Count = _itemsToCollect[index].Count;
							}
						}
						break;
					}
				}
				else
				{
					_collectingIntervalTimer.Tick();
				}

				yield return null;
			}
		}
	}
}
