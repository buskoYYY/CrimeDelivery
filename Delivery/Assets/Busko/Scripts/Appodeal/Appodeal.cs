using AppodealStack.Monetization.Common;
using UnityEngine;

namespace ArcadeBridge
{
    public class Appodeal : MonoBehaviour, IRewardedVideoAdListener
    {
        private const string _appkey = "17bd909f221c8a33d00d1933661d5ddc70059d24f6f0faac";

        public static Appodeal Instance;

        [SerializeField] private int _timeDelay = 30;
        [SerializeField] private int _repeatRate = 30;

        private int _score;
        private int _adTypes;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); 
            }
            else
            {
                Destroy(gameObject); 
            }
        }

        private void Start()
        {
            _adTypes = AppodealAdType.Interstitial | AppodealAdType.Banner | AppodealAdType.RewardedVideo | AppodealAdType.Mrec;
            AppodealCallbacks.Sdk.OnInitialized += OnInitializationFinished;
            Initialized();
            InvokeRepeating("ShowInterstitialAds", 30f, 30f);
        }

        public void ShowRewardAds() 
        {
            if (AppodealStack.Monetization.Api.Appodeal.IsLoaded(AppodealAdType.RewardedVideo))
            {
                AppodealStack.Monetization.Api.Appodeal.Show(AppodealShowStyle.RewardedVideo);
            }
        }

        public int AddScore(int points)
        {
            _score += points;
            return _score;
        }

        public void OnInitializationFinished(object sender, SdkInitializedEventArgs e) {}

        private void Initialized()
        {
            AppodealStack.Monetization.Api.Appodeal.Initialize(_appkey, _adTypes);

            AppodealStack.Monetization.Api.Appodeal.MuteVideosIfCallsMuted(true);

           AppodealStack.Monetization.Api.Appodeal.SetRewardedVideoCallbacks(this);
        }

        private void ShowInterstitialAds()
        {
            if (AppodealStack.Monetization.Api.Appodeal.CanShow(AppodealAdType.Interstitial))
            {
                AppodealStack.Monetization.Api.Appodeal.Show(AppodealAdType.Interstitial);
            }
        }

        #region RewardedVideoCallback
        public void OnRewardedVideoLoaded(bool isPrecache)
        {
            print("Video loaded");
        }

        public void OnRewardedVideoFailedToLoad()
        {
            print("Video failed");
        }

        public void OnRewardedVideoShowFailed()
        {
            print("Video show failed");
        }

        public void OnRewardedVideoShown()
        {
            print("Video shown");
        }

        public void OnRewardedVideoClicked()
        {
            print("Video is clicked");
        }

        public void OnRewardedVideoClosed(bool finished)
        {
            print("Video closed");
        }

        public void OnRewardedVideoExpired()
        {
            print("Video expired");
        }

        public void OnRewardedVideoFinished(double amount, string currency)
        {
            AddScore((int)amount);
        }
        #endregion
    }
}
