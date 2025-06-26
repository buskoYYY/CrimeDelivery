using Doozy.Engine.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeBridge
{
    public class SelectCarButtonInSelectMenu : MonoBehaviour
    {
        [SerializeField] private UIButton _UIButtonThis;
        [SerializeField] private Button[] _buttons;
        [SerializeField] private CanvasGroup[] _canvasGroups;
        [SerializeField] private bool _animationOnStart;
        void Start()
        {
            if(_animationOnStart)
                _UIButtonThis.OnClick.PlayAnimation(_UIButtonThis);

            _UIButtonThis.GetComponent<Button>().onClick.AddListener(DeactivateButton);
        }

        private void DeactivateButton()
        {
            _UIButtonThis.OnClick.PlayAnimation(_UIButtonThis);

            for (int i =0; i < _buttons.Length; i++)
            {
                _canvasGroups[i].interactable = false;
                _canvasGroups[i].blocksRaycasts = false;
                _buttons[i].interactable = false;
                _buttons[i].enabled = false;
            }
        }
    }
}
