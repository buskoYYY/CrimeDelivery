using ArcadeBridge;
using UnityEngine;
public class TutorialTrigger : MonoBehaviour
{
    public DriveTutorial driveTutorial;
    public int tutorialIndex = 0;

    private void Start()
    {
        driveTutorial = FindFirstObjectByType<DriveTutorial>();
    }

    private void OnTriggerEnter(Collider other)
    {
        CarComponentsController player = other.GetComponentInParent<CarComponentsController>();
        if (player != null)
        {
            if (player.isPlayer)
            {
                driveTutorial.SetTutorialStep(tutorialIndex);
            }
        }
    }
}
