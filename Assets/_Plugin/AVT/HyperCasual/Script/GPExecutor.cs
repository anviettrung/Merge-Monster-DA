using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using AVT;

public class GPExecutor : Singleton<GPExecutor>
{
	public bool isTest = false;
	public int gameplaySceneIndex = 1;
	public int bonusLevelSceneIndex = 1;

	[SerializeField] GLDBase defaultGLD;
	[SerializeField] GPBase gameplay;

	public GPMerge Gameplay => (GPMerge)gameplay;
	public GameDatabase gameDB => GameDatabase.Instance;

	public UnityEvent onInitializationDone = new UnityEvent();

	float timePlayed = 0;

	private void OnValidate()
    {
		if (gameplay == null)
			gameplay = GetComponent<GPBase>();
    }

    private void Start()
	{
		GLDBase levelDataToPlay = defaultGLD;

		gameplay.LoadGPData(levelDataToPlay);
		gameplay.OnInitializationGame();
		onInitializationDone.Invoke();
	}

	public void StartGame()
	{
		gameplay.OnStartGame();

		AnalyticsManager.LogEventLevelStart(SaveLoad.SaveFile.CurrentGameLevel);
	}

	private void Update()
	{
		if (gameplay.IsGameStared)
			gameplay.CheckGameLogic();
		timePlayed += Time.deltaTime;
	}

	public void Replay()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

	public void NextLevel()
	{
		if (SaveLoad.IsExist)
		{
			SaveLoad.SaveFile.CurrentGameLevel += 1;
			AnalyticsManager.LogEventLevelEngagePassed(SaveLoad.SaveFile.CurrentGameLevel);
		}
		SceneManager.LoadScene(gameplaySceneIndex);
	}

	public void PlayLevel(int x)
	{
		if (SaveLoad.IsExist)
		{
			SaveLoad.SaveFile.CurrentGameLevel = x;
		}

		SceneManager.LoadScene(gameplaySceneIndex);
	}

	public void PlayBonusLevel()
	{
		SceneManager.LoadScene(bonusLevelSceneIndex);
	}
}
