using ArcadeBridge.ArcadeIdleEngine.Interactables;
using UnityEngine;

namespace ArcadeBridge
{
    public class CheckProgressForWorkBench: MonoBehaviour
    {
        private void Start()
        {
            if (SaveLoadService.instance.PlayerProgress.isWorkBenchCreated)
            {
                GetComponent<Unlocker>().OnUnlockedInvoke();
                Destroy(gameObject);
            }
        }
    }
}
