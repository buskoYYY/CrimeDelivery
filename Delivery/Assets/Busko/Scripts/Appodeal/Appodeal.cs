using AppodealStack.Monetization.Common;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Appodeal : MonoBehaviour, IInterstitialAdListener, IRewardedVideoAdListener
{
    private const string _appkey = "17bd909f221c8a33d00d1933661d5ddc70059d24f6f0faac";

    public static Appodeal Instance;

    private int _score;
    private int _adTypes;
    private Action OnReward;

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
        _adTypes = AppodealAdType.Interstitial |AppodealAdType.RewardedVideo;
        AppodealCallbacks.Sdk.OnInitialized += OnInitializationFinished;
        Initialized();
    }

    public void ShowRewardAds(Action onReward)
    {
        OnReward = onReward;
        if (AppodealStack.Monetization.Api.Appodeal.IsLoaded(AppodealAdType.RewardedVideo) 
            || AppodealStack.Monetization.Api.Appodeal.CanShow(AppodealAdType.RewardedVideo))
        {
            AppodealStack.Monetization.Api.Appodeal.Show(AppodealShowStyle.RewardedVideo);
        }
    }

    public int AddScore(int points)
    {
        _score += points;
        return _score;
    }

    public void OnInitializationFinished(object sender, SdkInitializedEventArgs e) { }

    public bool ShowInterstitialAds()
    {
        if (AppodealStack.Monetization.Api.Appodeal.IsLoaded(AppodealAdType.Interstitial)
            && AppodealStack.Monetization.Api.Appodeal.CanShow(AppodealAdType.Interstitial))
        {
            AppodealStack.Monetization.Api.Appodeal.Show(AppodealAdType.Interstitial);
            return true;
        }
        return false;
    }

    private void Initialized()
    {
        AppodealStack.Monetization.Api.Appodeal.Initialize(_appkey, _adTypes);

        AppodealStack.Monetization.Api.Appodeal.MuteVideosIfCallsMuted(true);

        AppodealStack.Monetization.Api.Appodeal.SetInterstitialCallbacks(this);

        AppodealStack.Monetization.Api.Appodeal.SetRewardedVideoCallbacks(this);
    }

    #region RewardedVideoCallback
    public void OnRewardedVideoLoaded(bool isPrecache)
    {
        print("Video loaded");
    }

    public void OnRewardedVideoFailedToLoad()
    {
        //SceneManager.LoadScene(1);
    }

    public void OnRewardedVideoShowFailed()
    {
        SceneManager.LoadScene(1);
    }

    public void OnRewardedVideoShown()
    {
        OnReward?.Invoke();
        SceneManager.LoadScene(1);
    }

    public void OnRewardedVideoClicked()
    {
        SceneManager.LoadScene(1);
    }

    public void OnRewardedVideoClosed(bool finished)
    {
        SceneManager.LoadScene(1);
    }

    public void OnRewardedVideoExpired()
    {
        SceneManager.LoadScene(1);
    }

    public void OnRewardedVideoFinished(double amount, string currency)
    {
        AddScore((int)amount);
    }

    #endregion

    #region InterstitialVideoCallback


    public void OnInterstitialLoaded(bool isPrecache)
    {
        print("Video loaded");
    }

    public void OnInterstitialFailedToLoad()
    {
        //SceneManager.LoadScene(1);
    }

    public void OnInterstitialShowFailed()
    {
        SceneManager.LoadScene(1);
    }

    public void OnInterstitialShown()
    {
        SceneManager.LoadScene(1);
    }

    public void OnInterstitialClosed()
    {
        SceneManager.LoadScene(1); 
    }

    public void OnInterstitialClicked()
    {
        SceneManager.LoadScene(1);
    }

    public void OnInterstitialExpired()
    {
        SceneManager.LoadScene(1);
    }
    #endregion
}
