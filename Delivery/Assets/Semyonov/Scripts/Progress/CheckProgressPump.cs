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
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
            else if (SaveLoadService.instance.PlayerProgress.isPumpCreated)
            {
                GetComponent<Unlocker>().OnUnlockedInvoke();
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
        }

    }
}
