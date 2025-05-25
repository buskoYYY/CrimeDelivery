using ArcadeBridge.ArcadeIdleEngine.Helpers;
using ArcadeBridge.ArcadeIdleEngine.Interactables;
using ArcadeBridge.ArcadeIdleEngine.Items;
using ArcadeBridge.ArcadeIdleEngine.Pools;
using ArcadeBridge.ArcadeIdleEngine.Storage;
using System;
using System.Collections;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Processors.Sellers
{
	[SelectionBase]
    public class SellerFloatingText : MonoBehaviour
	{
		public event Action<Item, bool> RemovingObjectFromInventoryWithSave;
		public SellerFloatingTextDefinition Definition => _definition;

		private int _carIndex;

		[SerializeField] private int itemCountForDeactivate;

		[SerializeField] SellerFloatingTextDefinition _definition;
		[SerializeField] Inventory _inventory;
		[SerializeField] Transform _sellingPoint;
		[SerializeField] Timer _timer;

		Camera _camera;
		
		void Awake()
		{
			_camera = Camera.main;
		}

        public void InitState(int carIndex)
        {
			_carIndex = carIndex;

			CarData carData = null;

			foreach (CarData data in SaveLoadService.instance.PlayerProgress.cunstructedCars)
            {
				if(data.index == carIndex)
                {
					carData = data;
                }
            }

			if(carData == null) return;

			foreach (ItemDefinition definition in _definition.SellableItemDefinitions)
            {
				Item item = definition.Pool.Item;

				foreach(CarDetail detail in carData.carDetails)
                {
                    if (item.name.Equals(detail.name))
                    {
						RemovingObjectFromInventoryWithSave?.Invoke(item, false);
						StartCoroutine(DestroyItemDelay());
					}
                }
			}
        }

        void Update()
		{
			if (_inventory.IsEmpty)
			{
				return;
			}
			// If inventory is empty do nothing. If it's not, then if it has an item that we can sell, increase the timer and sell it.
			foreach (ItemDefinition sellable in _definition.SellableItemDefinitions)
			{
				if (!_inventory.Contains(sellable, out Item result))
				{
					continue;
				}

				if (_timer.IsCompleted)
				{
					_inventory.Remove(result);

					if(SaveLoadService.instance != null)
						SaveLoadService.instance.RemoveFromData(result);

					RemovingObjectFromInventoryWithSave?.Invoke(result, true);

					TweenHelper.KillAllTweens(result.transform);
					TweenHelper.Jump(result.transform, _sellingPoint.position, _definition.JumpHeight, 1, _definition.JumpDuration, () => Sell(result));
					_timer.SetZero();
					StartCoroutine(DestroyItemDelay());
					return;
				}
				else
				{
					_timer.Tick();
				}
				return;
			}
		}
		
		void Sell(Item item)
		{
			item.ReleaseToPool();
			int itemSellValue = item.Definition.SellValue;
			_definition.IncomeResource.RuntimeValue += itemSellValue;
			//_definition.FloatingTextResourceAnimator.Play(transform, _camera.transform, itemSellValue);
		}

		IEnumerator DestroyItemDelay ()
		{
			yield return new WaitForSeconds(0.2f);
			itemCountForDeactivate--;

			if(itemCountForDeactivate == 0)
				gameObject.SetActive(false);
        }

	}
}
