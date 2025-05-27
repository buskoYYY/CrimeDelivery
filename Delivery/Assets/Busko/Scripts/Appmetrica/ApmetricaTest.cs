using Io.AppMetrica;
using UnityEngine;

namespace ArcadeBridge
{
    public class ApmetricaTest : MonoBehaviour
    {
        private void Start()
        {
            if (AppMetrica.IsActivated())
            {
                AnalyticsEvents.SendEvent("AppMetrica SDK is initialized.");
            }
            else
            {
                Debug.LogError("AppMetrica SDK is not activated. Event will not be sent.");
            }
        }
    }
}
