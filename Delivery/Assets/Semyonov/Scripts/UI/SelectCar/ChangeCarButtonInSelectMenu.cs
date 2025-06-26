using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeBridge
{
    public class ChangeCarButtonInSelectMenu : MonoBehaviour
    {

        [SerializeField] private SelectCarButton _selectCarButton;
        [SerializeField] private UIView _UIView;
        [SerializeField] private UIButton _UIButton;
        [SerializeField] private Button _button;

        private void Start()
        {
            _UIButton.OnClick.PlayAnimation(_UIButton);

            _button.onClick.AddListener(CheckView);
        }
        private void CheckView()
        {
            if (_UIView.IsHiding)
            {
                _selectCarButton.StopDestroying();
                
                _UIView.Show();
            }
        }
    }
}
