using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace ArcadeBridge
{
    public class DeliveryButton : MonoBehaviour, IPointerClickHandler
    {
        private const string Garage = "Garage";

        [SerializeField] private MenuWindow _menu;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!SceneManager.GetActiveScene().name.Equals(Garage))
            {
                if (_menu)
                    Destroy(_menu.gameObject);
            }
            else
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}
