using ArcadeBridge.ArcadeIdleEngine.Interactables;
using UnityEngine;

namespace ArcadeBridge
{
    public class CheckProgressPump : MonoBehaviour
    {
        private void Start()
        {
            if (SaveLoadService.instance.PlayerProgress.isWheelsPumped)
            {
                Destroy(gameObject);
            }
            else if (SaveLoadService.instance.PlayerProgress.isPumpCreated)
            {
                GetComponent<Unlocker>().OnUnlockedInvoke();
                Destroy(gameObject);
            }
        }

    }
}
