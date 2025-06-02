using ArcadeBridge.ArcadeIdleEngine.Interactables;
using UnityEngine;

namespace ArcadeBridge
{
    public class CheckProgressCarForParts : MonoBehaviour
    {
        private void Start()
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
