using ArcadeBridge.ArcadeIdleEngine.Interactables;
using UnityEngine;

namespace ArcadeBridge
{
    public class CheckProgressForWorkBench: MonoBehaviour
    {
        public void Init()
        {
            GetComponent<Unlocker>().OnUnlockedInvoke();

            if (SaveLoadService.instance.PlayerProgress.isWorkBenchCreated)
            {
                gameObject.SetActive(false);
            }
            else
            {
                GetComponent<WorkBenchSpawner>().ObjectForInteraction.gameObject.SetActive(false);
            }
        }
    }
}
