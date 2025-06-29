using Doozy.Engine.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ArcadeBridge
{
    public class SelectCarButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _backgroundThis;
        [SerializeField] private Image _backgroundChangeCar;
        [SerializeField] private Color _selected;
        [SerializeField] private Color _base;
        [SerializeField] private GameObject _UiCarSelectionManager;
        [SerializeField] private UIView _uIView;

        private bool _invoked;

        private Coroutine _destroyCoroutine;
        public void Init(GameObject UiCarSelectionManager, UIView uIView)
        {
            _UiCarSelectionManager = UiCarSelectionManager;
            _uIView = uIView;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_invoked) return;

            _backgroundChangeCar.color = _base;

            _backgroundThis.color = _selected;

            _uIView.Hide();

            _destroyCoroutine = StartCoroutine(DelayedDestroyUICarSelectionManager());

            MainMenuLogic.Instance.ChangePlayerCar();

            _invoked = true;
        }
        public void StopDestroying()
        {
            StopCoroutine(_destroyCoroutine);

            _backgroundChangeCar.color = _selected;

            _backgroundThis.color = _base;

            _invoked = false;
        }
        private IEnumerator DelayedDestroyUICarSelectionManager()
        {
            yield return new WaitForSeconds(.4f);

            Destroy(_UiCarSelectionManager.gameObject);
        }
    }
}
