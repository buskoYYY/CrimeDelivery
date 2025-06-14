using ArcadeBridge.ArcadeIdleEngine.Actors;
using System;
using UnityEngine;

namespace ArcadeBridge
{
    public class ConstructionCanvas : MonoBehaviour
    {
        [SerializeField] private MenuWindow _menuPrefab;
        [SerializeField] private FinishConstructionWindow _finishConstructionWindowPrefab;
        [SerializeField] private UIJoystick _uIJoystick;

        private FinishConstructionWindow _finishConstructionWindow;
        private MenuWindow _menu;

        public static ConstructionCanvas Instance { get; private set; }
        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        public void ShowMenu()
        {
            if (_menu)
            {
                Destroy(_menu.gameObject);
                return;
            }

            _uIJoystick.gameObject.SetActive(false);

            _menu = Instantiate(_menuPrefab, transform);

            _menu.OnDestroyInvoked += OnWindowDestroy;
        }
        public void ShowFinishWindow()
        {
            _uIJoystick.gameObject.SetActive(false);

            _finishConstructionWindow = Instantiate(_finishConstructionWindowPrefab, transform);

            _finishConstructionWindow.OnDestroyInvoked += OnWindowDestroy;
        }

        private void OnWindowDestroy()
        {
            if(_menu)
                _menu.OnDestroyInvoked -= OnWindowDestroy;

            if(_finishConstructionWindow)
                 _finishConstructionWindow.OnDestroyInvoked -= OnWindowDestroy;

            _uIJoystick.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            if (_menu)
                _menu.OnDestroyInvoked -= OnWindowDestroy;

            if (_finishConstructionWindow)
                _finishConstructionWindow.OnDestroyInvoked -= OnWindowDestroy;

        }
    }
}
