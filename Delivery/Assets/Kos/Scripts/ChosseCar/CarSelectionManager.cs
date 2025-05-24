using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace ArcadeBridge
{
    public class CarSelectionManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CarRenderManager carRenderManager; 

        [Header("UI Elements")]
        [SerializeField] private List<GameObject> prefabCar;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private ScrollRect scrollPanel;
        [SerializeField] private GridLayoutGroup gridLayout;
        [SerializeField] private GameObject renderSelectedCar;

        [Header("Grid Settings")]
        [SerializeField] private int columns = 3;
        [SerializeField] private Vector2 spacing = new Vector2(20, 20);
        [SerializeField] private Vector2 buttonSize = new Vector2(200, 200);
        [SerializeField] private int paddingLeft = 20;
        [SerializeField] private int paddingRight = 20;
        [SerializeField] private int paddingTop = 20;
        [SerializeField] private int paddingBottom = 20;

        private List<GameObject> _carButtons = new List<GameObject>();
        private List<GameObject> _availableCars = new List<GameObject>();
        private RectOffset _padding;
        private int _selectedCarIndex = -1;
        private GameObject _lastSelectedButton;
        private int _pendingRenders;

        private void Awake()
        {
            _padding = new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);
        }

        private void Start()
        {
            InitializeCarList();
            StartCoroutine(CreateCarSelectionGrid());
        }

        private void InitializeCarList()
        {
            _availableCars = new List<GameObject>(prefabCar);
        }

        private IEnumerator CreateCarSelectionGrid()
        {
            if (buttonPrefab == null || scrollPanel == null || gridLayout == null)
            {
                Debug.LogError("Required references are not assigned!");
                yield break;
            }

            ClearExistingButtons();

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = columns;
            gridLayout.spacing = spacing;
            gridLayout.cellSize = buttonSize;
            gridLayout.padding = _padding;

            RectTransform content = scrollPanel.content;
            float panelWidth = scrollPanel.viewport.rect.width;
            content.sizeDelta = new Vector2(panelWidth, 0);

            _pendingRenders = _availableCars.Count;

            for (int i = 0; i < _availableCars.Count; i++)
            {
                GameObject carPrefab = _availableCars[i];
                int index = i;

                GameObject button = Instantiate(buttonPrefab, gridLayout.transform);
                _carButtons.Add(button);

                RawImage rawImage = button.GetComponentInChildren<RawImage>();
                if (rawImage == null)
                {
                    Debug.LogWarning("RawImage not found in button prefab");
                    _pendingRenders--;
                    continue;
                }

                CarButton carButton = button.GetComponent<CarButton>();
                if (carButton != null)
                {
                    carButton.Initialize(carPrefab, index, this, carRenderManager);
                }

                yield return carRenderManager.RenderCarImage(carPrefab, texture =>
                {
                    rawImage.texture = texture;
                    _pendingRenders--;

                    if (_pendingRenders == 0)
                    {
                        carRenderManager.DisposeRenderCamera();
                    }
                });
            }

            UpdateContentHeight();
        }

        private void UpdateContentHeight()
        {
            if (gridLayout == null) return;

            int rowCount = Mathf.CeilToInt((float)_availableCars.Count / columns);
            float totalHeight = rowCount * (buttonSize.y + spacing.y)
                             + _padding.top + _padding.bottom;

            scrollPanel.content.sizeDelta = new Vector2(
                scrollPanel.content.sizeDelta.x,
                totalHeight
            );
        }

        private void ClearExistingButtons()
        {
            foreach (GameObject button in _carButtons)
            {
                if (button != null) Destroy(button);
            }
            _carButtons.Clear();
        }

        public void SelectCar(int carIndex)
        {
            if (carIndex < 0 || carIndex >= _availableCars.Count)
                return;

            Debug.Log($"Selected car: {_availableCars[carIndex].name}");

            foreach (Transform child in renderSelectedCar.transform)
            {
                Destroy(child.gameObject);
            }

            GameObject carInstance = Instantiate(_availableCars[carIndex], renderSelectedCar.transform);

            carInstance.transform.localPosition = Vector3.zero;
            carInstance.transform.localRotation = Quaternion.identity;
            carInstance.transform.localScale = Vector3.one;

            HighlightSelectedButton(_carButtons[carIndex]);
        }

        private void HighlightSelectedButton(GameObject newButton)
        {
            if (_lastSelectedButton != null)
            {
                Image lastImage = _lastSelectedButton.GetComponent<Image>();
                if (lastImage != null)
                {
                    Color baseColor = Color.black;
                    baseColor.a = lastImage.color.a;
                    lastImage.color = baseColor;

                    Transform checkmark = _lastSelectedButton.transform.Find("Checkmark");
                    Transform indicator = _lastSelectedButton.transform.Find("SelectedIndicator");
                    if (checkmark != null) checkmark.gameObject.SetActive(false);
                    if (indicator != null) indicator.gameObject.SetActive(false);
                }
            }

            Image newImage = newButton.GetComponent<Image>();
            if (newImage != null)
            {
                Color selectedColor = Color.yellow;
                selectedColor.a = newImage.color.a;
                newImage.color = selectedColor;

                Transform checkmark = newButton.transform.Find("Checkmark");
                Transform selectedIndicator = newButton.transform.Find("SelectedIndicator");
                if (checkmark != null) checkmark.gameObject.SetActive(true);
                if (selectedIndicator != null) selectedIndicator.gameObject.SetActive(true);
            }

            _lastSelectedButton = newButton;
        }
    }
}



