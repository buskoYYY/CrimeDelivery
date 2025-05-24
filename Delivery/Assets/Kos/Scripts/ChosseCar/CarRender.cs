using UnityEngine;
using UnityEngine.UI;

namespace ArcadeBridge
{
    public class CarRender : MonoBehaviour
    {
        [Header("Render Settings")]
        [SerializeField] private RawImage targetRawImage; // Ссылка на UI RawImage
        [SerializeField] private Camera renderCamera; // Готовая камера из префаба

        private RenderTexture _renderTexture;

        void Start()
        {
            RendCar ();
        }

        private void RendCar ()
        {
            if (targetRawImage == null || renderCamera == null)
            {
                Debug.LogError("Target RawImage or Render Camera is not assigned!");
                return;
            }

            // Создаем Render Texture
            _renderTexture = new RenderTexture(1080, 1080, 24);
            _renderTexture.Create();

            // Назначаем Render Texture
            renderCamera.targetTexture = _renderTexture;
            targetRawImage.texture = _renderTexture;
        }
    }
}
