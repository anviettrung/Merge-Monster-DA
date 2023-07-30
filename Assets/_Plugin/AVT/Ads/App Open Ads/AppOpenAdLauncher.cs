using GoogleMobileAds.Api;
using UnityEngine;

public class AppOpenAdLauncher : Singleton<AppOpenAdLauncher>
{
    public bool enableAds = false;
    public bool isTestAds = false;

    protected void Awake()
    {
        if (enableAds)
        {
            AppOpenAdManager.IsTestAds = isTestAds;
            MobileAds.Initialize(status => { AppOpenAdManager.Instance.LoadAd(); });

            // By default, AOA will show immediately whenever an interstitial or reward ad was dismissed from screen (OnApplicationPause)
            // We have to track if the game was unpaused from ads.
            //EventManager.StartListening("show_interstitial_ads", SetResumeFromAdsToTrue);
            //EventManager.StartListening("show_reward_ads", SetResumeFromAdsToTrue);

			//EventManager.StartListening("hidden_interstitial_ads", SetResumeFromAdsToFalse);
			//EventManager.StartListening("hidden_reward_ads", SetResumeFromAdsToFalse);
		}
    }

    private void OnApplicationPause(bool pause)
    {
#if UNITY_ANDROID
        if (!pause && AppOpenAdManager.ConfigResumeApp && !AppOpenAdManager.ResumeFromAds && enableAds)
        {
            AppOpenAdManager.Instance.ShowAdIfAvailable();
        }

        // Reset after when the app is foregrounded
        //if (!pause)
        //	AppOpenAdManager.ResumeFromAds = false;
#elif UNITY_IOS
        if (!pause && AppOpenAdManager.ConfigResumeApp && !AppOpenAdManager.ResumeFromAds && enableAds)
        {
            AppOpenAdManager.Instance.ShowAdIfAvailable();
        }
#else
        AD_UNIT_ID = "unexpected_platform";
#endif
    }

    public void SetResumeFromAdsToTrue() => AppOpenAdManager.ResumeFromAds = true;
    public void SetResumeFromAdsToFalse() => AppOpenAdManager.ResumeFromAds = false;

    private bool didShowAoALoadingTime = false;
    private float lastCheck = 0;
    public void ShowAOAIfLoadedInLoadingTime()
	{
        if (didShowAoALoadingTime || lastCheck  + 0.2f > Time.time)
            return;

        lastCheck = Time.time;
        didShowAoALoadingTime = AppOpenAdManager.Instance.ShowAdIfAvailable();
	}
}