using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeBridge
{
    public class RenderButton : MonoBehaviour
    {
        [Header("Car Prefabs")]
        [SerializeField] private List<GameObject> testCar;
        [SerializeField] private AvailableCars availableCars;

        [Header("UI")]
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform contentParent;
        [SerializeField] private RawImage selectedCarPreviewImage;

        [Header("Grid Layout Settings")]
        [SerializeField] private int columns = 3;
        [SerializeField] private Vector2 cellSize = new Vector2(200, 200);
        [SerializeField] private Vector2 spacing = new Vector2(10, 10);
        [SerializeField] private RectOffset padding;

        [Header("Render Setup")]
        [SerializeField] private Camera renderCamera;
        [SerializeField] private Transform renderRoot;
        [SerializeField] private int textureSize = 512;
        [SerializeField] private string renderLayer = "CarPreview";

        [SerializeField] private RenderButtonSelectionManager selectionManager;
        public List<GameObject> prefabCar = new List<GameObject>();
        //private GameObject currentSelectedCarInstance;

        private void Start()
        {
            InitializeCarList();
            //SetupGridLayout();
            StartCoroutine(RenderAllCarButtons());
        }

        private void InitializeCarList()
        {
            //prefabCar = new List<GameObject>(testCar);
            prefabCar = new List<GameObject>(availableCars.defaultCars);
        }

        //private void SetupGridLayout()
        //{
        //    GridLayoutGroup grid = contentParent.GetComponent<GridLayoutGroup>();
        //    if (grid == null)
        //    {
        //        grid = contentParent.gameObject.AddComponent<GridLayoutGroup>();
        //    }

        //    grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        //    grid.constraintCount = columns;
        //    grid.cellSize = cellSize;
        //    grid.spacing = spacing;
        //    grid.padding = padding;
        //    grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        //    grid.childAlignment = TextAnchor.UpperLeft;
        //}

        private IEnumerator RenderAllCarButtons()
        {
            renderCamera.gameObject.SetActive(true);

            for (int i = 0; i < prefabCar.Count; i++)
            {
                GameObject carPrefab = prefabCar[i];

                // 1. Создаём кнопку
                GameObject buttonGO = Instantiate(buttonPrefab, contentParent);
                buttonGO.name = carPrefab.name;

                RawImage img = buttonGO.GetComponentInChildren<RawImage>();
                if (img == null)
                {
                    Debug.LogError("RawImage not found in button prefab!");
                    continue;
                }

                // 2. Спавним машину
                GameObject carInstance = Instantiate(carPrefab, renderRoot);
                carInstance.transform.localPosition = Vector3.zero;
                carInstance.transform.localRotation = Quaternion.Euler(0, 15, 0);
                carInstance.transform.localScale = Vector3.one;
                SetLayerRecursively(carInstance.transform, renderLayer);

                // 3. Подождать кадр
                yield return null;
                yield return new WaitForEndOfFrame();

                // 4. Настройка прозрачности
                renderCamera.clearFlags = CameraClearFlags.SolidColor;
                renderCamera.backgroundColor = new Color(0, 0, 0, 0);

                RenderTexture rt = new RenderTexture(textureSize, textureSize, 24, RenderTextureFormat.ARGB32);
                rt.useMipMap = false;
                rt.autoGenerateMips = false;

                renderCamera.targetTexture = rt;
                renderCamera.Render();

                RenderTexture.active = rt;

                Texture2D tex = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
                tex.ReadPixels(new Rect(0, 0, textureSize, textureSize), 0, 0);
                tex.Apply();

                RenderTexture.active = null;
                renderCamera.targetTexture = null;
                Destroy(rt);

                // 5. Присваиваем текстуру кнопке
                img.texture = tex;
                img.color = Color.white;

                // 6. Удалить объект
                Destroy(carInstance);

                // 7. Инициализировать кнопку
                RenderButtonItem item = buttonGO.GetComponent<RenderButtonItem>();
                if (item != null)
                {
                    item.Initialize(carPrefab, i, selectionManager);
                }
                else
                {
                    Debug.LogWarning("RenderButtonItem component not found on button prefab.");
                }

                yield return null;
            }
            
            renderCamera.gameObject.SetActive(false);
        }

        private void SetLayerRecursively(Transform obj, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            obj.gameObject.layer = layer;
            foreach (Transform child in obj)
            {
                SetLayerRecursively(child, layerName);
            }
        }



        public void OnCarButtonSelected(GameObject selectedCarPrefab)
        {
            Debug.Log("Выбрана машина: " + selectedCarPrefab.name);

            // Можешь также передать выбранную машину в другой менеджер
        }
    }
}








