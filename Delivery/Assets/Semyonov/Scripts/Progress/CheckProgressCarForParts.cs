using ArcadeBridge.ArcadeIdleEngine.Interactables;
using UnityEngine;

namespace ArcadeBridge
{
    public class CheckProgressCarForParts : MonoBehaviour
    {
        public void Init()
        {
            /*if (SaveLoadService.instance.PlayerProgress.isCarForPartsBrokenAbsolutly)
            {
                Destroy(gameObject);
            }
            else */
            if (SaveLoadService.instance.PlayerProgress.isCarForPartsCreated)
            {
                GetComponent<Unlocker>().OnUnlockedInvoke();
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
        }

    }
}
