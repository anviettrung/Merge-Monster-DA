using Firebase.Analytics;
using Firebase.RemoteConfig;
using System;
using UnityEngine;
using UnityEngine.Events;

public class FirebaseManager : MonoBehaviour 
{
    // temp
    public bool enableConfig = false;

    private static FirebaseManager m_Instance = null;
    private bool IsLoaded = false;

    public static FirebaseManager Instance {
        get {
            return m_Instance;
        }
    }

    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    private FirebaseRemoteConfigManager m_FirebaseRemoteConfigManager;

    private void Awake() {
        // singleton pattern
        if(m_Instance != null) {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    public void Init() {
        m_Instance = this;

        if (m_FirebaseRemoteConfigManager == null)
        {
            m_FirebaseRemoteConfigManager = new FirebaseRemoteConfigManager();
        }

        Debug.Log("Start Config");
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                InitializeFirebase();
            } else {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase() {
        IsLoaded = true;

        if (enableConfig)
        {
            m_FirebaseRemoteConfigManager.SetupDefaultConfigs();
            FetchData(() =>
            {
                Debug.Log("Fetch Success");
                EventManager.TriggerEvent("UpdateRemoteConfigs");
            });
        }
    }

    public bool IsFirebaseReady() {
        return IsLoaded;
    }

    #region Remote Config

    public void FetchData(UnityAction successCallback)
    {
        m_FirebaseRemoteConfigManager.FetchData(successCallback);
    }
    public ConfigValue GetConfigValue(string key)
    {
        return m_FirebaseRemoteConfigManager.GetValues(key);
    }
    #endregion

    #region Analytics
    public void LogAnalyticsEvent(string eventName, string parameterName, double value) {
        if (IsFirebaseReady()) {
            FirebaseAnalytics.LogEvent(eventName, parameterName, value);
        }
    }

    public void LogAnalyticsEvent(string eventName, params Parameter[] param) {
        if (IsFirebaseReady()) {
            FirebaseAnalytics.LogEvent(eventName, param);
        }
    }

    public void LogAnalyticsEvent(string eventName) {
        if (IsFirebaseReady()) {
            FirebaseAnalytics.LogEvent(eventName);
        }
    }

    public void SetUserProperty(string propertyName, string property) {
        if (IsFirebaseReady()) {
            FirebaseAnalytics.SetUserProperty(propertyName, property);
        }
    }
    #endregion
}
