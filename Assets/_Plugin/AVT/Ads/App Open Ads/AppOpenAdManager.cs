﻿using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class AppOpenAdManager
{
#if UNITY_ANDROID
    private const string ID_TIER_1 = "ca-app-pub-6336405384015455/9721840527";
    private const string ID_TIER_2 = "ca-app-pub-6336405384015455/8408758850";
    private const string ID_TIER_3 = "ca-app-pub-6336405384015455/5777853941";

#elif UNITY_IOS
    private const string ID_TIER_1 = "ca-app-pub-6336405384015455/6805704042";
    private const string ID_TIER_2 = "ca-app-pub-6336405384015455/4179540703";
    private const string ID_TIER_3 = "ca-app-pub-6336405384015455/5301050689";
#else
    private const string ID_TIER_1 = "";
    private const string ID_TIER_2 = "";
    private const string ID_TIER_3 = "";
#endif

    private static AppOpenAdManager instance;

    private AppOpenAd ad;

    private DateTime loadTime;

    private bool isShowingAd = false;

    private bool showFirstOpen = false;

    public static bool ConfigOpenApp = true;
    public static bool ConfigResumeApp = true;

    public static bool ResumeFromAds = false;

    public static bool IsTestAds = false;

    public static AppOpenAdManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AppOpenAdManager();
            }

            return instance;
        }
    }

    private bool IsAdAvailable => ad != null && (System.DateTime.UtcNow - loadTime).TotalHours < 4;

    private int tierIndex = 1;

    public void LoadAd()
    {
        // if (IAP_AD_REMOVED)
        //     return;

        LoadAOA();
    }

    public void LoadAOA()
    {
        string id = ID_TIER_1;
        if (tierIndex == 2)
            id = ID_TIER_2;
        else if (tierIndex == 3)
            id = ID_TIER_3;

        if (IsTestAds)
        {
#if UNITY_ANDROID
            id = "ca-app-pub-3940256099942544/3419835294";
#elif UNITY_IOS
            id = "ca-app-pub-3940256099942544/5662855259";
#else
            id = "unexpected_platform";
#endif
        }
        Debug.Log("Start request Open App Ads Tier " + tierIndex);

        AdRequest request = new AdRequest.Builder().Build();

        AppOpenAd.LoadAd(id, ScreenOrientation.Portrait, request, ((appOpenAd, error) =>
        {
            if (error != null)
            {
                // Handle the error.
                Debug.LogFormat("Failed to load the ad. (reason: {0}), tier {1}", error.LoadAdError.GetMessage(), tierIndex);
                tierIndex++;
                if (tierIndex <= 3)
                    LoadAOA();
                else
                    tierIndex = 1;
                return;
            }

            // App open ad is loaded.
            ad = appOpenAd;
            tierIndex = 1;
            loadTime = DateTime.UtcNow;
            //if (!showFirstOpen && ConfigOpenApp)// && FirebaseManager.Instance.GetConfigValue(Keys.config_enable_aoa).BooleanValue)
            //{
            //    ShowAdIfAvailable();
            //    showFirstOpen = true;
            //}
            //showFirstOpen = true;
        }));
    }

    public bool ShowAdIfAvailable()
    {
        if (!IsAdAvailable || isShowingAd)
        {
            return false;
        }

        ad.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
        ad.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
        ad.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
        ad.OnAdDidRecordImpression += HandleAdDidRecordImpression;
        ad.OnPaidEvent += HandlePaidEvent;

        ad.Show();

        return true;
    }

    private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args)
    {
        Debug.Log("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        isShowingAd = false;

        EventManager.TriggerEvent("App Open Ad Close");
        LoadAd();
    }

    private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args)
    {
        Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        LoadAd();
    }

    private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args)
    {
        Debug.Log("Displayed app open ad");
        isShowingAd = true;
    }

    private void HandleAdDidRecordImpression(object sender, EventArgs args)
    {
        Debug.Log("Recorded ad impression");
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
            args.AdValue.CurrencyCode, args.AdValue.Value);
    }
}