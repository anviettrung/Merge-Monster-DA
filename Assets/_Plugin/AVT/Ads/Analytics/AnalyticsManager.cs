using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using AppsFlyerSDK;

public class FirebaseStringEvent
{
    public const string level_engage = "level_engage";
    public const string level_engage_passed = "level_engage_passed";
    public const string impdau_inter_passed = "impdau_inter_passed";
    public const string impdau_reward_passed = "impdau_reward_passed";

    public const string level_start = "level_start";
    public const string level_passed = "level_passed";
    public const string level_failed = "level_failed";

    public const string show_interstitial_ads = "show_interstitial_ads";
    public const string show_rewarded_ads = "show_rewarded_ads";
}

public class AnalyticsManager : MonoBehaviour
{
    private static FirebaseManager m_FirebaseManager => FirebaseManager.Instance;

    public void Awake()
    {
        EventManager.StartListening(FirebaseStringEvent.show_interstitial_ads, LogEventImpdauInterPassed);
        EventManager.StartListening(FirebaseStringEvent.show_rewarded_ads, LogEventImpdauRewardPassed);

        EventManager.StartListening(FirebaseStringEvent.show_interstitial_ads, LogEventShowInterstitial);
        EventManager.StartListening(FirebaseStringEvent.show_rewarded_ads, LogEventShowReward);
    }

    public static void LogEventLevelEngage(int level, int result)
    {
        m_FirebaseManager?.LogAnalyticsEvent(
            FirebaseStringEvent.level_engage,
            new Parameter("Level", level),
            new Parameter("Result", result == 0 ? "Lose" : "Win"));
    }

    public static void LogEventLevelEngagePassed(int engageWithLevel)
    {
        m_FirebaseManager?.LogAnalyticsEvent(
            FirebaseStringEvent.level_engage_passed,
            new Parameter("EngageWithLevel", engageWithLevel));
    }

    public static void LogEventImpdauInterPassed()
    {
        int impdauPassed = PlayerPrefs.GetInt("impdau_inter_passed", 0);
        impdauPassed++;
        PlayerPrefs.SetInt("impdau_inter_passed", impdauPassed);

        m_FirebaseManager?.LogAnalyticsEvent(
            FirebaseStringEvent.impdau_inter_passed,
            new Parameter("ImpdauPassed", impdauPassed));

        // Log appsflyer
        var para = new Dictionary<string, string>();
        para.Add("ImpdauPassed", impdauPassed.ToString());
        AppsFlyer.sendEvent(FirebaseStringEvent.impdau_inter_passed, para);
    }

    public static void LogEventImpdauRewardPassed()
    {
        int impdauPassed = PlayerPrefs.GetInt("impdau_reward_passed", 0);
        impdauPassed++;
        PlayerPrefs.SetInt("impdau_reward_passed", impdauPassed);

        m_FirebaseManager?.LogAnalyticsEvent(
            FirebaseStringEvent.impdau_reward_passed,
            new Parameter("ImpdauPassed", impdauPassed));

        // Log appsflyer
        var para = new Dictionary<string, string>();
        para.Add("ImpdauPassed", impdauPassed.ToString());
        AppsFlyer.sendEvent(FirebaseStringEvent.impdau_reward_passed, para);
    }

    public static void LogEventLevelStart(int level)
    {
        m_FirebaseManager?.LogAnalyticsEvent(
            FirebaseStringEvent.level_start,
            new Parameter("Level", level));
    }

    public static void LogEventLevelPassed(int level)
    {
        m_FirebaseManager?.LogAnalyticsEvent(
            FirebaseStringEvent.level_passed,
            new Parameter("Level", level));
    }

    public static void LogEventLevelFailed(int level)
    {
        m_FirebaseManager?.LogAnalyticsEvent(
            FirebaseStringEvent.level_failed,
            new Parameter("Level", level));
    }

    public static void LogEventShowInterstitial() => LogEventShowInterstitial(1, "");
    public static void LogEventShowInterstitial(int has_ads, string placement)
    {
        m_FirebaseManager?.LogAnalyticsEvent(
            FirebaseStringEvent.show_interstitial_ads,
            new Parameter("has_ads", has_ads == 0 ? "No" : "Yes"),
            new Parameter("internet_available", HasInternet()),
            new Parameter("placement", placement));
    }

    public static void LogEventShowReward() => LogEventShowReward(1, "");
    public static void LogEventShowReward(int has_ads, string placement)
    {
        m_FirebaseManager?.LogAnalyticsEvent(
            FirebaseStringEvent.show_rewarded_ads,
            new Parameter("has_ads", has_ads == 0 ? "No" : "Yes"),
            new Parameter("internet_available", HasInternet()),
            new Parameter("placement", placement));
    }

    public static string HasInternet()
    {
        return Application.internetReachability == NetworkReachability.NotReachable ? "No" : "Yes";
    }
}
