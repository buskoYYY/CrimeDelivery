using ArcadeBridge.ArcadeIdleEngine.Items;
using ArcadeBridge.ArcadeIdleEngine.Storage;
using System;
using TMPro;
using UnityEngine;

namespace ArcadeBridge
{
    public class DetailsView: MonoBehaviour
    {
        [SerializeField] private Inventory _inventory;
        [SerializeField] private TextMeshProUGUI _countDetailsText;

        private int _count;

        private void Awake()
        {
            _inventory.ItemAdded += CheckItem;
            _inventory.ItemRemoved += CheckItemRemove;
        }

        private void CheckItemRemove(Item item, int count)
        {
            ClearCloneFromName.Clear(item);

            if (item.name.Equals("Detail"))
            {
                _count --;
                SetText(_count);
            }
        }

        private void CheckItem(Item item, int count)
        {
            ClearCloneFromName.Clear(item);

            if (item.name.Equals("Detail"))
            {
                _count = count;
                SetText(_count);
            }
        }

        private void SetText(int count)
        {
            _countDetailsText.text = count.ToString();
        }
        private void OnDestroy()
        {
            if (_inventory)
            {
                _inventory.ItemAdded -= CheckItem;
                _inventory.ItemRemoved -= CheckItemRemove;
            }
        }
    }
}
