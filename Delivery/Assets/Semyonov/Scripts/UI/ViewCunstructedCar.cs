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


        [SerializeField] private List<GameObject> cars = new List<GameObject>();
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

        public void ShowCarPreview(int carIndex)
        {
            // ������� ���������� ������
            if (currentCarInstance != null)
            {
                Destroy(currentCarInstance);
            }

            // ��������� ���������� �������
            if (carIndex < 0 || carIndex >= cars.Count)
            {
                Debug.LogError("Invalid car index!");
                return;
            }

            // ������� ����� ������
            currentCarInstance = Instantiate(cars[carIndex], spawnParent);
            currentCarInstance.transform.localPosition = spawnPosition;
            currentCarInstance.transform.localRotation = Quaternion.Euler(spawnRotation);

            // ������������� ���� ��� ����������
            SetLayerRecursively(currentCarInstance.transform, renderLayer);
            currentCarIndex = carIndex;

            // ���������� ������
            renderCamera.gameObject.SetActive(true);
        }
        public void UpdateCarStatsUI()
        {
            if (currentCarIndex < 0 || currentCarIndex >= cars.Count)
            {
                Debug.LogWarning("Invalid car index");
                return;
            }

            // �������� ��������� CarConfig
            var carConfig = cars[currentCarIndex].GetComponent<CarConfig>();
            if (carConfig == null)
            {
                Debug.LogWarning($"CarConfig not found on {cars[currentCarIndex].name}");
                return;
            }

            // ��������� CarSettings
            if (carConfig.carSettings == null)
            {
                Debug.LogWarning($"CarSettings not assigned in CarConfig on {cars[currentCarIndex].name}");
                return;
            }

            // ��������� DriftControllerSettings
            if (carConfig.carSettings.driftControllerSettings == null)
            {
                Debug.LogWarning($"DriftControllerSettings not assigned in CarSettings on {cars[currentCarIndex].name}");
                return;
            }

            var driftSettings = carConfig.carSettings.driftControllerSettings;

            // ��������� UI
            SetTextIfNotNull(accelText, driftSettings.Accel.ToString("F1"));
            SetTextIfNotNull(topSpeedText, driftSettings.TopSpeed.ToString("F1"));
            SetTextIfNotNull(rotateText, driftSettings.RotateAtStart.ToString("F1"));
            SetTextIfNotNull(rotVelText, driftSettings.RotVel.ToString("F1"));

            // ������������� ����� ������� narrative name
            Debug.Log($"Loaded car: {carConfig.carSettings.carNarrativeName}");
        }

        private void SetTextIfNotNull(TMP_Text textField, string value)
        {
            if (textField != null)
                textField.text = value;
        }

        private void Update()
        {
            // ������� ������
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
