using UnityEngine;

public class MAXManager : Singleton<MAXManager>
{
    public bool enableAds;

    [Header("Settings")]
    public float interstitial_Cooldown = 20;
    float interstitialReadyTime = 0;

    [Header("ID")]
    public string interstitialAdUnitId = "YOUR_AD_UNIT_ID";
    public string rewardAdUnitId = "YOUR_AD_UNIT_ID";
    public string bannerAdUnitId = "YOUR_AD_UNIT_ID";
    public string SdkKey = "Your_SDK_Key";

    private System.Action DoAfterRewardedComplete;

    int interstitialRetryAttempt;
    int rewardRetryAttempt;

    void Awake()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            // AppLovin SDK is initialized, start loading ads

            InitializeInterstitialAds();
            InitializeRewardedAds();
            InitializeBannerAds();
        };

        //MaxSdk.SetUserId(AppsFlyer.getAppsFlyerId());
        MaxSdk.SetSdkKey(SdkKey);
        MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();

        if (enableAds)
        {
            MaxSdk.ShowBanner(bannerAdUnitId);
        }

        interstitialReadyTime = 0;
    }
    public void ShowInterstitialAd()
    {
        if (enableAds) // && FirebaseManager.Instance.GetConfigValue(Keys.config_enable_interstitial).BooleanValue)
        {
            if (MaxSdk.IsInterstitialReady(interstitialAdUnitId) && interstitialReadyTime <= Time.time)
            {
                AppOpenAdManager.ResumeFromAds = true;
                // Pause game
                Time.timeScale = 0;
                EventManager.TriggerEvent("show_interstitial_ads");
                MaxSdk.ShowInterstitial(interstitialAdUnitId);
            }
        }
    }

    public bool ShowRewardedAd(System.Action action)
    {
        if (enableAds)
        {
            DoAfterRewardedComplete = action;
            if (MaxSdk.IsRewardedAdReady(rewardAdUnitId))
            {
                AppOpenAdManager.ResumeFromAds = true;
                // Pause game
                Time.timeScale = 0;
                EventManager.TriggerEvent("show_rewarded_ads");
                MaxSdk.ShowRewardedAd(rewardAdUnitId);

                return true;
            } else
			{
                return false;
			}
        }
        else
        {
            action();
            return true;
        }
    }

    #region Interstitial
    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(interstitialAdUnitId);
        EventManager.TriggerEvent("load_interstitial_ads");
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        interstitialRetryAttempt = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        interstitialRetryAttempt++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, interstitialRetryAttempt));

        Invoke("LoadInterstitial", (float)retryDelay);

        EventManager.TriggerEvent("load_failed_interstitial_ads");
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        EventManager.TriggerEvent("display_interstitial_ads");
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();

        // Resume game
        Time.timeScale = 1;
        EventManager.TriggerEvent("display_failed_interstitial_ads");
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        EventManager.TriggerEvent("click_interstitial_ads");
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();

        interstitialReadyTime = Time.time + interstitial_Cooldown;

        // Resume game
        Time.timeScale = 1;
        AppOpenAdManager.ResumeFromAds = false;
        EventManager.TriggerEvent("hidden_interstitial_ads");
    }

    private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Interstitial revenue paid");

        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        var data = new ImpressionData();
        data.AdFormat = "interstitial";
        data.AdUnitIdentifier = adUnitIdentifier;
        data.CountryCode = countryCode;
        data.NetworkName = networkName;
        data.Placement = placement;
        data.Revenue = revenue;

        AnalyticsRevenueAds.SendEvent(data, AdFormat.interstitial);
    }

    #endregion

    #region Rewarded
    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(rewardAdUnitId);
        EventManager.TriggerEvent("load_reward_ads");
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.
        
        // Reset retry attempt
        rewardRetryAttempt = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        rewardRetryAttempt++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, rewardRetryAttempt));

        Invoke("LoadRewardedAd", (float)retryDelay);

        EventManager.TriggerEvent("load_failed_reward_ads");
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        EventManager.TriggerEvent("display_reward_ads");
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
        
        // Resume game
        Time.timeScale = 1;
        EventManager.TriggerEvent("display_failed_reward_ads");
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        EventManager.TriggerEvent("click_reward_ads");
    }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();

        // Resume game
        Time.timeScale = 1;
        AppOpenAdManager.ResumeFromAds = false;
        EventManager.TriggerEvent("hidden_reward_ads");
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        EventManager.TriggerEvent("receive_reward_ads");
        DoAfterRewardedComplete();
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Rewarded ad revenue paid");

        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        var data = new ImpressionData();
        data.AdFormat = "video_reward";
        data.AdUnitIdentifier = adUnitIdentifier;
        data.CountryCode = countryCode;
        data.NetworkName = networkName;
        data.Placement = placement;
        data.Revenue = revenue;

        AnalyticsRevenueAds.SendEvent(data, AdFormat.video_rewarded);

        // Ad revenue paid. Use this callback to track user revenue.
        EventManager.TriggerEvent("revenue_paid_reward_ads");
    }
    #endregion

    #region Banner
    public void InitializeBannerAds()
    {
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;

        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.black);

        if (enableAds)
            MaxSdk.ShowBanner(bannerAdUnitId);
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Banner ad is ready to be shown.
        // If you have already called MaxSdk.ShowBanner(BannerAdUnitId) it will automatically be shown on the next ad refresh.
        Debug.Log("Banner ad loaded");
        //GameManager.Instance.ShowBannerAds();
    }

    private void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Banner ad failed to load. MAX will automatically try loading a new ad internally.
        Debug.Log("Banner ad failed to load with error code: " + errorInfo.Code);
    }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Banner ad clicked");
    }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Banner ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Banner ad revenue paid");

        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        var data = new ImpressionData();
        data.AdFormat = "banner";
        data.AdUnitIdentifier = adUnitIdentifier;
        data.CountryCode = countryCode;
        data.NetworkName = networkName;
        data.Placement = placement;
        data.Revenue = revenue;

        AnalyticsRevenueAds.SendEvent(data, AdFormat.banner);
    }
    #endregion

}
