using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;
using UnityEngine;

namespace ArcadeBridge
{
    public class AppodealTest : MonoBehaviour
    {
        private void Start()
        {
            int adTypes = AppodealAdType.Interstitial | AppodealAdType.Banner | AppodealAdType.RewardedVideo | AppodealAdType.Mrec;
            string appKey = "17bd909f221c8a33d00d1933661d5ddc70059d24f6f0faac";
            AppodealCallbacks.Sdk.OnInitialized += OnInitializationFinished;
            Appodeal.Initialize(appKey, adTypes);
        }

        public void OnInitializationFinished(object sender, SdkInitializedEventArgs e) { }
    }
}
