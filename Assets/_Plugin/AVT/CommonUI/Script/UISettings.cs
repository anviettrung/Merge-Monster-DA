using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject pauseGamePanel;

    [Header("Settings")]
    [SerializeField] Button settingsButton;
    [SerializeField] Button pauseButton;
    [SerializeField] Toggle soundToggle;
    [SerializeField] Toggle vibrationToggle;

    public bool IsPause { private set; get; }

    private void Awake()
    {
        soundToggle.onValueChanged.AddListener(SetSoundToggle);
        vibrationToggle.onValueChanged.AddListener(SetVibrationToggle);

        soundToggle.isOn = PlayerPrefs.GetInt("Sound On", 1) == 1;
        vibrationToggle.isOn = PlayerPrefs.GetInt("Vibration On", 1) == 1;
    }

	private void Start()
	{
        // Init on Start
        SetSoundToggle(PlayerPrefs.GetInt("Sound On", 1) == 1);
        SetVibrationToggle(PlayerPrefs.GetInt("Vibration On", 1) == 1);
    }

    #region Settings Functions

    public void PauseGame()
    {
        IsPause = true;
        if (pauseGamePanel != null) 
            pauseGamePanel.SetActive(true);
        Time.timeScale = 0;
        AudioListener.volume = 0;
    }

    public void ResumeGame()
    {
        IsPause = false;
        if (pauseGamePanel != null)
            pauseGamePanel.SetActive(false);
        Time.timeScale = 1;
        AudioListener.volume = PlayerPrefs.GetInt("Sound On", 1);
    }

    public void SetSoundToggle(bool value)
    {
        PlayerPrefs.SetInt("Sound On", value ? 1 : 0);
        AudioListener.volume = value ? 1 : 0;
    }

    public void SetVibrationToggle(bool value)
    {
        PlayerPrefs.SetInt("Vibration On", value ? 1 : 0);
#if MOREMOUNTAINS_NICEVIBRATIONS
        MoreMountains.NiceVibrations.MMVibrationManager.SetHapticsActive(value);
#endif
    }

#endregion
}
