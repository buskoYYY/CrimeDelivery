using UnityEngine;
using UnityEngine.EventSystems;

namespace ArcadeBridge
{
    public class CarsMenuButton : MonoBehaviour, IPointerClickHandler
    {
        //[SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject _UiCarSelectionManagerPrefab;
        private GameObject _UiCarSelectionManager;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_UiCarSelectionManager)
                Destroy(_UiCarSelectionManager.gameObject);

            _UiCarSelectionManager = Instantiate(_UiCarSelectionManagerPrefab);
        }
    }
}
