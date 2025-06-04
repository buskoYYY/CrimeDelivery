using ArcadeBridge.ArcadeIdleEngine.Interactables;
using UnityEngine;

namespace ArcadeBridge
{
    public class CheckProgressPump : MonoBehaviour
    {
        public void Init()
        {
            /*if (SaveLoadService.instance.PlayerProgress.isWheelsPumped)
            {
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
            else */

            GetComponent<Unlocker>().OnUnlockedInvoke();

            if (SaveLoadService.instance.PlayerProgress.isPumpCreated)
            {
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
            else
            {
                GetComponent<PumpSpawner>().ObjectForInteraction.gameObject.SetActive(false);
            }
        }

    }
}
