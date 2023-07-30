using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using System;
using System.Threading.Tasks;
using UnityEngine.Events;


public class FirebaseRemoteConfigManager
{
    private UnityAction m_FetchSuccessCallback;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    public void SetupDefaultConfigs(Dictionary<string, object> defaults)
    {
        if (defaults == null) return;
		try
		{
			FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
			ConfigSettings cs = FirebaseRemoteConfig.DefaultInstance.ConfigSettings;
			FirebaseRemoteConfig.DefaultInstance.SetConfigSettingsAsync(cs);
		}
		catch (Exception)
		{
		}
	}
    public ConfigValue GetValues(string key)
    {
        return FirebaseRemoteConfig.DefaultInstance.GetValue(key);
    }

    public void FetchData(UnityAction fetchSuccessCallback)
    {
        m_FetchSuccessCallback = fetchSuccessCallback;
        // FetchAsync only fetches new data if the current data is older than the provided
        // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
        // By default the timespan is 12 hours, and for production apps, this is a good
        // number.  For this example though, it's set to a timespan of zero, so that
        // changes in the console will always show up immediately.
        try
        {
            Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                TimeSpan.Zero);
            fetchTask.ContinueWith(FetchComplete);

        }
        catch (Exception)
        {
        }
    }

    private void FetchComplete(Task fetchTask)
    {
        if (fetchTask.IsCanceled)
        {
            Debug.Log("Fetch canceled.");
        }
        else if (fetchTask.IsFaulted)
        {
            Debug.Log("Fetch encountered an error.");
        }
        else if (fetchTask.IsCompleted)
        {
            Debug.Log("Fetch completed successfully!");
        }
        var info = FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case LastFetchStatus.Success:
                FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync();
                Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
                    info.FetchTime));
                if (m_FetchSuccessCallback != null)
                {
                    m_FetchSuccessCallback();
                }
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:
                        Debug.Log("Fetch failed for unknown reason");
                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");
                break;
        }
    }

    public void SetupDefaultConfigs()
    {
		Dictionary<string, object> defaults =
				new Dictionary<string, object>();

		defaults.Add(Keys.config_enable_interstitial, false);
		defaults.Add(Keys.config_enable_aoa, false);

		SetupDefaultConfigs(defaults);
	}
}
