using UnityEngine;

public class MAXQuickCall : MonoBehaviour
{
    public void ShowInterstitial()
    {
        MAXManager.Instance.ShowInterstitialAd();
    }

    public void ShowInterstitialIfOldPlayer()
    {
        if (PlayerPrefs.HasKey("play_game_second_time"))
        {
            MAXManager.Instance.ShowInterstitialAd();
        }
        else
        {
            PlayerPrefs.SetInt("play_game_second_time", 1);
        }
    }
}
