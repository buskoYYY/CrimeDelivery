using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeBridge
{
    public class RenderButtonItem : MonoBehaviour
    {
        [SerializeField] private Image highlightBackground;
        [SerializeField] private GameObject selectedIcon; // например, галочка

        private GameObject carPrefab;
        private RenderButtonSelectionManager selectionManager;
        private int carIndex;
        private RenderButton renderButton;

        public void Initialize(GameObject prefab, int index, RenderButtonSelectionManager manager, RenderButton renderButton)
        {
            carPrefab = prefab;
            selectionManager = manager;
            carIndex = index;
            this.renderButton = renderButton;
            GetComponent<Button>().onClick.AddListener(OnClick);

            SetSelected(false); // начальное состояние
        }

        private void OnClick()
        {
            selectionManager.OnButtonClicked(this, carPrefab, carIndex);
        }

        public void SetSelected(bool isSelected)
        {
            if (selectedIcon != null)
            {
                selectedIcon.SetActive(isSelected);
            }
            renderButton.OnCarButtonSelected(carPrefab);
        }
    }
}



