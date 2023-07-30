using AppsFlyerSDK;
using Firebase.Analytics;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;


public class AnalyticsRevenueAds
{
    public static string AppsflyerID;
    public static string FirebaseID;

    public static async void Init()
    {
        AppsflyerID = AppsFlyer.getAppsFlyerId();
        FirebaseID = await GetAnalyticsInstanceId();

    }

    public static void SendEvent(ImpressionData data, AdFormat type)
    {
        SendEventRealtime(data, type);

        SendEventThreshold(data, type);

        //  SendEventServer(data, type);
    }

    private static void SendEventRealtime(ImpressionData data, AdFormat type)
    {
        Firebase.Analytics.Parameter[] AdParameters = {
             new Firebase.Analytics.Parameter("ad_platform", "applovin"),
             new Firebase.Analytics.Parameter("ad_source", data.NetworkName),
             new Firebase.Analytics.Parameter("ad_unit_name", data.AdUnitIdentifier),
             new Firebase.Analytics.Parameter("currency","USD"),
             new Firebase.Analytics.Parameter("value",data.Revenue),
             new Firebase.Analytics.Parameter("placement",data.Placement),
             new Firebase.Analytics.Parameter("country_code",data.CountryCode),
             new Firebase.Analytics.Parameter("ad_format",data.AdFormat),
        };

        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression_rocket", AdParameters);
    }

    private static void SendEventThreshold(ImpressionData data, AdFormat type)
    {

        var rev = GetRevenueCache(type);
        rev += data.Revenue;
        var time = GetTimeLogin(type);
		// bool isMaxDay = CheckConditionDay(time, RocketRemoteConfig.GetIntConfig("config_max_day_send_revenue", 1));

		if (rev >= 0.1f)
		{
			// send event
			Firebase.Analytics.Parameter[] AdParameters = {
					new Firebase.Analytics.Parameter("ad_platform", "applovin"),
					new Firebase.Analytics.Parameter("ad_source", data.NetworkName),
					new Firebase.Analytics.Parameter("ad_unit_name", data.AdUnitIdentifier),
					new Firebase.Analytics.Parameter("currency","USD"),
					new Firebase.Analytics.Parameter("value",rev),
					new Firebase.Analytics.Parameter("placement",data.Placement),
					new Firebase.Analytics.Parameter("country_code",data.CountryCode),
					new Firebase.Analytics.Parameter("ad_format",data.AdFormat),
					  };

			Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression_threshold", AdParameters);

			SetRevenueCache(type, 0);
			SetTimeLogin(type, DateTime.Now.ToString());
		}
		else
		{
			SetRevenueCache(type, rev);
        }

    }

    private static void SendEventServer(ImpressionData data, AdFormat type)
    {
        var form = new WWWForm(); //here you create a new form connection

#if UNITY_EDITOR
        form.AddField("platform", 2);
#elif UNITY_IOS
            form.AddField("platform", 1);
#else
            form.AddField("platform", 0);
#endif
        //form.AddField("packagename", RocketConfig.package_name);
        form.AddField("ad_platform", "applovin");
        form.AddField("ad_source", data.NetworkName);
        form.AddField("ad_unit_name", data.AdUnitIdentifier);
        form.AddField("ad_format", data.AdFormat);
        form.AddField("currency", "USD");
        form.AddField("value", data.Revenue.ToString());
        form.AddField("appsflyer_id", AppsflyerID);
        form.AddField("firebase_id", FirebaseID);

        //send
        ObservableWWW.Post("http://analytics.rocketstudio.com.vn:2688/api/firebase_analystic", form).Subscribe(
            x =>
            {

                Debug.Log("SendAnalystic Done");
            }, // onSuccess
            ex =>
            {
                Debug.Log("SendAnalystic ex" + ex.Message);
                //MobileNativeMessage msg = new MobileNativeMessage("TAPP.vn", "Có lỗi xảy ra");
            }// onError
        );
    }

    private static double GetRevenueCache(AdFormat type)
    {
        return PlayerPrefs.GetFloat("revenueAd" + type, 0);

    }

    private static void SetRevenueCache(AdFormat type, double rev)
    {

        PlayerPrefs.SetFloat("revenueAd" + type, (float)rev);
    }

    private static bool CheckConditionDay(string stringTimeCheck, int maxDays)
    {
        if (string.IsNullOrEmpty(stringTimeCheck))
        {

            return false;
        }
        try
        {
            DateTime timeNow = DateTime.Now;
            DateTime timeOld = DateTime.Parse(stringTimeCheck);
            DateTime timeOldCheck = new DateTime(timeOld.Year, timeOld.Month, timeOld.Day, 0, 0, 0);
            long tickTimeNow = timeNow.Ticks;
            long tickTimeOld = timeOldCheck.Ticks;

            long elapsedTicks = tickTimeNow - tickTimeOld;
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
            double totalDay = elapsedSpan.TotalDays;

            if (totalDay >= maxDays)
            {
                return true;
            }
        }
        catch
        {
            return true;
        }

        return false;
    }

    private static string GetTimeLogin(AdFormat type)
    {
        return PlayerPrefs.GetString("time_login_check_rev" + type, DateTime.Now.ToString());
    }

    private static void SetTimeLogin(AdFormat type, string time)
    {
        PlayerPrefs.SetString("time_login_check_rev" + type, time);
    }

    public static Task<string> GetAnalyticsInstanceId()
    {
        return FirebaseAnalytics.GetAnalyticsInstanceIdAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                //DebugLog("App instance ID fetch was canceled.");
            }
            else if (task.IsFaulted)
            {
                //DebugLog(String.Format("Encounted an error fetching app instance ID {0}",
                //task.Exception.ToString()));
            }
            else if (task.IsCompleted)
            {
                //DebugLog(String.Format("App instance ID: {0}", task.Result));
            }
            return task;
        }).Unwrap();
    }


}

public class ImpressionData
{
    public string CountryCode;
    public string NetworkName;
    public string AdUnitIdentifier;
    public string Placement;
    public double Revenue;
    public string AdFormat;

}

public enum AdFormat
{
    interstitial,
    video_rewarded,
    banner
}
