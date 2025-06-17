using UnityEngine;
using UnityEngine.EventSystems;

namespace ArcadeBridge
{
    public class CarsMenuButton : MonoBehaviour, IPointerClickHandler
    {
        //[SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject _UiCarSelectionManagerPrefab;

        public void OnPointerClick(PointerEventData eventData)
        {
            Instantiate(_UiCarSelectionManagerPrefab);
        }
    }
}
