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

        public void Initialize(GameObject prefab, int index, RenderButtonSelectionManager manager)
        {
            carPrefab = prefab;
            selectionManager = manager;
            carIndex = index;

            GetComponent<Button>().onClick.AddListener(OnClick);

            SetSelected(false); // начальное состояние
        }

        private void OnClick()
        {
            selectionManager.OnButtonClicked(this, carPrefab, carIndex);
        }

        public void SetSelected(bool isSelected)
        {
            if (highlightBackground != null)
            {
                Color color = isSelected ? new Color(1f, 1f, 0f, highlightBackground.color.a) : new Color(0f, 0f, 0f, highlightBackground.color.a);
                highlightBackground.color = color;
            }

            if (selectedIcon != null)
            {
                selectedIcon.SetActive(isSelected);
            }
            FindObjectOfType<RenderButton>()?.OnCarButtonSelected(carPrefab);
        }
    }
}



