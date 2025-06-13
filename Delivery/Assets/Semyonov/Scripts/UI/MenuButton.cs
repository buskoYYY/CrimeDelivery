using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace ArcadeBridge
{
    public class MenuButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ConstructionCanvas _constructionCanvas;
        public void OnPointerClick(PointerEventData eventData)
        {
            _constructionCanvas.ShowMenu();
        }
    }
}
