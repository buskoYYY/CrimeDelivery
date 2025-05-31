using ArcadeBridge.ArcadeIdleEngine.Actors;
using System;
using UnityEngine;

namespace ArcadeBridge
{
    public class ConstructionCanvas : MonoBehaviour
    {
        [SerializeField] private FinishConstructionWindow _finishConstructionWindowPrefab;
        [SerializeField] private UIJoystick _uIJoystick;

        private FinishConstructionWindow _finishConstructionWindow;

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
        public void ShowFinishWindow()
        {
            _uIJoystick.gameObject.SetActive(false);

            _finishConstructionWindow = Instantiate(_finishConstructionWindowPrefab, transform);

            _finishConstructionWindow.OnDestroyInvoked += OnFinishWindowDestroy;
        }

        private void OnFinishWindowDestroy()
        {
            _uIJoystick.gameObject.SetActive(true);
        }

        /*bool isF;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!isF)
                {
                    isF = true;
                    ShowFinishWindow();
                }
                else
                {
                    isF = false;
                    _uIJoystick.gameObject.SetActive(true);
                    Destroy(_finishConstructionWindow);
                }
            }
        }*/
    }
}
