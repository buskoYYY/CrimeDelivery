using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;
using Io.AppMetrica;
using TMPro;
using UnityEngine;

namespace ArcadeBridge
{
    public class AppodealTest : MonoBehaviour, IRewardedVideoAdListener
    {
        private const string _appkey = "17bd909f221c8a33d00d1933661d5ddc70059d24f6f0faac";

        [SerializeField] private int _score;
        [SerializeField] private TextMeshProUGUI _scoreText;
        
        private int _adTypes;


        private void Start()
        {
            _adTypes = AppodealAdType.Interstitial | AppodealAdType.Banner | AppodealAdType.RewardedVideo | AppodealAdType.Mrec;
            AppodealCallbacks.Sdk.OnInitialized += OnInitializationFinished;
            Initialized();
        }

        public void ShowInterstitialAds()
        {
            if(Appodeal.CanShow(AppodealAdType.Interstitial))
            {
                Appodeal.Show(AppodealAdType.Interstitial);
            }
        }

        public void ShowRewardAds() 
        {
            if (Appodeal.IsLoaded(AppodealAdType.RewardedVideo))
            {
                Appodeal.Show(AppodealShowStyle.RewardedVideo);
            }
        }

        private void UpdateScore()
        {
            _scoreText.text = _score.ToString();
        }

        public void OnInitializationFinished(object sender, SdkInitializedEventArgs e) {}

        private void Initialized()
        {
            Appodeal.Initialize(_appkey, _adTypes);

            Appodeal.MuteVideosIfCallsMuted(true);

            Appodeal.SetRewardedVideoCallbacks(this);
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
            _score++;
            UpdateScore();
        }

        #endregion
    }

}
