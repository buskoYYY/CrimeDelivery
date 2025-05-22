using UnityEngine;
using UnityEngine.UI;

namespace ArcadeBridge
{
    public class DriveTutorial : MonoBehaviour
    {
        [SerializeField] private CollisionHandler _collisionHandler;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;
        [SerializeField] private Image _rigthtSteeringTutorial;
        [SerializeField] private Image _leftSteeringTutorial;


        private void OnEnable()
        {
            _collisionHandler.LeftTutorialOpend += OpenLeftButton;
            _collisionHandler.RightTutorialOpend += OpenRightButton;

        }

        private void OnDisable()
        {
            _collisionHandler.LeftTutorialOpend -= OpenLeftButton;
        }

        public void LeaveLeftTutorial()
        {
            TimeManager.Run();
            _leftSteeringTutorial.gameObject.SetActive(false);
        }

        public void LeaveRightTutorial()
        {
            TimeManager.Run();
            _rigthtSteeringTutorial.gameObject.SetActive(false);
            _leftButton.interactable = true;
        }

        private void OpenLeftButton()
        {
            TimeManager.Pause();
            _leftButton.gameObject.SetActive(true);
            _rightButton.gameObject.SetActive(true);
            _rightButton.interactable = false;
            _leftSteeringTutorial.gameObject.SetActive(true);

        }

        private void OpenRightButton()
        {
            TimeManager.Pause();
            _rightButton.interactable = true;
            _leftButton.interactable = false;
            _rigthtSteeringTutorial.gameObject.SetActive(true);
        }
    }
}
