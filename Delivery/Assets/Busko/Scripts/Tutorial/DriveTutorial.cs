using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ArcadeBridge
{
    public class DriveTutorial : MonoBehaviour
    {
        //[SerializeField] private CollisionHandler _collisionHandler;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;

        private RaceLogic raceLogic;
        private PlayerUIController playerUIController;

        private int currentTutorialTrigger = -1;

        private void OnEnable()
        {
            //_collisionHandler.LeftTutorialOpend += OpenLeftButton;
            //_collisionHandler.RightTutorialOpend += OpenRightButton;
        }

        private void OnDisable()
        {
            raceLogic.OnRaceStartedEvent -= StartRace;
        }


        private void Start()
        {
            raceLogic = FindFirstObjectByType<RaceLogic>();
            playerUIController = FindFirstObjectByType<PlayerUIController>();
            raceLogic.OnRaceStartedEvent += StartRace;
        }


        private void StartRace(RaceData raceData)
        {
            playerUIController.turnLeft.gameObject.SetActive(false);
            playerUIController.turnRight.gameObject.SetActive(false);
        }

        public void SetTutorialStep(int stepIndex)
        {
            switch (stepIndex)
            {
                case 0: OpenLeftButton(); break;
                case 1: OpenRightButton(); break;
            }    
        }

        public void LeaveLeftTutorial()
        {
            TimeManager.Run();
            playerUIController.turnLeft.gameObject.SetActive(true);
            playerUIController.turnRight.gameObject.SetActive(true);

            _leftButton.gameObject.SetActive(false);
            _rightButton.gameObject.SetActive(false);
        }

        public void LeaveRightTutorial()
        {
            TimeManager.Run();
            playerUIController.turnLeft.gameObject.SetActive(true);
            playerUIController.turnRight.gameObject.SetActive(true);

            _leftButton.gameObject.SetActive(false);
            _rightButton.gameObject.SetActive(false);
        }

        private void OpenLeftButton()
        {
            if (currentTutorialTrigger < 0)
            {
                currentTutorialTrigger = 0;
                TimeManager.Pause();
                playerUIController.turnLeft.gameObject.SetActive(false);
                playerUIController.turnRight.gameObject.SetActive(false);

                _leftButton.gameObject.SetActive(true);
                _rightButton.gameObject.SetActive(false);
            }
        }

        private void OpenRightButton()
        {
            if (currentTutorialTrigger < 1)
            {
                currentTutorialTrigger = 1;
                TimeManager.Pause();
                playerUIController.turnLeft.gameObject.SetActive(false);
                playerUIController.turnRight.gameObject.SetActive(false);

                _leftButton.gameObject.SetActive(false);
                _rightButton.gameObject.SetActive(true);
            }

        }
    }
}
