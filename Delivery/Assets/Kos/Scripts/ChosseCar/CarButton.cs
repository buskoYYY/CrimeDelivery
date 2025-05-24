using UnityEngine;
using UnityEngine.UI;

namespace ArcadeBridge
{
    public class CarButton : MonoBehaviour
    {
        [Header("Render Settings")]
        [SerializeField] private RawImage carRenderImage;

        private int _carIndex;
        private CarSelectionManager _carSelectionManager;
        private CarRenderManager _carRenderManager;

        public void Initialize(GameObject carPrefab, int index, CarSelectionManager manager, CarRenderManager renderManager)
        {
            _carIndex = index;
            _carSelectionManager = manager;
            _carRenderManager = renderManager;

            StartCoroutine(_carRenderManager.RenderCarImage(carPrefab, texture =>
            {
                carRenderImage.texture = texture;
            }));
        }

        public void OnClick()
        {
            _carSelectionManager.SelectCar(_carIndex);
        }
    }
}


