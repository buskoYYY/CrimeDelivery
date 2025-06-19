using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeBridge
{
    public class ViewCunstructedCar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform spawnParent;
        [SerializeField] private RawImage previewImage;
        [SerializeField] private float rotationSpeed = 20f;

        [Header("Render Settings")]
        [SerializeField] private Camera renderCamera;
        [SerializeField] private Vector3 spawnPosition = Vector3.zero;
        [SerializeField] private Vector3 spawnRotation = new Vector3(0, 15, 0);
        [SerializeField] private string renderLayer = "CarPreview";

        [Header("UI Settings")]
        [SerializeField] private TMP_Text accelText;
        [SerializeField] private TMP_Text topSpeedText;
        [SerializeField] private TMP_Text rotateText;
        [SerializeField] private TMP_Text rotVelText;


        [SerializeField] private CarsDatabase _carsDatabase;
        private GameObject currentCarInstance;
        private RenderTexture renderTexture;
        private int currentCarIndex = -1;

        public void PreviewCar()
        {
            previewImage.gameObject.SetActive(true);
            InitializeRenderTexture();
        }

        private void InitializeRenderTexture()
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
            }

            renderTexture = new RenderTexture(512, 512, 24);
            renderCamera.targetTexture = renderTexture;
            previewImage.texture = renderTexture;
        }

        public void ShowCarPreview(int carConstructingIndex)
        {
            // Удаляем предыдущую машину
            if (currentCarInstance != null)
            {
                Destroy(currentCarInstance);
            }

            CarConfig carConfig = _carsDatabase.carsConfigs.Find(car => car.constructionID == carConstructingIndex);
            GameObject car = carConfig.gameObject;

            // Проверяем валидность индекса
            if (carConstructingIndex < 0 
                || carConstructingIndex >= _carsDatabase.carsConfigs.Count
                || car == null)
            {
                Debug.LogError("Invalid car index!");
                return;
            }


            // Создаем новую машину
            currentCarInstance = Instantiate(car, spawnParent);

            if (!currentCarInstance.GetComponent<Rigidbody>())
                currentCarInstance.AddComponent<Rigidbody>();

            currentCarInstance.GetComponent<Rigidbody>().isKinematic = true;

            currentCarInstance.transform.localPosition = spawnPosition;
            currentCarInstance.transform.localRotation = Quaternion.Euler(spawnRotation);

            // Устанавливаем слой для рендеринга
            SetLayerRecursively(currentCarInstance.transform, renderLayer);
            currentCarIndex = _carsDatabase.carsConfigs.IndexOf(carConfig);

            // Активируем камеру
            renderCamera.gameObject.SetActive(true);
        }
        public void UpdateCarStatsUI()
        {
            if (currentCarIndex < 0 || currentCarIndex >= _carsDatabase.carsConfigs.Count)
            {
                Debug.LogWarning("Invalid car index");
                return;
            }

            // Получаем компонент CarConfig
            var carConfig = _carsDatabase.carsConfigs[currentCarIndex].GetComponent<CarConfig>();
            if (carConfig == null)
            {
                Debug.LogWarning($"CarConfig not found on {_carsDatabase.carsConfigs[currentCarIndex].name}");
                return;
            }

            // Проверяем CarSettings
            if (carConfig.carSettings == null)
            {
                Debug.LogWarning($"CarSettings not assigned in CarConfig on {_carsDatabase.carsConfigs[currentCarIndex].name}");
                return;
            }

            // Проверяем DriftControllerSettings
            if (carConfig.carSettings.driftControllerSettings == null)
            {
                Debug.LogWarning($"DriftControllerSettings not assigned in CarSettings on {_carsDatabase.carsConfigs[currentCarIndex].name}");
                return;
            }

            var driftSettings = carConfig.carSettings.driftControllerSettings;

            // Обновляем UI
            SetTextIfNotNull(accelText, driftSettings.Accel.ToString("F1"));
            SetTextIfNotNull(topSpeedText, driftSettings.TopSpeed.ToString("F1"));
            SetTextIfNotNull(rotateText, driftSettings.RotateAtStart.ToString("F1"));
            SetTextIfNotNull(rotVelText, driftSettings.RotVel.ToString("F1"));

            // Дополнительно можно вывести narrative name
            Debug.Log($"Loaded car: {carConfig.carSettings.carNarrativeName}");
        }

        private void SetTextIfNotNull(TMP_Text textField, string value)
        {
            if (textField != null)
                textField.text = value;
        }

        private void Update()
        {
            // Вращаем модель
            if (currentCarInstance != null)
            {
                currentCarInstance.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
                //UpdateCarStatsUI();
            }
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

        private void OnDestroy()
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
            }
        }
    }
}
