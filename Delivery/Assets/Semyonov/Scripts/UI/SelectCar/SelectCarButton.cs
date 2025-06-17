using Doozy.Engine.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ArcadeBridge
{
    public class SelectCarButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject _UiCarSelectionManager;
        [SerializeField] private UIView _uIView;

        public void Init(GameObject UiCarSelectionManager, UIView uIView)
        {
            _UiCarSelectionManager = UiCarSelectionManager;
            _uIView = uIView;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            _uIView.Hide();

            StartCoroutine(DelayedDestroyUICarSelectionManager());

            MainMenuLogic.Instance.Initialize();
        }

        private IEnumerator DelayedDestroyUICarSelectionManager()
        {
            yield return new WaitForSeconds(1f);

            Destroy(_UiCarSelectionManager.gameObject);
        }
    }
}
