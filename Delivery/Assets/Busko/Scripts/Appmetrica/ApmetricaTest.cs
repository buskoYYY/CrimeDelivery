using UnityEngine;

namespace ArcadeBridge
{
    public class ApmetricaTest : MonoBehaviour
    {
        private void Start()
        {
            AppMetricaActivator.Activate();
            AnalyticsEvents.SendEvent("AppMetrica SDK is not initialized.");
        }
    }
}
